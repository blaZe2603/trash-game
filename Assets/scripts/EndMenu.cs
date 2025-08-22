using UnityEngine;
using UnityEngine.SceneManagement;

public class EndMenu : MonoBehaviour
{


    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(2);
    }

    public void MainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }
    public void StopGame()
    {
        Application.Quit();
    }

}
