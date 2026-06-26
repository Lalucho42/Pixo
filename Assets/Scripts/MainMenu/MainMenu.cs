using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string GameScene = "Boceto";
    [SerializeField] private GestorDeEscenas gestorDeEscenas;

    public void StartGame()
    {
        if (gestorDeEscenas != null)
        {
            gestorDeEscenas.CargarJuego(GameScene);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(GameScene);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}