using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceFlowBridge : MonoBehaviour
{
    [SerializeField] private GameObject entranceRoot; // parent with the seven doors
    [SerializeField] private GameObject learningRoot; // parent for the room content (active after entrance)
    
    /// <summary>
    /// Call this when the "Appointment Process" door finishes opening (animation event, trigger, or button).
    /// </summary>
    public void EnterRoom()
    {
        if (entranceRoot) entranceRoot.SetActive(false);
        if (learningRoot) learningRoot.SetActive(true);

       // FindObjectOfType<ARFlow>()?.ShowLearningWelcome();
    }
}
