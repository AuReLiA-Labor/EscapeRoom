using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoorController : MonoBehaviour, IInteractable
{
    public Animator animator;
    public string openTrigger = "Open";
    public bool IsUnlocked { get; private set; } = false;

    [Header("Audio")]
    public AudioSource sfx;
    public AudioClip lockedSfx;
    public AudioClip unlockSfx;
    public AudioClip openSfx;

    public void UnlockWithKey()
    {
        IsUnlocked = true;
        if (sfx && unlockSfx) sfx.PlayOneShot(unlockSfx);
        GameManager.Instance.inventory.UseKey(); // consume if you want one-time use
    }

    public void Interact(Vector3 hitPoint)
    {
        if (!IsUnlocked)
        {
            // Try to unlock with key, else hint
            var inv = GameManager.Instance.inventory;
            if (inv != null && inv.HasKey) UnlockWithKey();
            else {
                if (sfx && lockedSfx) sfx.PlayOneShot(lockedSfx);
                GameManager.Instance.ui.ShowHint("You need a key to open this door.");
                return;
            }
        }
        animator?.SetTrigger(openTrigger);
        if (sfx && openSfx) sfx.PlayOneShot(openSfx);
        GameManager.Instance.OnDoorOpened();
        enabled = false;
    }
}