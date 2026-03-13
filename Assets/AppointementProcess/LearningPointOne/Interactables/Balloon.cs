using UnityEngine;
using UnityEngine.EventSystems; // for mobile/UI-safe pointer detection

[RequireComponent(typeof(Collider))] // swap to Collider2D for a 2D project
public class Balloon : MonoBehaviour, IInteractable, IPointerClickHandler
{
    public enum PopMode
    {
        InteractImmediate, // One tap/interaction pops the balloon
        ClickToPop         // Requires multiple taps; scales up slightly each tap
    }

    [Header("Balloon Data")]
    public bool isCorrect = false;

    [Header("Scoring")]
    [Min(1)] public int scoreToGive = 1;   // NEW: replaces statementText

    [Header("Pop Behavior")]
    public PopMode popMode = PopMode.InteractImmediate;
    [Min(1)] public int clicksToPop = 5;
    [Min(0f)] public float scaleIncreasePerClick = 0.1f;

    [Header("Input")]
    public Camera inputCamera; // If null, uses Camera.main
    public bool enableTouchRaycastFallback = true; // Works without EventSystem clicks

    [Header("Effects (Correct/Wrong)")]
    public ParticleSystem correctVfx;
    public ParticleSystem wrongVfx;
    public AudioClip ding; // correct
    public AudioClip buzz; // wrong

    [Header("Fallback Effects")]
    public ParticleSystem popEffectPrefab;
    public AudioClip popClip;
    [Range(0f, 1f)] public float popVolume = 1f;

    private bool _popped = false;
    private int _clicks = 0;
    private Vector3 _initialScale;

    void Awake()
    {
        _initialScale = transform.localScale;
        clicksToPop = Mathf.Max(1, clicksToPop);
        scaleIncreasePerClick = Mathf.Max(0f, scaleIncreasePerClick);
        if (inputCamera == null) inputCamera = Camera.main;
    }

    // IInteractable (parameterless)
    public void Interact()
    {
        if (_popped) return;
        StepTowardPop();
    }

    // IInteractable (required by your interface)
    public void Interact(Vector3 hitPoint)
    {
        Interact();
    }

    // Desktop/editor convenience; also works on mobile if OS simulates mouse
    void OnMouseDown()
    {
        if (_popped) return;
        StepTowardPop();
    }

    // Preferred on mobile when you have an EventSystem + PhysicsRaycaster
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_popped) return;
        StepTowardPop();
    }

    // Fallback: direct touch raycasts (no EventSystem needed)
    void Update()
    {
        if (!enableTouchRaycastFallback || _popped) return;
        if (Input.touchCount == 0) return;

        for (int i = 0; i < Input.touchCount; i++)
        {
            var touch = Input.GetTouch(i);
            if (touch.phase != TouchPhase.Began) continue;

            // Ignore touches over UI (if EventSystem exists)
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                continue;

            if (WasThisBalloonTapped(touch.position))
            {
                StepTowardPop();
                break; // consume one tap
            }
        }
    }

    private bool WasThisBalloonTapped(Vector2 screenPos)
    {
        if (inputCamera == null) return false;

        // 3D physics
        Ray ray = inputCamera.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit3D))
        {
            if (hit3D.transform == transform || hit3D.transform.IsChildOf(transform))
                return true;
        }

        // 2D physics (optional, in case you use 2D colliders)
        Vector3 world = inputCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, inputCamera.nearClipPlane));
        var hit2D = Physics2D.OverlapPoint(world);
        if (hit2D && (hit2D.transform == transform || hit2D.transform.IsChildOf(transform)))
            return true;

        return false;
    }

    private void StepTowardPop()
    {
        _clicks++;

        // Visual inflation for ClickToPop mode
        if (popMode == PopMode.ClickToPop && scaleIncreasePerClick > 0f)
        {
            float multiplier = 1f + (_clicks * scaleIncreasePerClick);
            transform.localScale = _initialScale * multiplier;
        }

        int required = (popMode == PopMode.InteractImmediate) ? 1 : clicksToPop;
        if (_clicks >= required)
            Pop();
    }

    private void Pop()
    {
        if (_popped) return;
        _popped = true;

        // 1) Trigger your existing pop pipeline once (keeps unlocks/timing/flow intact)
        if (GameManager.Instance != null)
            GameManager.Instance.ProcessBalloonPop(isCorrect, transform.position);

        // 2) Adjust to variable scoring (top up difference from the default +/-1)
        //    Correct:  +scoreToGive  (default was +1) -> add (scoreToGive-1)
        //    Wrong:    -scoreToGive  (default was -1) -> add -(scoreToGive-1)
        if (GameManager.Instance != null && scoreToGive > 1)
        {
            int extra = scoreToGive - 1;
            int delta = isCorrect ? extra : -extra;
            GameManager.Instance.AddScore(delta); // see GameManager patch below
        }

        // VFX (prefer correct/wrong; else generic)
        if (isCorrect) SpawnAndAutoDestroy(correctVfx);
        else           SpawnAndAutoDestroy(wrongVfx);

        if (!correctVfx && !wrongVfx)
            SpawnAndAutoDestroy(popEffectPrefab);

        // SFX (prefer correct/wrong; else generic)
        if (isCorrect && ding)        AudioSource.PlayClipAtPoint(ding,  transform.position, popVolume);
        else if (!isCorrect && buzz)  AudioSource.PlayClipAtPoint(buzz,  transform.position, popVolume);
        else if (popClip)             AudioSource.PlayClipAtPoint(popClip, transform.position, popVolume);

        Destroy(gameObject);
    }

    private void SpawnAndAutoDestroy(ParticleSystem prefab)
    {
        if (!prefab) return;
        var ps = Instantiate(prefab, transform.position, Quaternion.identity);
        var main = ps.main;

        float lifetime = main.duration;
        switch (main.startLifetime.mode)
        {
            case ParticleSystemCurveMode.TwoConstants:
                lifetime += main.startLifetime.constantMax;
                break;
            case ParticleSystemCurveMode.Constant:
                lifetime += main.startLifetime.constant;
                break;
        }
        Destroy(ps.gameObject, lifetime);
    }
}
