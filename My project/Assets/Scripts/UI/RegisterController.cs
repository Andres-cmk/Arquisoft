using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RegisterController : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_InputField confirmPasswordInput;

    [Header("UI")]
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Button registerButton;
    [SerializeField] private float successDelay = 1.2f;
    [SerializeField] private bool goToMainMenuOnSuccess = false; // false -> Login, true -> MainMenu

    private IAuthService _auth;
    private bool _isSubmitting;

    private void Awake()
    {
        _auth = AuthServiceProvider.Instance;
    }

    public async void OnRegisterClicked()
    {
        if (_isSubmitting)
            return;

        string username = usernameInput.text.Trim();
        string password = passwordInput.text;
        string confirm = confirmPasswordInput.text;

        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(confirm))
        {
            SetStatus("Todos los campos son obligatorios.");
            return;
        }

        if (password != confirm)
        {
            SetStatus("La contraseña y la confirmación no coinciden.");
            return;
        }

        _isSubmitting = true;
        SetSubmitInteractable(false);

        bool restoreSubmitState = true;
        try
        {
            AuthResult result = await _auth.RegisterAsync(username, password);
            SetStatus(result.Message);

            if (!result.Success)
                return;

            restoreSubmitState = false;
            StartCoroutine(RedirectAfterSuccess());
        }
        catch (Exception ex)
        {
            SetStatus("No se pudo completar el registro.");
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

    public void GoToLogin()
    {
        SceneManager.LoadScene("LoginScene");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    private IEnumerator RedirectAfterSuccess()
    {
        yield return new WaitForSeconds(successDelay);

        if (goToMainMenuOnSuccess)
            SceneManager.LoadScene("MainMenuScene");
        else
            SceneManager.LoadScene("LoginScene");
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }

    private void SetSubmitInteractable(bool interactable)
    {
        if (registerButton != null)
            registerButton.interactable = interactable;
    }
}
