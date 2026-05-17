using UnityEngine;
using UnityEngine.Events;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] private UnityEvent onEnterEvents;
    [SerializeField] private UnityEvent onExitEvents;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameConstants.TAG_PLAYER))
            onEnterEvents.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(GameConstants.TAG_PLAYER))
            onExitEvents.Invoke();
    }
}
