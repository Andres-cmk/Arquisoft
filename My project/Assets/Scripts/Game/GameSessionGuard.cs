using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameSessionGuard : MonoBehaviour
{
    [SerializeField] private string loginSceneName = "LoginScene";
    [SerializeField] private bool validateTokenWithApi = true;
    [SerializeField] private int validationTimeoutSeconds = 10;

    private async void Start()
    {
        if (!SessionManager.IsLoggedIn)
        {
            RedirectToLogin();
            return;
        }

        if (!validateTokenWithApi || !AuthServiceProvider.IsApiEnabled)
            return;

        bool tokenValid = await ValidateTokenAsync(SessionManager.AccessToken);
        if (tokenValid)
            return;

        SessionManager.Logout();
        RedirectToLogin();
    }

    private async Task<bool> ValidateTokenAsync(string accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
            return false;

        string baseUrl = AuthServiceProvider.ApiBaseUrl;
        if (string.IsNullOrWhiteSpace(baseUrl))
            return false;

        string url = baseUrl.TrimEnd('/') + "/auth/me";
        using (var request = UnityWebRequest.Get(url))
        {
            request.timeout = validationTimeoutSeconds;
            request.SetRequestHeader("Authorization", "Bearer " + accessToken);

            var op = request.SendWebRequest();
            while (!op.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
                return false;

            return request.responseCode >= 200 && request.responseCode < 300;
        }
    }

    private void RedirectToLogin()
    {
        if (SceneManager.GetActiveScene().name == loginSceneName)
            return;

        SceneManager.LoadScene(loginSceneName);
    }
}
