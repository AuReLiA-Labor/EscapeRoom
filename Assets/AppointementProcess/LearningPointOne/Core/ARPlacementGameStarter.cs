using UnityEngine;

public class ARPlacementGameStarter : MonoBehaviour
{
    [Tooltip("The ARContentPlacer responsible for placing the AR room.")]
    public ARContentPlacer placer;

    [Tooltip("The flow controller used to show/hide the UI panels.")]
    public ARFlow arFlow;

    [Tooltip("Start gameplay immediately when placed.")]
    public bool autoStartOnPlaced = false;

    bool _subscribed;

    void Awake()
    {
        // MOD: try to auto-wire if missing
        if (!placer) placer = FindObjectOfType<ARContentPlacer>(true);
        if (!arFlow) arFlow = FindObjectOfType<ARFlow>(true);
    }

    void OnEnable()
    {
        if (placer != null && !_subscribed)
        {
            placer.Placed += OnPlaced;
            _subscribed = true;
        }
    }

    void OnDisable()
    {
        if (placer != null && _subscribed)
        {
            placer.Placed -= OnPlaced;
            _subscribed = false;
        }
    }

    private void OnPlaced()
    {
        // Show HUD / enter in-game panel
        if (arFlow != null) arFlow.ShowInGamePanel();

        // Start the gameplay timer if desired
        if (autoStartOnPlaced && GameManager.Instance != null)
            GameManager.Instance.StartGameplay();
    }
}