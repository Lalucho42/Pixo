using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string GameScene;


    public void StartGame() {

        SceneManager.LoadScene(GameScene);
    
    }


    public void QuitGame()
    {
        Application.Quit();
    }


}
