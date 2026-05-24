using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Names")]
    public string gameWorldSceneName =
        "GameWorld";

    public void EnterWorld()
    {
        SceneManager.LoadScene(
            gameWorldSceneName
        );
    }

    public void QuitGame()
    {
        Debug.Log(
            "Quit Game"
        );

        Application.Quit();
    }
}