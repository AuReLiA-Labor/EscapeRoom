using UnityEngine;

public interface IInteractable
{
    // Called when the user taps this object in AR
    void Interact(Vector3 hitPoint);
}