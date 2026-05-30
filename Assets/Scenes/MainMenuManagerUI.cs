using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Names")]
    public string gameWorldSceneName =
        "GameWorld";

    [Header("UI")]
    public GameObject lobbyPanel;

    // =====================================
    // ENTER WORLD
    // Shows the host/join lobby panel
    // =====================================

    public void EnterWorld()
    {
        if (NetworkRelay.instance != null)
        {
            NetworkRelay.instance.EnterWorld();
        }
    }

    // =====================================
    // QUIT
    // =====================================

    public void QuitGame()
    {
        Application.Quit();
    }
}