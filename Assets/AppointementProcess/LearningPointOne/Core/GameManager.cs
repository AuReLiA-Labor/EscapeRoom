using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Balloon sets")]
    [SerializeField] private BalloonSetController balloonSet1;
    [SerializeField] private BalloonSetController balloonSet2;

    [Header("Inventory")]
    public Inventory inventory; // <- keep public so other scripts can check/use it

    [Header("Drawers & items")]
    [SerializeField] private DrawerLock drawer1; // opens at 5 points (Pen)
    [SerializeField] private DrawerLock drawer2; // opens at 10 points (Document)
    [SerializeField] private DrawerLock drawer3; // opens after signing (Key)

    [Header("Exit")]
    [SerializeField] private GameObject exitDoor; // keep inactive until key acquired

    [Header("UI Bridge")]
    public UIManager ui; // <- made PUBLIC (fixes CS0122). Ensure this is assigned in Inspector.
    [SerializeField] private GameObject failPanel;
    [SerializeField] private GameObject successPanel;
    [SerializeField] private GameObject timeScorePanel;

    [Header("Timer")]
    [SerializeField] private float countdownSeconds = 120f;

    int _score;
    float _timeLeft;
    bool _running;
    bool _levelComplete;

    bool _drawer1Unlocked;
    bool _drawer2Unlocked;

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (balloonSet1) balloonSet1.Completed += OnSet1Completed;
        if (!inventory) inventory = FindObjectOfType<Inventory>();
    }

    void OnDestroy()
    {
        if (balloonSet1) balloonSet1.Completed -= OnSet1Completed;
        if (Instance == this) Instance = null;
    }

    void Update()
    {
        if (!_running || _levelComplete) return;

        _timeLeft -= Time.deltaTime;
        if (ui) ui.SetTimer(Mathf.Max(0f, _timeLeft));

        if (_timeLeft <= 0f)
        {
            _running = false;
            StopAllSets();
            ShowFail();
        }
    }

    // ===== Flow =====

    public void StartGameplay()
    {
        if (timeScorePanel) timeScorePanel.SetActive(true);

        _levelComplete = false;
        _running = true;
        _score = 0;
        _timeLeft = countdownSeconds;

        if (ui)
        {
            ui.SetScore(_score);
            ui.SetTimer(_timeLeft);
            // Update signing button visibility at start (should be off)
            ui.SetSigningButtonVisible(false);
            EvaluateSigningState();
        }

        _drawer1Unlocked = _drawer2Unlocked = false;

        if (balloonSet1) balloonSet1.BeginSet();
        if (balloonSet2) balloonSet2.StopSet(true);
    }

    public void ResetGame()
    {
        inventory?.ResetAll();
        _levelComplete = false;
        _running = false;
        _score = 0;
        _timeLeft = countdownSeconds;

        if (ui)
        {
            ui.SetScore(_score);
            ui.SetTimer(_timeLeft);
            ui.SetSigningButtonVisible(false);
        }

        StopAllSets();

        if (drawer1) drawer1.ResetState();
        if (drawer2) drawer2.ResetState();
        if (drawer3) drawer3.ResetState();

        if (exitDoor) exitDoor.SetActive(false);

        if (failPanel) failPanel.SetActive(false);
        if (successPanel) successPanel.SetActive(false);
        if (timeScorePanel) timeScorePanel.SetActive(false);
    }

    void StopAllSets()
    {
        if (balloonSet1) balloonSet1.StopSet(true);
        if (balloonSet2) balloonSet2.StopSet(true);
    }

    // ===== Balloon scoring =====

    public void ProcessBalloonPop(bool isCorrect, Vector3 worldPos)
    {
        if (_levelComplete || !_running) return;

        int delta = isCorrect ? 1 : 0; // no penalty for wrong
        AddScore(delta);
    }

    public void AddScore(int delta)
    {
        if (delta <= 0 && !_running) return;

        _score += delta;
        if (ui) ui.SetScore(_score);

        // Unlocks at 5 and 10
        if (_score >= 5 && !_drawer1Unlocked) {
            _drawer1Unlocked = true;
            drawer1?.UnlockAndOpen();
            ui?.ShowHint("Great job! The first drawer is now unlocked. Collect the pen.");
        }

        if (_score >= 10 && !_drawer2Unlocked) {
            _drawer2Unlocked = true;
            drawer2?.UnlockAndOpen();
            ui?.ShowHint("Awesome! The second drawer is now unlocked. Grab the document.");
        }

    }

    // ===== Item & document/key flow =====

    /// <summary>
    /// Called by item pickups (Pen, Document, Key).
    /// </summary>
    public void OnItemPicked(ItemType item)
    {
        switch (item) {
            case ItemType.Pen:
                ui.ShowHint("You picked up the pen! Keep going.");
                break;
            case ItemType.Document:
                if (inventory.HasPen)
                    ui.ShowHint("Great! You have both the pen and the document. Tap the Sign button to sign.");
                else
                    ui.ShowHint("You found the document. Now find the pen to sign it.");
                break;
            case ItemType.Key:
                ui.ShowHint("You collected the key. Head to the exit door to escape!");
                
                break;
            
        }
        EvaluateSigningState();

    }

    public void OnDocumentSigned() {
        drawer3?.UnlockAndOpen();              // still open the third drawer
        exitDoor?.SetActive(true);             // exit door appears immediately
        ui?.ShowHint("You have achieved the goal! Head to the exit door to escape.");
        ui?.SetSigningButtonVisible(false);    // hide Sign button
        inventory?.SignDocument();
    }


    /// <summary>
    /// If some systems directly notify that the key was obtained.
    /// </summary>
    public void OnKeyPicked()
    {
        // Old call expected GiveKey(); in new Inventory use Add(Key)
        inventory?.Add(ItemType.Key); // <- fixes CS1061
        if (exitDoor) exitDoor.SetActive(true);
    }

    /// <summary>
    /// Called by DoorController when the exit door opens.
    /// Kept because DoorController references OnDoorOpened().
    /// </summary>
    public void OnDoorOpened()
    {
        OnLevelComplete();
    }

    /// <summary>
    /// Player reached exit / completed level.
    /// </summary>
    public void OnLevelComplete()
    {
        if (_levelComplete) return;

        _levelComplete = true;
        _running = false;
        StopAllSets();

        if (successPanel) successPanel.SetActive(true);
    }
    private void EvaluateSigningState()
    {
        bool canSign = inventory && inventory.HasPen && inventory.HasDocument && !inventory.DocumentSigned;
        ui?.SetSigningButtonVisible(canSign);
        if (canSign)
            ui?.ShowHint("You have the pen and the document. Tap the Sign button to sign.");
    }

    // ===== Set coordination =====

    void OnSet1Completed()
    {
        if (balloonSet2) balloonSet2.BeginSet();
    }

    // ===== Fail UI =====

    void ShowFail()
    {
        if (failPanel) failPanel.SetActive(true);
    }
}
