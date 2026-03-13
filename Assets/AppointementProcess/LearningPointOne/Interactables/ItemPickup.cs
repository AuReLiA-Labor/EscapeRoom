using UnityEngine;

public enum ItemType { Pen, Document, Key }

[RequireComponent(typeof(Collider))]
public class ItemPickup : MonoBehaviour, IInteractable
{
    public ItemType itemType;
    public AudioSource sfx;
    public AudioClip pickupSfx;

    private bool _taken = false;

    private void OnEnable() { _taken = false; }

    public void Interact(Vector3 hitPoint)
    {
        if (_taken) return;
        _taken = true;

        if (GameManager.Instance && GameManager.Instance.inventory)
        {
            GameManager.Instance.inventory.Add(itemType);
            GameManager.Instance.OnItemPicked(itemType);
        }

        if (sfx && pickupSfx) sfx.PlayOneShot(pickupSfx);
        gameObject.SetActive(false);
    }
}