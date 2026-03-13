using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemPickup_updated : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemType_updated itemType;
    [SerializeField] private AudioSource sfx;
    [SerializeField] private AudioClip pickupClip;

    private bool _taken;

    public void Interact(Vector3 hitPoint)
    {
        if (_taken) return;
        _taken = true;

        var gm = GameManager_updated.Instance;
        if (gm && gm.Inventory)
        {
            gm.Inventory.Add(itemType);
            gm.OnItemPicked(itemType);
        }

        if (sfx && pickupClip) sfx.PlayOneShot(pickupClip);
        gameObject.SetActive(false);
    }
}