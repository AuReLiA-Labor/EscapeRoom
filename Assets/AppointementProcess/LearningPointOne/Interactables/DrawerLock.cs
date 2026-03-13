using UnityEngine;
using System.Collections;

public class DrawerLock : MonoBehaviour
{
    public enum Axis { X = 0, Y = 1, Z = 2 }

    [Header("Drawer Movement")]
    [SerializeField] private Transform drawerTransform;   // front mesh to move
    [SerializeField] private Axis slideAxis = Axis.X;     // matches your scene
    [SerializeField] private float openDistance = 0.12f;  // how far from closed (local units)
    [SerializeField] private float openSeconds = 0.15f;   // animation time

    [Header("Contents")]
    [SerializeField] private GameObject itemInside;

    public bool IsUnlocked { get; private set; }
    bool _opened;
    Vector3 _closedLocalPos;

    void Awake()
    {
        if (!drawerTransform) drawerTransform = transform;
        _closedLocalPos = drawerTransform.localPosition;
        if (itemInside) itemInside.SetActive(false);
    }

    public void Unlock() => IsUnlocked = true;

    public void UnlockAndOpen() { Unlock(); Open(); }

    public void Open()
    {
        if (_opened || !IsUnlocked) return;
        StopAllCoroutines();
        StartCoroutine(SlideRoutine(true));
    }

    public void Close()
    {
        if (!_opened) return;
        StopAllCoroutines();
        StartCoroutine(SlideRoutine(false));
    }

    IEnumerator SlideRoutine(bool opening)
    {
        Vector3 from = drawerTransform.localPosition;
        Vector3 delta = slideAxis == Axis.X ? Vector3.right * openDistance
                     : slideAxis == Axis.Y ? Vector3.up    * openDistance
                                           : Vector3.forward * openDistance;
        Vector3 to = opening ? _closedLocalPos + delta : _closedLocalPos;

        float t = 0f, dur = Mathf.Max(0.01f, openSeconds);
        while (t < 1f)
        {
            t += Time.deltaTime / dur;
            drawerTransform.localPosition = Vector3.Lerp(from, to, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
        drawerTransform.localPosition = to;
        _opened = opening;
        if (itemInside) itemInside.SetActive(_opened);
    }

    public void ResetState()
    {
        IsUnlocked = false;
        _opened = false;
        if (drawerTransform) drawerTransform.localPosition = _closedLocalPos;
        if (itemInside) itemInside.SetActive(false);
    }
}
