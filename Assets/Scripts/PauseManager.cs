using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public static bool isPaused = false;

    private void Awake()
    {
        if (pauseMenuUI == null) pauseMenuUI = GameObject.Find("PauseMenuCanvas");
    }

    private void Start()
    {
        if (pauseMenuUI != null)
        {
            Button resumeBtn = pauseMenuUI.transform.Find("PauseBackground/ResumeButton")?.GetComponent<Button>();
            if (resumeBtn != null && resumeBtn.onClick.GetPersistentEventCount() == 0) resumeBtn.onClick.AddListener(Resume);

            Button exitBtn = pauseMenuUI.transform.Find("PauseBackground/ExitButton")?.GetComponent<Button>();
            if (exitBtn != null && exitBtn.onClick.GetPersistentEventCount() == 0) exitBtn.onClick.AddListener(QuitGame);
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Pause()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
