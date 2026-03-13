using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARHealth : MonoBehaviour
{
    void OnEnable()  { ARSession.stateChanged += OnState; }
    void OnDisable() { ARSession.stateChanged -= OnState; }
    void OnState(ARSessionStateChangedEventArgs e) =>
        Debug.Log("[AR] State: " + e.state);
}