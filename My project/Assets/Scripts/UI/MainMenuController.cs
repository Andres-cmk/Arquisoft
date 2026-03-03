using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private float redirectDelay = 1.5f;

    public void GoToRegister()
    {
        SceneManager.LoadScene("RegisterScene");
    }

    public void GoToLogin()
    {
        SceneManager.LoadScene("LoginScene");
    }

    public void PlayGame()
    {
        if (SessionManager.IsLoggedIn)
        {
            SceneManager.LoadScene("GameScene");
            return;
        }

        if (statusText != null)
            statusText.text = "Debes iniciar sesión primero.";

        StartCoroutine(RedirectToLogin());
    }

    private IEnumerator RedirectToLogin()
    {
        yield return new WaitForSeconds(redirectDelay);
        SceneManager.LoadScene("LoginScene");
    }
}
