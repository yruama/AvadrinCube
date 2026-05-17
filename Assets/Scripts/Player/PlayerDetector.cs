using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerDetector : MonoBehaviour
{
   // Créer un événement UnityEvent
    [SerializeField]
    private UnityEvent onEnterEvents;

    [SerializeField]
    private UnityEvent onExitEvents;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == GameConstants.TAG_PLAYER) {
            onEnterEvents.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == GameConstants.TAG_PLAYER) {
            onExitEvents.Invoke();
        }
    }
}
