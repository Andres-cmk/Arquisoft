using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;

    [Header("UI")]
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Button loginButton;

    private IAuthService _auth;
    private bool _isSubmitting;

    private void Awake()
    {
        _auth = AuthServiceProvider.Instance;
    }

    public async void OnLoginClicked()
    {
        if (_isSubmitting)
            return;

        string username = usernameInput.text.Trim();
        string password = passwordInput.text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            SetStatus("Usuario y contraseña son obligatorios.");
            return;
        }

        _isSubmitting = true;
        SetSubmitInteractable(false);

        bool restoreSubmitState = true;
        try
        {
            AuthResult result = await _auth.LoginAsync(username, password);
            SetStatus(result.Message);

            if (!result.Success)
                return;

            if (string.IsNullOrWhiteSpace(result.AccessToken))
            {
                SetStatus("Login invalido: access token faltante.");
                return;
            }

            SessionManager.SetSession(result.User, result.AccessToken);
            restoreSubmitState = false;
            SceneManager.LoadScene("MainMenuScene");
        }
        catch (Exception ex)
        {
            SetStatus("No se pudo completar el login.");
            Debug.LogException(ex);
        }
        finally
        {
            if (restoreSubmitState)
            {
                _isSubmitting = false;
                SetSubmitInteractable(true);
            }
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }

    private void SetSubmitInteractable(bool interactable)
    {
        if (loginButton != null)
            loginButton.interactable = interactable;
    }
}
