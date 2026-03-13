using UnityEngine;

public class GameManager_updated : MonoBehaviour
{
    public static GameManager_updated Instance { get; private set; }

    [Header("Timer")]
    [Tooltip("Set between 300-420 seconds for 5-7 minutes.")]
    [SerializeField] private float countdownSeconds = 360f;

    [Header("Balloon Sets")]
    [SerializeField] private BalloonSetController_updated balloonSet1;
    [SerializeField] private BalloonSetController_updated balloonSet2;

    [Header("Inventory")]
    [SerializeField] private Inventory_updated inventory;
    public Inventory_updated Inventory => inventory;

    [Header("Drawers")]
    [SerializeField] private DrawerLock_updated drawer1;
    [SerializeField] private DrawerLock_updated drawer2;
    [SerializeField] private DrawerLock_updated drawer3;

    [Header("Exit Door Root")]
    [SerializeField] private GameObject exitDoorRoot;

    [Header("UI")]
    [SerializeField] private UIManager_updated ui;
    public UIManager_updated UI => ui;

    private int _score;
    private float _timeLeft;
    private bool _running;
    private bool _levelComplete;
    private bool _drawer1Unlocked, _drawer2Unlocked;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

#if UNITY_2023_1_OR_NEWER
        inventory = FindFirstObjectByType<Inventory_updated>();
        ui = FindFirstObjectByType<UIManager_updated>();
#else
        // Older Unity compatibility
        inventory = FindObjectOfType<Inventory_updated>(true);
        ui = FindObjectOfType<UIManager_updated>(true);
#endif

    }

    private void OnEnable()
    {
        if (balloonSet1) balloonSet1.Completed += OnSet1Completed;
        if (inventory) inventory.OnChanged += EvaluateSigningState;
    }

    private void OnDisable()
    {
        if (balloonSet1) balloonSet1.Completed -= OnSet1Completed;
        if (inventory) inventory.OnChanged -= EvaluateSigningState;
    }

    private void Start()
    {
        ResetGame();
    }

    private void Update()
    {
        if (!_running || _levelComplete) return;

        _timeLeft -= Time.deltaTime;
        ui?.SetTimer(Mathf.Max(0f, _timeLeft));

        if (_timeLeft <= 0f)
        {
            _running = false;
            StopAllSets();
            ui?.ShowFail(true);
        }
    }

    public void StartGameplay()
    {
        ResetGame();

        _running = true;
        _timeLeft = countdownSeconds;

        ui?.ShowWelcome(false);
        ui?.ShowFail(false);
        ui?.ShowComplete(false);
        ui?.SetTimer(_timeLeft);
        ui?.SetScore(_score);

        balloonSet1?.BeginSet();
        balloonSet2?.StopSet(true);

        if (exitDoorRoot) exitDoorRoot.SetActive(true);
    }

    public void ResetGame()
    {
        _running = false;
        _levelComplete = false;

        _score = 0;
        _timeLeft = countdownSeconds;

        _drawer1Unlocked = _drawer2Unlocked = false;

        inventory?.ResetAll();

        drawer1?.ResetState();
        drawer2?.ResetState();
        drawer3?.ResetState();

        balloonSet1?.StopSet(true);
        balloonSet2?.StopSet(true);

        ui?.SetScore(_score);
        ui?.SetTimer(_timeLeft);
        ui?.SetSigningButtonVisible(false);
        ui?.ShowFail(false);
        ui?.ShowComplete(false);
    }

    public void ProcessBalloonPop(bool isCorrect)
    {
        if (!_running || _levelComplete) return;

        int delta = isCorrect ? 1 : -1;
        AddScore(delta);
    }

    private void AddScore(int delta)
    {
        _score += delta;
        ui?.SetScore(_score);

        if (_score >= 5 && !_drawer1Unlocked)
        {
            _drawer1Unlocked = true;
            drawer1?.UnlockAndOpen();
            ui?.ShowHint("First drawer unlocked! Collect the digital pen.");
        }

        if (_score >= 10 && !_drawer2Unlocked)
        {
            _drawer2Unlocked = true;
            drawer2?.UnlockAndOpen();
            ui?.ShowHint("Second drawer unlocked! Collect the bias awareness document.");
        }
    }

    private void OnSet1Completed()
    {
        balloonSet2?.BeginSet();
    }

    public void OnItemPicked(ItemType_updated item)
    {
        switch (item)
        {
            case ItemType_updated.Pen:
                ui?.ShowHint("Pen collected. Keep popping correct facts!");
                break;
            case ItemType_updated.Document:
                ui?.ShowHint("Document collected. If you have the pen too, tap Sign.");
                break;
            case ItemType_updated.Key:
                ui?.ShowHint("Key collected. Open the exit door!");
                break;
        }

        EvaluateSigningState();
    }

    private void EvaluateSigningState()
    {
        if (!ui || inventory == null) return;

        bool canSign = inventory.HasPen && inventory.HasDocument && !inventory.DocumentSigned;
        ui.SetSigningButtonVisible(canSign);
    }

    public void OnDocumentSigned()
    {
        if (_levelComplete) return;

        ui?.SetSigningButtonVisible(false);
        ui?.ShowHint("Document signed! Third drawer unlocked.");

        drawer3?.UnlockAndOpen();
    }

    public void OnDoorOpened()
    {
        if (_levelComplete) return;

        _levelComplete = true;
        _running = false;

        StopAllSets();
        ui?.ShowComplete(true);
        ui?.ShowHint("Escape successful!");
    }

    private void StopAllSets()
    {
        balloonSet1?.StopSet(true);
        balloonSet2?.StopSet(true);
    }
}
