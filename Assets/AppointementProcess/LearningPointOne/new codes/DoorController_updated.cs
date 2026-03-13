using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoorController_updated : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator animator;
    [SerializeField] private string openTrigger = "Open";

    [Header("Audio")]
    [SerializeField] private AudioSource sfx;
    [SerializeField] private AudioClip lockedClip;
    [SerializeField] private AudioClip openClip;

    private bool _opened;

    public void Interact(Vector3 hitPoint)
    {
        if (_opened) return;

        var gm = GameManager_updated.Instance;
        if (!gm || !gm.Inventory) return;

        if (!gm.Inventory.HasKey)
        {
            if (sfx && lockedClip) sfx.PlayOneShot(lockedClip);
            gm.UI?.ShowHint("You need a key to open this door.");
            return;
        }

        _opened = true;
        animator?.SetTrigger(openTrigger);
        if (sfx && openClip) sfx.PlayOneShot(openClip);

        gm.OnDoorOpened();
    }
}