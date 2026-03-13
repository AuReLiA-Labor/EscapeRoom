using System.Collections;
using UnityEngine;

public class DrawerLock_updated : MonoBehaviour
{
    public enum Axis { X, Y, Z }

    [SerializeField] private Transform drawerTransform;
    [SerializeField] private Axis slideAxis = Axis.X;
    [SerializeField] private float openDistance = 0.12f;
    [SerializeField] private float openSeconds = 0.15f;
    [SerializeField] private GameObject itemInside;

    public bool IsUnlocked { get; private set; }

    private Vector3 _closedLocalPos;
    private bool _opened;
    private Coroutine _anim;

    private void Awake()
    {
        if (drawerTransform)
            _closedLocalPos = drawerTransform.localPosition;

        if (itemInside) itemInside.SetActive(false);
    }

    public void UnlockAndOpen()
    {
        IsUnlocked = true;
        SetOpen(true);
    }

    public void TryToggle()
    {
        if (!IsUnlocked) return;
        SetOpen(!_opened);
    }

    public void SetOpen(bool open)
    {
        if (!drawerTransform) return;

        if (_anim != null) StopCoroutine(_anim);
        _anim = StartCoroutine(Animate(open));
    }

    private IEnumerator Animate(bool opening)
    {
        Vector3 from = drawerTransform.localPosition;
        Vector3 to = _closedLocalPos;

        switch (slideAxis)
        {
            case Axis.X: to.x += (opening ? openDistance : 0f); break;
            case Axis.Y: to.y += (opening ? openDistance : 0f); break;
            case Axis.Z: to.z += (opening ? openDistance : 0f); break;
        }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, openSeconds);
            drawerTransform.localPosition = Vector3.Lerp(from, to, t);
            yield return null;
        }

        drawerTransform.localPosition = to;
        _opened = opening;
        if (itemInside) itemInside.SetActive(_opened);
        _anim = null;
    }

    public void ResetState()
    {
        IsUnlocked = false;
        _opened = false;
        if (drawerTransform) drawerTransform.localPosition = _closedLocalPos;
        if (itemInside) itemInside.SetActive(false);
    }
}
