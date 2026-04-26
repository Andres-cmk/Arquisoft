using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;

public class RelayLobbyClient : MonoBehaviour
{
    public static RelayLobbyClient Instance { get; private set; }

    ISession currentSession;

    public string CurrentSessionId => currentSession != null ? currentSession.Id : null;
    public string CurrentJoinCode => currentSession != null ? currentSession.Code : null;

    public static RelayLobbyClient GetOrCreate()
    {
        if (Instance != null)
        {
            return Instance;
        }

        Instance = FindFirstObjectByType<RelayLobbyClient>();
        if (Instance != null)
        {
            return Instance;
        }

        GameObject go = new GameObject("RelayLobbyClient");
        Instance = go.AddComponent<RelayLobbyClient>();
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
    }

    async Task EnsureUnityServicesReadyAsync()
    {
        string profile = GetAuthProfileName();
        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            InitializationOptions options = new InitializationOptions().SetProfile(profile);
            await UnityServices.InitializeAsync(options);
        }
        else if (AuthenticationService.Instance.Profile != profile)
        {
            if (AuthenticationService.Instance.IsSignedIn)
            {
                AuthenticationService.Instance.SignOut(true);
            }

            AuthenticationService.Instance.SwitchProfile(profile);
        }

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public async void StartHostWithRelaySession(
        string matchId,
        int maxPlayers,
        Action<MatchmakingClient.MatchResponse> onSuccess,
        Action<string> onError)
    {
        try
        {
            RtsNetcodeRuntime.EnsureNetworkManager();
            await EnsureUnityServicesReadyAsync();

            SessionOptions options = new SessionOptions
            {
                MaxPlayers = Mathf.Clamp(maxPlayers, 2, 4)
            }.WithRelayNetwork();

            currentSession = await MultiplayerService.Instance.CreateSessionAsync(options);
            RtsNetworkCommandBus.GetOrCreate().Activate();

            PublishRelayMetadata(
                matchId,
                currentSession.Id,
                currentSession.Code,
                currentSession.Code,
                currentSession.Id,
                onSuccess,
                onError);
        }
        catch (Exception ex)
        {
            onError?.Invoke(ex.Message);
        }
    }

    public async void JoinRelaySessionByCode(
        string joinCode,
        Action onSuccess,
        Action<string> onError)
    {
        if (string.IsNullOrEmpty(joinCode))
        {
            onError?.Invoke("Join code vacio.");
            return;
        }

        try
        {
            RtsNetcodeRuntime.EnsureNetworkManager();
            await EnsureUnityServicesReadyAsync();
            currentSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(joinCode);
            RtsNetworkCommandBus.GetOrCreate().Activate();
            onSuccess?.Invoke();
        }
        catch (Exception ex)
        {
            onError?.Invoke(ex.Message);
        }
    }

    public void PublishRelayMetadata(
        string matchId,
        string lobbyId,
        string lobbyCode,
        string relayJoinCode,
        string sessionName,
        System.Action<MatchmakingClient.MatchResponse> onSuccess,
        System.Action<string> onError)
    {
        MatchmakingClient client = MatchmakingClient.GetOrCreate();
        MatchmakingClient.RelaySessionData data = new MatchmakingClient.RelaySessionData
        {
            lobbyId = lobbyId,
            lobbyCode = lobbyCode,
            relayJoinCode = relayJoinCode,
            sessionName = sessionName
        };

        client.PublishRelayData(matchId, data, onSuccess, onError);
    }

    public void StartHostWithRelay()
    {
        Debug.LogWarning("[MULTIPLAYER] Usa StartHostWithRelaySession con matchId y maxPlayers.");
    }

    public void StartClientWithRelay(string relayJoinCode)
    {
        JoinRelaySessionByCode(
            relayJoinCode,
            () => Debug.Log("[MULTIPLAYER] Cliente unido a sesion Relay/Lobby."),
            error => Debug.LogWarning("[MULTIPLAYER] Error uniendo cliente: " + error));
    }

    public async void LeaveCurrentSession()
    {
        ISession session = currentSession;
        currentSession = null;

        try
        {
            if (session != null)
            {
                await session.LeaveAsync();
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[MULTIPLAYER] Error saliendo de Relay/Lobby: " + ex.Message);
        }
        finally
        {
            NetworkManager networkManager = NetworkManager.Singleton;
            if (networkManager != null && networkManager.IsListening)
            {
                networkManager.Shutdown();
            }
        }
    }

    string GetAuthProfileName()
    {
        if (AuthSession.UserId > 0)
        {
            return "u_" + AuthSession.UserId;
        }

        return "default";
    }
}
