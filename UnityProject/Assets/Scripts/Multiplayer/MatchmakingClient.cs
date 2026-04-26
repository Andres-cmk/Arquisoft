using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class MatchmakingClient : MonoBehaviour
{
    [Header("Matchmaking API")]
    [SerializeField] string baseUrl = "http://127.0.0.1:8001";

    public static MatchmakingClient Instance { get; private set; }

    [Serializable]
    public class JoinQueueRequest
    {
        public int minPlayers = 2;
        public int maxPlayers = 2;
        public string gameMode = "standard";
        public string region = "default";
    }

    [Serializable]
    public class ReadyRequest
    {
        public bool ready = true;
    }

    [Serializable]
    public class RelaySessionData
    {
        public string lobbyId;
        public string lobbyCode;
        public string relayJoinCode;
        public string sessionName;
    }

    [Serializable]
    public class MatchPlayer
    {
        public int userId;
        public string username;
        public string role;
        public bool ready;
    }

    [Serializable]
    public class MatchResponse
    {
        public string matchId;
        public string status;
        public int minPlayers;
        public int maxPlayers;
        public string gameMode;
        public string region;
        public int hostUserId;
        public MatchPlayer[] players;
        public RelaySessionData relay;
        public string createdAtUtc;
        public string updatedAtUtc;
        public string role;
    }

    public static MatchmakingClient GetOrCreate()
    {
        if (Instance != null)
        {
            return Instance;
        }

        Instance = FindFirstObjectByType<MatchmakingClient>();
        if (Instance != null)
        {
            return Instance;
        }

        GameObject go = new GameObject("MatchmakingClient");
        Instance = go.AddComponent<MatchmakingClient>();
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

    public void JoinQueue(Action<MatchResponse> onSuccess, Action<string> onError, int minPlayers = 2, int maxPlayers = 2)
    {
        JoinQueueRequest request = new JoinQueueRequest
        {
            minPlayers = minPlayers,
            maxPlayers = maxPlayers
        };

        StartCoroutine(PostJsonCoroutine("/matchmaking/queue/join", request, onSuccess, onError));
    }

    public void GetMatch(string matchId, Action<MatchResponse> onSuccess, Action<string> onError)
    {
        if (string.IsNullOrEmpty(matchId))
        {
            onError?.Invoke("Match id vacio.");
            return;
        }

        StartCoroutine(GetJsonCoroutine("/matchmaking/matches/" + UnityWebRequest.EscapeURL(matchId), onSuccess, onError));
    }

    public void PublishRelayData(string matchId, RelaySessionData relayData, Action<MatchResponse> onSuccess, Action<string> onError)
    {
        if (string.IsNullOrEmpty(matchId))
        {
            onError?.Invoke("Match id vacio.");
            return;
        }

        if (relayData == null)
        {
            onError?.Invoke("Relay data vacio.");
            return;
        }

        StartCoroutine(PostJsonCoroutine("/matchmaking/matches/" + UnityWebRequest.EscapeURL(matchId) + "/relay", relayData, onSuccess, onError));
    }

    public void SetReady(string matchId, bool ready, Action<MatchResponse> onSuccess, Action<string> onError)
    {
        if (string.IsNullOrEmpty(matchId))
        {
            onError?.Invoke("Match id vacio.");
            return;
        }

        ReadyRequest request = new ReadyRequest { ready = ready };
        StartCoroutine(PostJsonCoroutine("/matchmaking/matches/" + UnityWebRequest.EscapeURL(matchId) + "/ready", request, onSuccess, onError));
    }

    public void LeaveMatch(string matchId, Action<MatchResponse> onSuccess, Action<string> onError)
    {
        if (string.IsNullOrEmpty(matchId))
        {
            onError?.Invoke("Match id vacio.");
            return;
        }

        StartCoroutine(PostJsonCoroutine("/matchmaking/matches/" + UnityWebRequest.EscapeURL(matchId) + "/leave", new EmptyPayload(), onSuccess, onError));
    }

    string BuildUrl(string endpoint)
    {
        string cleanBase = baseUrl.TrimEnd('/');
        string cleanEndpoint = endpoint.StartsWith("/") ? endpoint : "/" + endpoint;
        return cleanBase + cleanEndpoint;
    }

    IEnumerator GetJsonCoroutine(string endpoint, Action<MatchResponse> onSuccess, Action<string> onError)
    {
        UnityWebRequest request = UnityWebRequest.Get(BuildUrl(endpoint));
        AuthSession.ApplyAuthorization(request);
        request.timeout = 10;

        yield return request.SendWebRequest();
        HandleJsonResponse(request, onSuccess, onError);
    }

    IEnumerator PostJsonCoroutine<T>(string endpoint, T payload, Action<MatchResponse> onSuccess, Action<string> onError)
    {
        string json = JsonUtility.ToJson(payload);
        UnityWebRequest request = new UnityWebRequest(BuildUrl(endpoint), UnityWebRequest.kHttpVerbPOST);
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        AuthSession.ApplyAuthorization(request);
        request.timeout = 10;

        yield return request.SendWebRequest();
        HandleJsonResponse(request, onSuccess, onError);
    }

    void HandleJsonResponse(UnityWebRequest request, Action<MatchResponse> onSuccess, Action<string> onError)
    {
        bool hasError = request.result != UnityWebRequest.Result.Success || request.responseCode >= 400;
        string responseText = request.downloadHandler != null ? request.downloadHandler.text : string.Empty;

        if (hasError)
        {
            onError?.Invoke(string.IsNullOrEmpty(responseText) ? request.error : responseText);
            request.Dispose();
            return;
        }

        MatchResponse response = JsonUtility.FromJson<MatchResponse>(responseText);
        onSuccess?.Invoke(response);
        request.Dispose();
    }

    [Serializable]
    class EmptyPayload
    {
    }
}
