using System.Collections;
using UnityEngine;

/// <summary>
/// Handles the transition from the entrance room into the learning point room.
/// When the player enters a trigger near the open door, this script will optionally
/// animate the door, hide the entrance environment, show the learning environment,
/// reveal an additional plane and smoothly move the camera forward to simulate
/// walking through the door.
/// Attach this script to a GameObject with a trigger collider located at the open door.
/// </summary>
[RequireComponent(typeof(Collider))]
public class EntranceTransition : MonoBehaviour
{
    [Header("Scene Objects")]
    [Tooltip("Root object for the entrance environment (seven doors). Will be deactivated after the transition.")]
    public GameObject entranceRoot;
    [Tooltip("Root object for the learning point environment. Will be activated after the transition.")]
    public GameObject learningPointRoot;
    [Tooltip("An additional plane or object (e.g. plane 4) to reveal when entering the learning environment.")]
    public GameObject planeToReveal;

    [Header("Door and Animation")]
    [Tooltip("If assigned, this door will be commanded to open at the start of the transition.")]
    public EntranceDoor entranceDoor;
    [Tooltip("Delay after opening the door before switching environments (seconds). This should roughly match the door animation length.")]
    public float doorOpenDelay = 1f;

    [Header("Camera Movement")]
    [Tooltip("Distance to nudge the camera forward when passing through the door (meters).")]
    public float cameraMoveDistance = 0.5f;
    [Tooltip("Duration of the camera movement (seconds).")]
    public float cameraMoveDuration = 0.5f;

    private bool _triggered;

    private void Reset()
    {
        // Ensure this collider acts as a trigger by default.
        Collider c = GetComponent<Collider>();
        if (c != null)
            c.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only trigger once and only react to the main camera / player's root.
        if (_triggered) return;
        // Accept either a Camera component or the object tagged as MainCamera.
        if (other.GetComponentInParent<Camera>() == null && !other.CompareTag("MainCamera"))
            return;
        _triggered = true;
        StartCoroutine(DoTransition());
    }

    private IEnumerator DoTransition()
    {
        // If there is a door to open, toggle it.
        if (entranceDoor != null)
        {
            // The EntranceDoor script toggles open/close when calling OpenDoor().  
            // Ensure the door is opened only once.
            if (!entranceDoor.open)
            {
                entranceDoor.OpenDoor();
            }
            // Wait for the animation before switching rooms.
            if (doorOpenDelay > 0f)
                yield return new WaitForSeconds(doorOpenDelay);
        }

        // Hide the entrance and show the learning room.
        if (entranceRoot != null) entranceRoot.SetActive(false);
        if (learningPointRoot != null) learningPointRoot.SetActive(true);
      //  FindObjectOfType<ARFlow>()?.ShowLearningWelcome();
        if (planeToReveal != null) planeToReveal.SetActive(true);

        // Optional camera movement for immersion.
        Camera cam = Camera.main;
        if (cam != null && cameraMoveDistance != 0f && cameraMoveDuration > 0f)
        {
            Vector3 startPos = cam.transform.position;
            Vector3 targetPos = startPos + cam.transform.forward * cameraMoveDistance;
            float t = 0f;
            while (t < cameraMoveDuration)
            {
                float progress = t / cameraMoveDuration;
                cam.transform.position = Vector3.Lerp(startPos, targetPos, progress);
                t += Time.deltaTime;
                yield return null;
            }
            cam.transform.position = targetPos;
        }
    }
}