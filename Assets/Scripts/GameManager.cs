using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
using TMPro;
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
    public static bool IsPlaying => CurrentState == GameState.Playing;

    private GameObject pauseCanvas;
    private GameObject deathCanvas;
    private CanvasGroup pauseCanvasGroup;
    private CanvasGroup deathCanvasGroup;
    private Button pauseContinueButton;
    private Button pauseRestartButton;
    private Button pauseMainMenuButton;
    private Button deathContinueButton;
    private Button deathMainMenuButton;

    private const float FADE_DURATION = 0.25f;
    private Coroutine activeFade;

    private void Awake()
    {
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
        _instance = this;

        CurrentState = GameState.Playing;
        CreatePauseMenu();
        CreateDeathMenu();
    }

    private void Start()
    {
        if (pauseCanvas != null) pauseCanvas.SetActive(false);
        if (deathCanvas != null) deathCanvas.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (CurrentState == GameState.Playing)
                Pause();
            else if (CurrentState == GameState.Paused)
                Resume();
        }
    }

    private void CreatePauseMenu()
    {
        pauseCanvas = new GameObject("PauseMenuCanvas");
        DontDestroyOnLoad(pauseCanvas);

        SetupCanvas(pauseCanvas, 90);
        pauseCanvasGroup = pauseCanvas.AddComponent<CanvasGroup>();

        Image bg = pauseCanvas.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.75f);
        bg.raycastTarget = true;

        RectTransform bgRt = pauseCanvas.GetComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.offsetMin = Vector2.zero;
        bgRt.offsetMax = Vector2.zero;

        GameObject container = CreateContainer(pauseCanvas.transform, "PauseBg", new Vector2(500f, 400f));

        CreateMenuLabel(container.transform, "PausedLabel", "PAUSED",
            80f, FontStyles.Bold, Color.white, new Vector2(500f, 100f));

        pauseContinueButton = CreateMenuButton(container.transform, "ContinueButton", "CONTINUE",
            new Color(0.2f, 0.6f, 0.2f, 1f), Color.white, 22f, Resume);

        pauseRestartButton = CreateMenuButton(container.transform, "RestartButton", "RESTART",
            new Color(0.15f, 0.15f, 0.15f, 0.95f), new Color(0.85f, 0.85f, 0.85f, 1f), 16f, Restart);

        pauseMainMenuButton = CreateMenuButton(container.transform, "MainMenuButton", "RETURN TO MAIN MENU",
            new Color(0.15f, 0.15f, 0.15f, 0.95f), new Color(0.85f, 0.85f, 0.85f, 1f), 16f, ReturnToMainMenu);

        EnsureEventSystem();
        ConfigureButtonNavigation(pauseContinueButton, pauseRestartButton, pauseMainMenuButton);
    }

    private void CreateDeathMenu()
    {
        deathCanvas = new GameObject("DeathMenuCanvas");
        DontDestroyOnLoad(deathCanvas);

        SetupCanvas(deathCanvas, 100);
        deathCanvasGroup = deathCanvas.AddComponent<CanvasGroup>();

        Image bg = deathCanvas.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.82f);
        bg.raycastTarget = true;

        RectTransform bgRt = deathCanvas.GetComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.offsetMin = Vector2.zero;
        bgRt.offsetMax = Vector2.zero;

        GameObject container = CreateContainer(deathCanvas.transform, "DeathBg", new Vector2(500f, 350f));

        CreateMenuLabel(container.transform, "YouDiedLabel", "YOU DIED",
            80f, FontStyles.Bold, new Color(0.85f, 0.08f, 0.08f, 1f), new Vector2(500f, 100f));

        deathContinueButton = CreateMenuButton(container.transform, "ContinueButton", "CONTINUE",
            new Color(0.78f, 0.07f, 0.07f, 1f), Color.white, 22f, DeathContinue);

        deathMainMenuButton = CreateMenuButton(container.transform, "MainMenuButton", "RETURN TO MAIN MENU",
            new Color(0.15f, 0.15f, 0.15f, 0.95f), new Color(0.85f, 0.85f, 0.85f, 1f), 16f, ReturnToMainMenu);

        EnsureEventSystem();
        ConfigureButtonNavigation(deathContinueButton, deathMainMenuButton, null);
    }

    private void SetupCanvas(GameObject canvasObj, int sortingOrder)
    {
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortingOrder;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();
    }

    private GameObject CreateContainer(Transform parent, string name, Vector2 size)
    {
        GameObject container = new GameObject(name, typeof(RectTransform));
        container.transform.SetParent(parent, false);
        RectTransform rt = container.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = size;
        rt.anchoredPosition = Vector2.zero;

        VerticalLayoutGroup vlg = container.AddComponent<VerticalLayoutGroup>();
        vlg.childAlignment = TextAnchor.MiddleCenter;
        vlg.spacing = 20f;
        vlg.childForceExpandWidth = false;
        vlg.childForceExpandHeight = false;
        ContentSizeFitter csf = container.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        return container;
    }

    private void CreateMenuLabel(Transform parent, string objName, string text,
        float fontSize, FontStyles fontStyle, Color color, Vector2 size)
    {
        GameObject go = new GameObject(objName, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = fontStyle;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        RectTransform titleRt = go.GetComponent<RectTransform>();
        titleRt.sizeDelta = size;
        LayoutElement le = go.AddComponent<LayoutElement>();
        le.preferredWidth = size.x;
        le.preferredHeight = size.y;
    }

    private Button CreateMenuButton(Transform parent, string objName, string text,
        Color bgColor, Color textColor, float fontSize, UnityEngine.Events.UnityAction callback)
    {
        GameObject go = new GameObject(objName, typeof(RectTransform));
        go.transform.SetParent(parent, false);

        Image img = go.AddComponent<Image>();
        img.color = bgColor;

        Button btn = go.AddComponent<Button>();
        RectTransform btnRt = go.GetComponent<RectTransform>();
        btnRt.sizeDelta = new Vector2(460f, 58f);

        LayoutElement le = go.AddComponent<LayoutElement>();
        le.preferredWidth = 460f;
        le.preferredHeight = 58f;

        GameObject txtGo = new GameObject("Text", typeof(RectTransform));
        txtGo.transform.SetParent(go.transform, false);
        RectTransform txtRt = txtGo.GetComponent<RectTransform>();
        txtRt.anchorMin = Vector2.zero;
        txtRt.anchorMax = Vector2.one;
        txtRt.offsetMin = Vector2.zero;
        txtRt.offsetMax = Vector2.zero;

        TextMeshProUGUI tmp = txtGo.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = FontStyles.Bold;
        tmp.color = textColor;
        tmp.alignment = TextAlignmentOptions.Center;

        ColorBlock colors = btn.colors;
        colors.highlightedColor = new Color(bgColor.r + 0.15f, bgColor.g + 0.15f, bgColor.b + 0.15f, 1f);
        colors.pressedColor = new Color(bgColor.r - 0.1f, bgColor.g - 0.1f, bgColor.b - 0.1f, 1f);
        colors.selectedColor = new Color(bgColor.r + 0.15f, bgColor.g + 0.15f, bgColor.b + 0.15f, 1f);
        colors.colorMultiplier = 1f;
        btn.colors = colors;

        btn.onClick.AddListener(PlayButtonClick);
        btn.onClick.AddListener(callback);

        return btn;
    }

    private void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>(FindObjectsInactive.Include) == null)
        {
            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<EventSystem>();
            esObj.AddComponent<InputSystemUIInputModule>();
        }
    }

    private void ConfigureButtonNavigation(params Button[] buttons)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null) continue;
            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.Explicit;
            if (i > 0) nav.selectOnUp = buttons[i - 1];
            if (i < buttons.Length - 1 && buttons[i + 1] != null) nav.selectOnDown = buttons[i + 1];
            buttons[i].navigation = nav;
        }
    }

    public void Pause()
    {
        if (CurrentState != GameState.Playing) return;
        CurrentState = GameState.Paused;
        Player.IsDead = false;
        Time.timeScale = 0f;
        AudioListener.pause = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (pauseCanvas != null)
        {
            pauseCanvas.SetActive(true);
            StartFade(pauseCanvasGroup, true);
        }

        if (pauseContinueButton != null)
            EventSystem.current?.SetSelectedGameObject(pauseContinueButton.gameObject);
    }

    public void Resume()
    {
        if (CurrentState != GameState.Paused) return;
        CurrentState = GameState.Playing;

        if (activeFade != null) StopCoroutine(activeFade);
        if (pauseCanvasGroup != null) pauseCanvasGroup.alpha = 0f;
        if (pauseCanvas != null) pauseCanvas.SetActive(false);

        Time.timeScale = 1f;
        AudioListener.pause = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        CurrentState = GameState.Playing;
        Player.IsDead = false;

        if (pauseCanvas != null) pauseCanvas.SetActive(false);
        if (deathCanvas != null) deathCanvas.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        CurrentState = GameState.Playing;
        Player.IsDead = false;

        if (pauseCanvas != null) pauseCanvas.SetActive(false);
        if (deathCanvas != null) deathCanvas.SetActive(false);

        SceneManager.LoadScene("MainMenu");
    }

    public void ShowDeathMenu()
    {
        CurrentState = GameState.Dead;
        Player.IsDead = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (deathCanvas != null)
        {
            deathCanvas.SetActive(true);
            StartFade(deathCanvasGroup, true);
        }

        if (deathContinueButton != null)
            EventSystem.current?.SetSelectedGameObject(deathContinueButton.gameObject);
    }

    public void DeathContinue()
    {
        CurrentState = GameState.Playing;
        Player.IsDead = false; // Resetea el estado estático del Player

        if (activeFade != null) StopCoroutine(activeFade);
        if (deathCanvasGroup != null) deathCanvasGroup.alpha = 0f;
        if (deathCanvas != null) deathCanvas.SetActive(false);

        Time.timeScale = 1f;
        AudioListener.pause = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Player player = FindFirstObjectByType<Player>();
        if (player == null) return;

        // Desactivamos el CharacterController un frame para mover al player sin interferencias
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // Teletransportar al Checkpoint
        player.transform.position = CheckpointManager.respawnPosition;
        player.transform.rotation = CheckpointManager.respawnRotation;

        if (cc != null) cc.enabled = true;

        // --- LLAMADA CRÍTICA AL HEALTH SYSTEM ---
        HealthSystem hs = player.GetComponent<HealthSystem>();
        if (hs != null)
        {
            hs.ResetDeath(); // <--- Aquí es donde se cura y revive
        }
    }

    private void PlayButtonClick()
    {
        AudioListener.pause = false;
        if (AudioManager.instance != null && AudioManager.instance.buttonClick != null)
            AudioManager.instance.PlaySFX(AudioManager.instance.buttonClick);
        if (CurrentState != GameState.Playing)
            AudioListener.pause = true;
    }

    private void StartFade(CanvasGroup canvasGroup, bool fadeIn)
    {
        if (activeFade != null) StopCoroutine(activeFade);
        activeFade = StartCoroutine(FadeCoroutine(canvasGroup, fadeIn));
    }

    private IEnumerator FadeCoroutine(CanvasGroup canvasGroup, bool fadeIn)
    {
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;

        canvasGroup.alpha = startAlpha;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        float elapsed = 0f;
        while (elapsed < FADE_DURATION)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / FADE_DURATION);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;

        if (fadeIn)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        activeFade = null;
    }
}
