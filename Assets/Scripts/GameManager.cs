using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public enum GameState { Playing, Paused, Dead }

    public static GameManager instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<GameManager>(FindObjectsInactive.Include);
            return _instance;
        }
    }
    private static GameManager _instance;

    public static GameState CurrentState { get; private set; } = GameState.Playing;
    public static bool IsPaused => CurrentState == GameState.Paused;
    public static bool IsDead => CurrentState == GameState.Dead;

    [Header("UI Referencias - Menu de Pausa")]
    public GameObject pauseCanvas;
    public CanvasGroup pauseCanvasGroup;
    public Button pauseContinueButton;
    public Button pauseRestartButton;
    public Button pauseMainMenuButton;

    [Header("UI Referencias - Menu de Muerte")]
    public GameObject deathCanvas;
    public CanvasGroup deathCanvasGroup;
    public Button deathContinueButton;
    public Button deathMainMenuButton;

    private const float FADE_DURATION = 0.25f;
    private Coroutine activeFade;

    private void Awake()
    {
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
        _instance = this;
        CurrentState = GameState.Playing;
    }

    private void Start()
    {
        if (pauseCanvas != null) pauseCanvas.SetActive(false);
        if (deathCanvas != null) deathCanvas.SetActive(false);

        SetupButtonListeners();
    }

    private void SetupButtonListeners()
    {
        if (pauseContinueButton != null) pauseContinueButton.onClick.AddListener(Resume);
        if (pauseRestartButton != null) pauseRestartButton.onClick.AddListener(Restart);
        if (pauseMainMenuButton != null) pauseMainMenuButton.onClick.AddListener(ReturnToMainMenu);

        if (deathContinueButton != null) deathContinueButton.onClick.AddListener(DeathContinue);
        if (deathMainMenuButton != null) deathMainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (CurrentState == GameState.Playing) Pause();
            else if (CurrentState == GameState.Paused) Resume();
        }
    }

    public void Pause()
    {
        if (CurrentState != GameState.Playing) return;
        CurrentState = GameState.Paused;
        Time.timeScale = 0f;
        AudioListener.pause = true;
        SetCursorState(true);

        if (pauseCanvas != null)
        {
            pauseCanvas.SetActive(true);
            StartFade(pauseCanvasGroup, true);
            EventSystem.current?.SetSelectedGameObject(pauseContinueButton.gameObject);
        }
    }

    public void Resume()
    {
        if (CurrentState != GameState.Paused) return;
        CurrentState = GameState.Playing;

        Time.timeScale = 1f;
        AudioListener.pause = false;
        SetCursorState(false);

        if (pauseCanvas != null)
        {
            StartFade(pauseCanvasGroup, false, () => pauseCanvas.SetActive(false));
        }
    }

    public void ShowDeathMenu()
    {
        CurrentState = GameState.Dead;
        Time.timeScale = 0f;
        AudioListener.pause = true;
        SetCursorState(true);

        if (deathCanvas != null)
        {
            deathCanvas.SetActive(true);
            StartFade(deathCanvasGroup, true);
            EventSystem.current?.SetSelectedGameObject(deathContinueButton.gameObject);
        }
    }

    public void DeathContinue()
    {
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SetCursorState(false);

        if (deathCanvas != null) deathCanvas.SetActive(false);

        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            player.transform.position = CheckpointManager.respawnPosition;
            player.transform.rotation = CheckpointManager.respawnRotation;
            if (cc != null) cc.enabled = true;

            HealthSystem hs = player.GetComponent<HealthSystem>();
            if (hs != null) hs.ResetDeath();
        }
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void SetCursorState(bool visible)
    {
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = visible;
    }

    private void StartFade(CanvasGroup cg, bool fadeIn, System.Action onComplete = null)
    {
        if (cg == null) { onComplete?.Invoke(); return; }
        if (activeFade != null) StopCoroutine(activeFade);
        activeFade = StartCoroutine(FadeCoroutine(cg, fadeIn, onComplete));
    }

    private IEnumerator FadeCoroutine(CanvasGroup cg, bool fadeIn, System.Action onComplete)
    {
        float start = cg.alpha;
        float end = fadeIn ? 1f : 0f;
        float elapsed = 0f;

        while (elapsed < FADE_DURATION)
        {
            elapsed += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsed / FADE_DURATION);
            yield return null;
        }

        cg.alpha = end;
        activeFade = null;
        onComplete?.Invoke();
    }
}