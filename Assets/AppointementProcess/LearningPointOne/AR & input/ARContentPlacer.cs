using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARContentPlacer : MonoBehaviour
{
    [Header("Scene Refs")]
    [SerializeField] private Camera arCamera;
    [SerializeField] private GameObject gameRoot;
    [SerializeField] private GameObject placementIndicator;

    [Header("UX")]
    [SerializeField] private bool includeEstimatedPlanes = true;
    [SerializeField] private bool includeFeaturePoints  = true;
    [SerializeField] private bool onlyHorizontalUp      = true;
    [SerializeField] private float extraYOffset         = 0.0f;
    [SerializeField] private bool diagnostics           = true;

    private ARRaycastManager _ray;
    private ARPlaneManager   _planes;
    private bool _placed;
    private static readonly List<ARRaycastHit> _hits = new();

    public event Action Placed;

    void Awake()
    {
        _ray    = GetComponent<ARRaycastManager>() ?? FindObjectOfType<ARRaycastManager>(true);
        _planes = GetComponent<ARPlaneManager>()   ?? FindObjectOfType<ARPlaneManager>(true);
        if (!arCamera) arCamera = Camera.main;
    }

    void Start()
    {
        if (gameRoot)           gameRoot.SetActive(false);
        if (placementIndicator) placementIndicator.SetActive(false);
    }

    void Update()
    {
        if (_placed || _ray == null) return;

        // --- indicator update ---
        var center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        var types  = TrackableType.PlaneWithinPolygon;
        if (includeEstimatedPlanes) types |= TrackableType.PlaneEstimated;
        if (includeFeaturePoints)   types |= TrackableType.FeaturePoint;

        if (_ray.Raycast(center, _hits, types))
        {
            var pose = _hits[0].pose;
            if (onlyHorizontalUp && _planes)
            {
                var pl = _planes.GetPlane(_hits[0].trackableId);
                if (pl && pl.alignment != PlaneAlignment.HorizontalUp)
                {
                    if (placementIndicator) placementIndicator.SetActive(false);
                    return;
                }
            }
            if (placementIndicator)
            {
                placementIndicator.transform.SetPositionAndRotation(
                    pose.position,
                    YawFacingCamera()
                );
                placementIndicator.SetActive(true);
            }
        }
        else if (placementIndicator) placementIndicator.SetActive(false);

        // --- tap to place ---
        if (Input.touchCount == 0) return;
        var t = Input.GetTouch(0);
        if (t.phase != TouchPhase.Began) return;

        // Ignore UI if you have any overlay; comment out if not needed
        if (EventSystem.current && (EventSystem.current.IsPointerOverGameObject() ||
                                    EventSystem.current.IsPointerOverGameObject(t.fingerId)))
            return;

        if (_ray.Raycast(t.position, _hits, TrackableType.PlaneWithinPolygon))
        {
            var pose = _hits[0].pose;
            if (onlyHorizontalUp && _planes)
            {
                var pl = _planes.GetPlane(_hits[0].trackableId);
                if (pl && pl.alignment != PlaneAlignment.HorizontalUp) return;
            }
            PlaceAt(_hits[0], pose);
        }
    }

    private void PlaceAt(ARRaycastHit hit, Pose pose)
    {
        if (!gameRoot)
        {
            Debug.LogWarning("[AR] gameRoot not assigned.");
            return;
        }

        // 1) move to the plane hit / reticle pose (yaw to camera)
        var rot = YawFacingCamera();
        gameRoot.transform.SetPositionAndRotation(pose.position, rot);
        gameRoot.SetActive(true); // ensure visible

        // 2) snap the *bottom* of the meshes to plane Y so it isn't buried
        var planeY  = pose.position.y;
        var bottomY = GetContentBottomWorldY(gameRoot.transform);
        var dy      = (planeY - bottomY) + extraYOffset;
        gameRoot.transform.position += Vector3.up * dy;

        // 3) safety checks: layer & scale
        if (arCamera && ((arCamera.cullingMask & (1 << gameRoot.layer)) == 0))
            Debug.LogWarning($"[AR] Camera '{arCamera.name}' does not render layer {LayerMask.LayerToName(gameRoot.layer)} of gameRoot.");

        if (gameRoot.transform.lossyScale.magnitude < 0.001f)
        {
            Debug.LogWarning("[AR] gameRoot scale is near zero; normalizing to (1,1,1).");
            gameRoot.transform.localScale = Vector3.one;
        }

        // 4) finish
        if (placementIndicator) placementIndicator.SetActive(false);
        _placed = true;
        Placed?.Invoke();

        // (Optional) hide plane visuals / stop detection
        if (_planes)
        {
            foreach (var p in _planes.trackables) p.gameObject.SetActive(false);
            _planes.enabled = false;
        }

        if (diagnostics) Debug.Log("[AR] Placed + snapped to plane.");
    }

    private static float GetContentBottomWorldY(Transform root)
    {
        var renderers = root.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0) return root.position.y;

        var minY = float.PositiveInfinity;
        foreach (var r in renderers)
        {
            if (!r.enabled) continue;
            var b = r.bounds;
            if (b.size == Vector3.zero) continue;
            if (b.min.y < minY) minY = b.min.y;
        }
        return float.IsPositiveInfinity(minY) ? root.position.y : minY;
    }

    private Quaternion YawFacingCamera()
    {
        if (!arCamera) return Quaternion.identity;
        var fwd = arCamera.transform.forward; fwd.y = 0f;
        if (fwd.sqrMagnitude < 1e-4f) fwd = Vector3.forward;
        return Quaternion.LookRotation(fwd.normalized, Vector3.up);
    }
}
