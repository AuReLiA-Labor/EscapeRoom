using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.TouchPhase;

public class TouchInteractor : MonoBehaviour
{
    [SerializeField] private Camera arCamera;
    [SerializeField] private LayerMask interactablesLayer = ~0;
    [SerializeField] private float maxDistance = 15f;
    [SerializeField] private InputAction tapAction; // optional

    void OnEnable(){ if (tapAction != null) tapAction.Enable(); }
    void OnDisable(){ if (tapAction != null) tapAction.Disable(); }

    void Update()
    {
        if (tapAction == null)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !IsPointerOverUI())
                TryRaycastAt(Input.GetTouch(0).position);
            return;
        }

        // New Input System path
        if (tapAction.WasPerformedThisFrame() && !IsPointerOverUI())
        {
            var pos = Mouse.current != null ? Mouse.current.position.ReadValue()
                : (Vector2)UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0].screenPosition;
            TryRaycastAt(pos);
        }
    }

    private void TryRaycastAt(Vector2 screenPos)
    {
        var ray = arCamera.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out var hit, maxDistance, interactablesLayer, QueryTriggerInteraction.Ignore))
            hit.collider.GetComponentInParent<IInteractable>()?.Interact(hit.point);
    }

    private static bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        // Mouse
        if (EventSystem.current.IsPointerOverGameObject()) return true;

        // Touch (new Input System)
        var ts = UnityEngine.InputSystem.Touchscreen.current;
        if (ts != null)
        {
            foreach (var t in ts.touches)
            {
                if (!t.press.isPressed) continue;
                // fingerId is the control index
                if (EventSystem.current.IsPointerOverGameObject(t.touchId.ReadValue()))
                    return true;
            }
        }
        return false;
    }

}