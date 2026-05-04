using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TMP_InputField usernameInput;
    [SerializeField] TMP_InputField passwordInput;
    [SerializeField] Button loginButton;
    [SerializeField] Button backButton;
    [SerializeField] TMP_Text statusText;

    void Awake()
    {
        if (loginButton != null)
        {
            loginButton.onClick.AddListener(OnLoginClicked);
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackClicked);
        }

        if (usernameInput != null)
        {
            usernameInput.gameObject.SetActive(false);
        }

        if (passwordInput != null)
        {
            passwordInput.gameObject.SetActive(false);
        }

        SetStatus(string.Empty);
    }

    void OnDestroy()
    {
        if (loginButton != null)
        {
            loginButton.onClick.RemoveListener(OnLoginClicked);
        }

        if (backButton != null)
        {
            backButton.onClick.RemoveListener(OnBackClicked);
        }
    }

    public void OnLoginClicked()
    {
        ApiClient apiClient = ApiClient.GetOrCreate();
        if (apiClient == null)
        {
            SetStatus("No hay ApiClient en escena.");
            return;
        }

        SetStatus("Abriendo login web...");

        apiClient.LoginWithBrowser(
            OnLoginSuccess,
            OnLoginError
        );
    }

    public void OnBackClicked()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int previousIndex = currentIndex - 1;

        if (previousIndex >= 0)
        {
            SceneManager.LoadScene(previousIndex);
            return;
        }

        SetStatus("No hay una escena anterior.");
    }

    void OnLoginSuccess(ApiClient.LoginResponse response)
    {
        string message = "Login correcto";

        if (!string.IsNullOrEmpty(response?.message))
        {
            message = response.message;
        }

        if (!string.IsNullOrEmpty(response?.username))
        {
            message += " (" + response.username + ")";
        }

        SetStatus(message);
    }

    void OnLoginError(string error)
    {
        SetStatus(string.IsNullOrEmpty(error) ? "Error en login" : error);
    }

    void SetStatus(string text)
    {
        if (statusText != null)
        {
            statusText.text = text;
        }
    }
}
