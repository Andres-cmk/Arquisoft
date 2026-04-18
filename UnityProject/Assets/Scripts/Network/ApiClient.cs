using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ApiClient : MonoBehaviour
{
    [Header("FastAPI")]
    [SerializeField] string baseUrl = "http://127.0.0.1:8000";
    [SerializeField] string loginEndpoint = "/auth/login";

    public static ApiClient Instance { get; private set; }
    public bool IsAuthenticated => isLoggedIn;

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

    public IEnumerator LoginCoroutine(string username, string password, Action<LoginResponse> onSuccess, Action<string> onError)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            onError?.Invoke("Usuario o clave vacios.");
            yield break;
        }

        LoginRequest payload = new LoginRequest { username = username, password = password };
        string json = JsonUtility.ToJson(payload);
        UnityWebRequest request = new UnityWebRequest(BuildUrl(loginEndpoint), UnityWebRequest.kHttpVerbPOST);
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.timeout = 10;

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success || request.responseCode >= 400)
        {
            string backendMessage = request.downloadHandler != null ? request.downloadHandler.text : string.Empty;
            if (!string.IsNullOrEmpty(backendMessage))
            {
                onError?.Invoke(backendMessage);
            }
            else
            {
                onError?.Invoke(string.IsNullOrEmpty(request.error) ? "No se pudo iniciar sesion." : request.error);
            }

            request.Dispose();
            yield break;
        }

        LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
        if (response == null || string.IsNullOrEmpty(response.message))
            response = new LoginResponse { message = "Login successful" };

        isLoggedIn = true;

        onSuccess?.Invoke(response);
        request.Dispose();
    }

    public void Logout()
    {
        isLoggedIn = false;
    }

    string BuildUrl(string endpoint)
    {
        string cleanBase = baseUrl.TrimEnd('/');
        string cleanEndpoint = endpoint.StartsWith("/") ? endpoint : "/" + endpoint;
        return cleanBase + cleanEndpoint;
    }
}
