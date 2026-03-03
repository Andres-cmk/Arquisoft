using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuController : MonoBehaviour
{
    public void OnLogoutClicked()
    {
        SessionManager.Logout();
        SceneManager.LoadScene("MainMenuScene");
    }
}
