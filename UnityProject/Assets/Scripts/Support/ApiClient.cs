using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ApiClient : MonoBehaviour
{
    [Header("Support API")]
    [SerializeField] string baseUrl = "http://127.0.0.1:8000";
    [SerializeField] string loginEndpoint = "/auth/login";
    [SerializeField] string sessionSummaryEndpoint = "/support/session-summary";

    public static ApiClient Instance { get; private set; }
    public bool IsAuthenticated => AuthSession.IsAuthenticated;
    public int UserId => AuthSession.UserId;
    public string Username => AuthSession.Username;
    public string AccessToken => AuthSession.AccessToken;

    bool isLoggedIn;

    [Serializable]
    class LoginRequest
    {
        public string username;
        public string password;
    }

    [Serializable]
    public class LoginResponse
    {
        public string message;
        public int user_id;
        public string username;
        public string access_token;
        public string token_type;
    }

    public static ApiClient GetOrCreate()
    {
        if (Instance != null)
        {
            return Instance;
        }

        Instance = FindFirstObjectByType<ApiClient>();
        if (Instance != null)
        {
            return Instance;
        }

        GameObject go = new GameObject("ApiClient");
        Instance = go.AddComponent<ApiClient>();
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

    public void Login(string username, string password, Action<LoginResponse> onSuccess, Action<string> onError)
    {
        StartCoroutine(LoginCoroutine(username, password, onSuccess, onError));
    }

    public void SendSessionSummary(GameSessionStats.SessionSummaryPayload payload, Action<string> onSuccess, Action<string> onError)
    {
        if (payload == null)
        {
            onError?.Invoke("Payload de sesion vacio.");
            return;
        }

        if (!IsAuthenticated)
        {
            onError?.Invoke("No autenticado.");
            return;
        }

        StartCoroutine(PostJsonCoroutine(sessionSummaryEndpoint, payload,
            onSuccess: text =>
            {
                string message = "Sesion enviada.";
                if (!string.IsNullOrEmpty(text))
                {
                    message = text;
                }

                onSuccess?.Invoke(message);
            },
            onError: err =>
            {
                onError?.Invoke(err);
            }));
    }

    public IEnumerator LoginCoroutine(string username, string password, Action<LoginResponse> onSuccess, Action<string> onError)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            onError?.Invoke("Usuario o clave vacios.");
            yield break;
        }

        LoginRequest payload = new LoginRequest { username = username, password = password };
        LoginResponse response = null;
        string requestError = null;

        yield return PostJsonCoroutine(loginEndpoint, payload,
            onSuccess: text =>
            {
                response = JsonUtility.FromJson<LoginResponse>(text);
            },
            onError: err =>
            {
                requestError = err;
            });

        if (!string.IsNullOrEmpty(requestError))
        {
            onError?.Invoke(requestError);
            yield break;
        }

        if (response == null || string.IsNullOrEmpty(response.message))
            response = new LoginResponse { message = "Login successful" };

        if (string.IsNullOrEmpty(response.access_token))
        {
            isLoggedIn = false;
            onError?.Invoke("Login sin token de acceso.");
            yield break;
        }

        isLoggedIn = true;
        AuthSession.SetAuthenticated(response.user_id, response.username, response.access_token);

        onSuccess?.Invoke(response);
    }

    public void Logout()
    {
        isLoggedIn = false;
        AuthSession.Clear();
    }

    string BuildUrl(string endpoint)
    {
        string cleanBase = baseUrl.TrimEnd('/');
        string cleanEndpoint = endpoint.StartsWith("/") ? endpoint : "/" + endpoint;
        return cleanBase + cleanEndpoint;
    }

    IEnumerator PostJsonCoroutine<T>(string endpoint, T payload, Action<string> onSuccess, Action<string> onError)
    {
        string json = JsonUtility.ToJson(payload);
        UnityWebRequest request = new UnityWebRequest(BuildUrl(endpoint), UnityWebRequest.kHttpVerbPOST);
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        AuthSession.ApplyAuthorization(request);
        request.timeout = 10;

        yield return request.SendWebRequest();

        bool hasError = request.result != UnityWebRequest.Result.Success || request.responseCode >= 400;
        if (hasError)
        {
            string backendMessage = request.downloadHandler != null ? request.downloadHandler.text : string.Empty;
            if (!string.IsNullOrEmpty(backendMessage))
            {
                onError?.Invoke(backendMessage);
            }
            else
            {
                onError?.Invoke(string.IsNullOrEmpty(request.error) ? "Request failed." : request.error);
            }

            request.Dispose();
            yield break;
        }

        string responseText = request.downloadHandler != null ? request.downloadHandler.text : string.Empty;
        onSuccess?.Invoke(responseText);
        request.Dispose();
    }
}
