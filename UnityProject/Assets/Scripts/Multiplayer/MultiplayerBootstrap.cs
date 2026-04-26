using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerBootstrap : MonoBehaviour
{
    [Header("Match Settings")]
    [SerializeField] int minPlayers = 2;
    [SerializeField] int maxPlayers = 2;
    [SerializeField] float pollIntervalSeconds = 2f;
    [SerializeField] string multiplayerSceneName = "MapGeneratorScene";

    public static MultiplayerBootstrap Instance { get; private set; }

    MatchmakingClient matchmakingClient;
    RelayLobbyClient relayLobbyClient;
    MatchmakingClient.MatchResponse currentMatch;
    Coroutine pollRoutine;
    bool relayFlowInProgress;
    bool relayConnected;
    bool loadingGameScene;

    public MatchmakingClient.MatchResponse CurrentMatch => currentMatch;
    public bool HasMatch => currentMatch != null && !string.IsNullOrEmpty(currentMatch.matchId);
    public bool IsHost => HasMatch && currentMatch.hostUserId == AuthSession.UserId;

    public static MultiplayerBootstrap GetOrCreate()
    {
        if (Instance != null)
        {
            return Instance;
        }

        Instance = FindFirstObjectByType<MultiplayerBootstrap>();
        if (Instance != null)
        {
            return Instance;
        }

        GameObject go = new GameObject("MultiplayerBootstrap");
        Instance = go.AddComponent<MultiplayerBootstrap>();
        return Instance;
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        matchmakingClient = MatchmakingClient.GetOrCreate();
        relayLobbyClient = RelayLobbyClient.GetOrCreate();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public void FindMatch()
    {
        if (!AuthSession.IsAuthenticated)
        {
            Debug.LogWarning("[MULTIPLAYER] Debes iniciar sesion antes de buscar partida.");
            return;
        }

        relayFlowInProgress = false;
        relayConnected = false;
        loadingGameScene = false;

        matchmakingClient.JoinQueue(
            response =>
            {
                HandleMatchUpdated(response);
                StartPolling();
                Debug.Log("[MULTIPLAYER] Match " + response.matchId + " status=" + response.status + " role=" + response.role);
            },
            error => Debug.LogWarning("[MULTIPLAYER] Error buscando partida: " + error),
            minPlayers,
            maxPlayers);
    }

    public void RefreshMatch()
    {
        if (!HasMatch)
        {
            return;
        }

        matchmakingClient.GetMatch(
            currentMatch.matchId,
            HandleMatchUpdated,
            error => Debug.LogWarning("[MULTIPLAYER] Error refrescando partida: " + error));
    }

    public void MarkReady(bool ready)
    {
        if (!HasMatch)
        {
            return;
        }

        matchmakingClient.SetReady(
            currentMatch.matchId,
            ready,
            HandleMatchUpdated,
            error => Debug.LogWarning("[MULTIPLAYER] Error cambiando ready: " + error));
    }

    public void LeaveCurrentMatch()
    {
        string matchId = HasMatch ? currentMatch.matchId : null;

        if (pollRoutine != null)
        {
            StopCoroutine(pollRoutine);
            pollRoutine = null;
        }

        currentMatch = null;
        relayFlowInProgress = false;
        relayConnected = false;
        loadingGameScene = false;

        if (RtsNetworkCommandBus.Instance != null)
        {
            RtsNetworkCommandBus.Instance.Deactivate();
        }

        if (RtsMultiplayerWorldInitializer.Instance != null)
        {
            RtsMultiplayerWorldInitializer.Instance.ResetInitializationState();
        }

        RtsEntityRegistry.Clear();

        if (relayLobbyClient != null)
        {
            relayLobbyClient.LeaveCurrentSession();
        }

        if (!string.IsNullOrEmpty(matchId) && matchmakingClient != null && AuthSession.IsAuthenticated)
        {
            matchmakingClient.LeaveMatch(
                matchId,
                _ => Debug.Log("[MULTIPLAYER] Partida abandonada en matchmaking."),
                error => Debug.LogWarning("[MULTIPLAYER] Error abandonando matchmaking: " + error));
        }
    }

    void StartPolling()
    {
        if (pollRoutine != null)
        {
            return;
        }

        pollRoutine = StartCoroutine(PollMatchRoutine());
    }

    IEnumerator PollMatchRoutine()
    {
        while (HasMatch && currentMatch.status != "starting")
        {
            yield return new WaitForSeconds(pollIntervalSeconds);
            RefreshMatch();
        }

        pollRoutine = null;
    }

    void HandleMatchUpdated(MatchmakingClient.MatchResponse response)
    {
        currentMatch = response;
        TryStartRelayFlow();
        TryLoadGameScene();
    }

    void TryStartRelayFlow()
    {
        if (!HasMatch || relayConnected || relayFlowInProgress)
        {
            return;
        }

        bool enoughPlayers = currentMatch.status == "matched" || currentMatch.status == "full" || currentMatch.status == "readying";
        if (IsHost && enoughPlayers && !HasRelayJoinCode())
        {
            relayFlowInProgress = true;
            relayLobbyClient.StartHostWithRelaySession(
                currentMatch.matchId,
                currentMatch.maxPlayers,
                response =>
                {
                    relayConnected = true;
                    relayFlowInProgress = false;
                    HandleMatchUpdated(response);
                    MarkReady(true);
                },
                error =>
                {
                    relayFlowInProgress = false;
                    Debug.LogWarning("[MULTIPLAYER] Error creando sesion Relay/Lobby: " + error);
                });
            return;
        }

        if (!IsHost && HasRelayJoinCode())
        {
            relayFlowInProgress = true;
            relayLobbyClient.JoinRelaySessionByCode(
                currentMatch.relay.relayJoinCode,
                () =>
                {
                    relayConnected = true;
                    relayFlowInProgress = false;
                    MarkReady(true);
                },
                error =>
                {
                    relayFlowInProgress = false;
                    Debug.LogWarning("[MULTIPLAYER] Error uniendo a sesion Relay/Lobby: " + error);
                });
        }
    }

    bool HasRelayJoinCode()
    {
        return currentMatch != null
            && currentMatch.relay != null
            && !string.IsNullOrEmpty(currentMatch.relay.relayJoinCode);
    }

    void TryLoadGameScene()
    {
        if (!HasMatch || loadingGameScene || currentMatch.status != "starting")
        {
            return;
        }

        loadingGameScene = true;
        SceneManager.LoadScene(multiplayerSceneName);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!HasMatch || currentMatch.status != "starting")
        {
            return;
        }

        RtsMultiplayerWorldInitializer.GetOrCreate().InitializeForCurrentMatch();
        RtsNetworkCommandBus.GetOrCreate().Activate();
    }

    public int GetLocalPlayerSlot()
    {
        return GetPlayerSlotByUserId(AuthSession.UserId);
    }

    public int GetPlayerSlotByUserId(int userId)
    {
        if (currentMatch == null || currentMatch.players == null)
        {
            return 0;
        }

        for (int i = 0; i < currentMatch.players.Length; i++)
        {
            if (currentMatch.players[i] != null && currentMatch.players[i].userId == userId)
            {
                return i;
            }
        }

        return -1;
    }

    public int GetPlayerCount()
    {
        if (currentMatch == null || currentMatch.players == null)
        {
            return 1;
        }

        return Mathf.Clamp(currentMatch.players.Length, 1, 4);
    }
}
