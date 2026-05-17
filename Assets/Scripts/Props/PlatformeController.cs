using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlatformeController : MonoBehaviour
{
    public LayerMask layermask;
    public float waitTime;
    public float timeToReachNextPoint;

    public bool cyclic;

    public bool _canMove = true;

    public Vector3[] localWaypoints;
    Vector3[] globalWaypoints;
    int toWaypointIndex;

    private Coroutine moveRoutine;
    private CharacterController controller;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }

        moveRoutine = StartCoroutine(MoveCoroutine());
    }

    void Move()
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveToPosition(globalWaypoints[toWaypointIndex], timeToReachNextPoint));
    }

    IEnumerator MoveToPosition(Vector3 target, float duration)
    {
        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (!_canMove) yield break;

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Vector3 nextPos = Vector3.Lerp(start, target, t);
            Vector3 delta = nextPos - transform.position;
            controller.Move(delta); // déplacement via CharacterController
            yield return null;
        }

        // s'assurer qu'on est exactement sur la position cible
        Vector3 finalDelta = target - transform.position;
        controller.Move(finalDelta);

        if (_canMove)
            OnMoveComplete();
    }

    void OnMoveComplete()
    {
        toWaypointIndex++;
        if (toWaypointIndex >= globalWaypoints.Length)
        {
            toWaypointIndex = 0;
        }
        if (cyclic)
        {
            System.Array.Reverse(globalWaypoints);
        }

        moveRoutine = StartCoroutine(MoveCoroutine());
    }

    IEnumerator MoveCoroutine()
    {
        yield return new WaitForSeconds(waitTime);
        Move();
    }

    void OnDrawGizmos()
    {
        if (localWaypoints != null)
        {
            Gizmos.color = Color.red;
            float size = 0.3f;

            for (int i = 0; i < localWaypoints.Length; i++)
            {
                Vector3 globalWaypointPos = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + transform.position;
                Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            other.GetComponent<PlayerMovement>().SetParent(transform);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            other.GetComponent<PlayerMovement>().ClearParent();
        }
    }

    public void Activate()
    {
        _canMove = true;
        toWaypointIndex = 1;
        Move();
    }

    public void EnableFromSwitch()
    {
        Activate();
    }

    public void CallFromSwitch()
    {
        _canMove = false;
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveToPosition(globalWaypoints[0], 1f));
    }

    public void ActivateAfterPlayerMove()
    {
        Activate();
    }
}
