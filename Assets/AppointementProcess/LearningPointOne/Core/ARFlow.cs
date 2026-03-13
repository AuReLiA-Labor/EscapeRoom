using UnityEngine;
using UnityEngine.UI;

public class ARFlow : MonoBehaviour
{
    [Header("Top-level canvases / roots")]
    [SerializeField] private GameObject welcomeCanvas;
    [SerializeField] private GameObject howToScanCanvas;
    [SerializeField] private GameObject scanningCanvas;   // overlay while scanning
    [SerializeField] private GameObject inGameCanvas;

    [Header("In-game panels (children of inGameCanvas)")]
    [SerializeField] private GameObject learningPointWelcomePanel;
    [SerializeField] private GameObject timeScorePanel;
    [SerializeField] private GameObject failPanel;
    [SerializeField] private GameObject successPanel;

    [Header("AR content")]
    [SerializeField] private GameObject arContentRoot;
    [SerializeField] private ARContentPlacer contentPlacer;

    [Header("Buttons (optional)")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button scanButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button restartButtonsFail;
    [SerializeField] private Button restartButtonsSuccess;
    [SerializeField] private Button closeFailButton;
    [SerializeField] private Button closeSuccessButton;

    [Header("Behaviour")]
    [Tooltip("If ON, tapping Scan jumps straight to gameplay (not typical).")]
    public bool autoEnterGameOnScan = false;

    [Tooltip("Disable UI raycasts on the scanning overlay so taps reach AR.")]
    [SerializeField] private bool passThroughTouchesOnScanning = true; // MOD

    // MOD: cache raycast blockers we’ll toggle
    private GraphicRaycaster _scanRaycaster;
    private CanvasGroup _scanCanvasGroup;

    void Awake()
    {
        if (startButton) startButton.onClick.AddListener(OnStartButtonClicked);
        if (scanButton)  scanButton.onClick.AddListener(OnScanButtonClicked);
        if (playButton)  playButton.onClick.AddListener(OnPlayButtonClicked);

        if (restartButtonsFail)    restartButtonsFail.onClick.AddListener(OnRestartButtonClicked);
        if (restartButtonsSuccess) restartButtonsSuccess.onClick.AddListener(OnRestartButtonClicked);
        if (closeFailButton)       closeFailButton.onClick.AddListener(ShowWelcome);
        if (closeSuccessButton)    closeSuccessButton.onClick.AddListener(ShowWelcome);

        if (contentPlacer) contentPlacer.Placed += OnContentPlaced;

        // MOD: cache components for pass-through
        if (scanningCanvas)
        {
            _scanRaycaster   = scanningCanvas.GetComponent<GraphicRaycaster>();
            _scanCanvasGroup = scanningCanvas.GetComponent<CanvasGroup>();
        }
    }

    void OnDestroy()
    {
        if (contentPlacer) contentPlacer.Placed -= OnContentPlaced;
    }

    void Start() => ShowWelcome();

    // ====== Public helpers ======

    public void ShowInGamePanel()
    {
        ToggleAll(welcome:false, howTo:false, scanning:false, inGame:true);
        SetActiveSafe(learningPointWelcomePanel, true);
        SetActiveSafe(timeScorePanel, false);
    }

    public void ShowWelcome()
    {
        ToggleAll(welcome:true, howTo:false, scanning:false, inGame:false);
        if (GameManager.Instance) GameManager.Instance.ResetGame();
    }

    // ====== Button handlers ======

    private void OnStartButtonClicked()
    {
        ToggleAll(welcome:false, howTo:true, scanning:false, inGame:false);
    }

    private void OnScanButtonClicked()
    {
        if (autoEnterGameOnScan)
        {
            ShowInGamePanel();
            GameManager.Instance?.StartGameplay();
            return;
        }

        // Enter scanning: enable AR systems & scanning overlay
        ToggleAll(welcome:false, howTo:false, scanning:true, inGame:false);
        SetActiveSafe(arContentRoot, true);
        // MOD: allow taps to reach AR while scanning
        SetScanningPassThrough(passThroughTouchesOnScanning);
    }

    private void OnPlayButtonClicked()
    {
        SetActiveSafe(learningPointWelcomePanel, false);
        SetActiveSafe(timeScorePanel, true);
        GameManager.Instance?.StartGameplay();
    }

    private void OnRestartButtonClicked() => ShowWelcome();

    private void OnContentPlaced()
    {
        // Once placed, hide scanning overlay; gameplay UI proceeds next
        SetActiveSafe(scanningCanvas, false);
        // MOD: restore overlay’s raycast behavior
        SetScanningPassThrough(false);
    }

    // ====== Utils ======

    private void ToggleAll(bool welcome, bool howTo, bool scanning, bool inGame)
    {
        SetActiveSafe(welcomeCanvas, welcome);
        SetActiveSafe(howToScanCanvas, howTo);
        SetActiveSafe(scanningCanvas, scanning);
        SetActiveSafe(inGameCanvas, inGame);

        // Keep the AR content visible once placed.
        if (arContentRoot)
        {
            bool shouldBeActive = scanning || inGame;
            if (arContentRoot.activeSelf != shouldBeActive)
                arContentRoot.SetActive(shouldBeActive);
        }
    }


    private static void SetActiveSafe(GameObject go, bool active)
    {
        if (go && go.activeSelf != active) go.SetActive(active);
    }

    // MOD: central place to allow/deny UI blocking on the scanning overlay
    private void SetScanningPassThrough(bool passThrough)
    {
        if (_scanCanvasGroup != null)
            _scanCanvasGroup.blocksRaycasts = !passThrough;

        if (_scanRaycaster != null)
            _scanRaycaster.enabled = !passThrough;
    }
}
