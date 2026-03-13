using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Proximity-triggered entrance UI for the "Appointment Process" door.
/// - Shows EntranceCanvas when the player/camera is near
/// - Displays locked/unlocked status
/// - "Open" button calls the door's OpenDoor()
/// - On open, optionally enters the room via EntranceFlowBridge.EnterRoom()
/// </summary>
public class OpenEntranceDorTrigger : MonoBehaviour
{
    [Header("Proximity")]
    [Tooltip("Assign the player camera (or leave empty to auto-grab Camera.main).")]
    public Transform player;
    [Tooltip("Meters from this object within which the panel appears.")]
    public float activationDistance = 2.5f;

    [Header("UI Panel (EntranceCanvas)")]
    public GameObject entranceCanvas;
    public TMP_Text statusText;
    public Button openButton;

    [Header("Door to Open")]
    [Tooltip("Script with an OpenDoor() method (e.g., DoorController).")]
    public MonoBehaviour door;                        // any component that has OpenDoor()
    [Tooltip("Method invoked on 'door' via SendMessage (keep default unless your method is named differently).")]
    public string openMethodName = "OpenDoor";

    [Header("Flow Bridge")]
    [Tooltip("Calls EnterRoom() after the door opens.")]
    public EntranceFlowBridge flowBridge;
    [Tooltip("If true, room transition happens immediately after triggering OpenDoor(). Turn OFF if you use an animation event to time it.")]
    public bool autoEnterRoomOnOpen = true;

    [Header("Door State")]
    [Tooltip("Set true ONLY for the 'Appointment Process' door. Others stay locked.")]
    public bool isUnlocked = true;

    bool _panelVisible;
    bool _doorHasOpened;

    void Awake()
    {
        // Wire the button
        if (openButton != null) openButton.onClick.AddListener(OnOpenClicked);
        HidePanelImmediate();
    }

    void Start()
    {
        // Lazy camera resolve
        if (player == null && Camera.main != null)
            player = Camera.main.transform;

        RefreshPanelContent(); // reflect locked/unlocked on startup
    }

    void Update()
    {
        if (_doorHasOpened) { HidePanel(); return; }
        if (player == null) return;

        float d = Vector3.Distance(player.position, transform.position);
        bool shouldShow = d <= activationDistance;

        if (shouldShow != _panelVisible)
        {
            if (shouldShow) ShowPanel();
            else HidePanel();
        }
    }

    // ===== Button handler =====

    void OnOpenClicked()
    {
        if (_doorHasOpened) return;

        if (!isUnlocked)
        {
            // Simply refresh text; the button should already be non-interactable
            RefreshPanelContent();
            return;
        }

        // Try to open the door
        if (door != null)
        {
            // Use SendMessage so any door controller with 'OpenDoor' works
            door.SendMessage(openMethodName, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            Debug.LogWarning("[OpenEntranceDorTrigger] No door component assigned.");
        }

        if (autoEnterRoomOnOpen)
        {
            // Transition immediately (use animation event method below if you want to wait for the animation end)
            EnterRoomNow();
        }
        else
        {
            // Wait for animation event: call OnDoorOpenAnimationComplete() from your Animator.
            // Panel can already hide to avoid double input
            HidePanel();
        }
    }

    // ===== Public hooks =====

    /// <summary>
    /// Call this from an Animation Event at the end of the door-open animation.
    /// </summary>
    public void OnDoorOpenAnimationComplete()
    {
        if (_doorHasOpened) return;
        EnterRoomNow();
    }

    public void Unlock()  { isUnlocked = true;  RefreshPanelContent(); }
    public void LockDoor(){ isUnlocked = false; RefreshPanelContent(); }

    // ===== Internals =====

    void EnterRoomNow()
    {
        _doorHasOpened = true;
        HidePanelImmediate();
        if (flowBridge != null) flowBridge.EnterRoom();
    }

    void ShowPanel()
    {
        _panelVisible = true;
        if (entranceCanvas != null && !entranceCanvas.activeSelf)
            entranceCanvas.SetActive(true);

        RefreshPanelContent();
    }

    void HidePanel()
    {
        _panelVisible = false;
        if (entranceCanvas != null && entranceCanvas.activeSelf)
            entranceCanvas.SetActive(false);
    }

    void HidePanelImmediate()
    {
        _panelVisible = false;
        if (entranceCanvas != null)
            entranceCanvas.SetActive(false);
    }

    void RefreshPanelContent()
    {
        if (statusText != null)
        {
            statusText.text = isUnlocked
                ? "Appointment Process: You can enter."
                : "This door is locked.";
        }

        if (openButton != null)
            openButton.interactable = isUnlocked && !_doorHasOpened;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (activationDistance < 0f) activationDistance = 0f;
        RefreshPanelContent();
    }
#endif
}
