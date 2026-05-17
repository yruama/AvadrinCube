using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class PlatformeController : MonoBehaviour, ISwitchable
{
    [Header("Waypoints (espace local)")]
    [SerializeField] private Vector3[] localWaypoints;

    [Header("Timing")]
    [SerializeField] private PlatformMotionConfig _motionConfig;
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private float timeToReachNextPoint = 5f;

    [Header("Comportement")]
    [SerializeField] private bool cyclic;
    [SerializeField] private bool startMovingOnAwake = true;
    [SerializeField] private bool pingPong;

    [Header("Collider du dessus")]
    [SerializeField] private bool autoSetupTopCollider = true;
    [SerializeField] private Vector3 topColliderSize = new Vector3(2f, 0.15f, 2f);
    [SerializeField] private Vector3 topColliderCenter = new Vector3(0f, 0.5f, 0f);

    [SerializeField] private bool _canMove = true;

    private const string TopSurfaceName = "PlatformTopSurface";

    private readonly HashSet<CharacterController> _passengers = new HashSet<CharacterController>();

    private Vector3[] _globalWaypoints;
    private int _waypointIndex;
    private int _direction = 1;

    private enum MoveState { Waiting, Moving }
    private MoveState _state = MoveState.Waiting;
    private float _stateTimer;
    private Vector3 _segmentStart;
    private Vector3 _segmentTarget;
    private Vector3 _positionBeforeFixedUpdate;

    public Vector3 FrameDelta { get; private set; }
    public Vector3 Velocity { get; private set; }

    public bool CanMove
    {
        get => _canMove;
        set => _canMove = value;
    }

    void Awake()
    {
        if (_motionConfig != null)
        {
            waitTime = _motionConfig.waitTime;
            timeToReachNextPoint = _motionConfig.timeToReachNextPoint;
        }

        RebuildGlobalWaypoints();
        if (autoSetupTopCollider)
            SetupColliders();
    }

    void Start()
    {
        if (startMovingOnAwake && _globalWaypoints.Length > 1)
            BeginWaiting();
    }

    void FixedUpdate()
    {
        _positionBeforeFixedUpdate = transform.position;
        FrameDelta = Vector3.zero;
        Velocity = Vector3.zero;

        if (_canMove && _globalWaypoints != null && _globalWaypoints.Length >= 2)
        {
            float dt = Time.fixedDeltaTime;

            switch (_state)
            {
                case MoveState.Waiting:
                    _stateTimer -= dt;
                    if (_stateTimer <= 0f)
                        StartNextSegment();
                    break;

                case MoveState.Moving:
                    _stateTimer += dt;
                    float duration = Mathf.Max(timeToReachNextPoint, 0.01f);
                    float t = Mathf.Clamp01(_stateTimer / duration);
                    transform.position = Vector3.Lerp(_segmentStart, _segmentTarget, EaseInOut(t));
                    if (t >= 1f)
                        OnSegmentComplete();
                    break;
            }
        }

        FrameDelta = transform.position - _positionBeforeFixedUpdate;
        if (Time.fixedDeltaTime > 0f)
            Velocity = FrameDelta / Time.fixedDeltaTime;

        TransportPassengers();
    }

    public void SetPassenger(CharacterController passenger, bool isRiding)
    {
        if (passenger == null)
            return;

        if (isRiding)
            _passengers.Add(passenger);
        else
            _passengers.Remove(passenger);
    }

    private void TransportPassengers()
    {
        if (FrameDelta.sqrMagnitude < 0.000001f)
            return;

        _passengers.RemoveWhere(cc => cc == null || !cc.enabled);

        foreach (CharacterController passenger in _passengers)
            passenger.Move(FrameDelta);
    }

    void OnDrawGizmosSelected()
    {
        if (localWaypoints == null || localWaypoints.Length == 0)
            return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            Vector3 p = Application.isPlaying && _globalWaypoints != null && i < _globalWaypoints.Length
                ? _globalWaypoints[i]
                : transform.position + localWaypoints[i];
            Gizmos.DrawWireSphere(p, 0.25f);
        }
    }

    public void RebuildGlobalWaypoints()
    {
        if (localWaypoints == null || localWaypoints.Length == 0)
        {
            _globalWaypoints = new Vector3[0];
            return;
        }

        _globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
            _globalWaypoints[i] = transform.position + localWaypoints[i];
    }

    private void SetupColliders()
    {
        foreach (Collider col in GetComponentsInChildren<Collider>(true))
        {
            if (col.isTrigger || col.gameObject.name == TopSurfaceName)
                continue;
            col.enabled = false;
        }

        Transform top = transform.Find(TopSurfaceName);
        if (top == null)
        {
            top = new GameObject(TopSurfaceName).transform;
            top.SetParent(transform, false);
        }

        top.localPosition = Vector3.zero;
        top.localRotation = Quaternion.identity;
        top.gameObject.layer = GameConstants.LAYER_GROUND;

        BoxCollider box = top.GetComponent<BoxCollider>();
        if (box == null)
            box = top.gameObject.AddComponent<BoxCollider>();

        box.isTrigger = false;
        box.center = topColliderCenter;
        box.size = topColliderSize;

        if (top.GetComponent<MovingPlatformSurface>() == null)
            top.gameObject.AddComponent<MovingPlatformSurface>();
    }

    private void BeginWaiting()
    {
        _state = MoveState.Waiting;
        _stateTimer = waitTime;
    }

    private void StartNextSegment()
    {
        _segmentStart = transform.position;
        _waypointIndex = Mathf.Clamp(_waypointIndex, 0, _globalWaypoints.Length - 1);
        _segmentTarget = _globalWaypoints[_waypointIndex];
        _state = MoveState.Moving;
        _stateTimer = 0f;
    }

    private void OnSegmentComplete()
    {
        transform.position = _segmentTarget;
        AdvanceWaypointIndex();
        BeginWaiting();
    }

    private void AdvanceWaypointIndex()
    {
        if (pingPong && _globalWaypoints.Length > 1)
        {
            int last = _globalWaypoints.Length - 1;
            int next = _waypointIndex + _direction;
            if (next > last) { _direction = -1; _waypointIndex = last - 1; }
            else if (next < 0) { _direction = 1; _waypointIndex = 1; }
            else _waypointIndex = next;
            return;
        }

        _waypointIndex++;
        if (_waypointIndex >= _globalWaypoints.Length)
        {
            if (cyclic)
                System.Array.Reverse(_globalWaypoints);
            _waypointIndex = 0;
        }
    }

    private static float EaseInOut(float t) => t * t * (3f - 2f * t);

    public void Activate()
    {
        _canMove = true;
        if (_globalWaypoints != null && _globalWaypoints.Length > 1)
        {
            _waypointIndex = Mathf.Min(1, _globalWaypoints.Length - 1);
            BeginWaiting();
        }
    }

    public void EnableFromSwitch() => Activate();

    public void CallFromSwitch()
    {
        _canMove = false;
        _state = MoveState.Moving;
        _stateTimer = 0f;
        _segmentStart = transform.position;
        _segmentTarget = _globalWaypoints != null && _globalWaypoints.Length > 0
            ? _globalWaypoints[0]
            : transform.position;
        timeToReachNextPoint = 1f;
    }

    public void ActivateAfterPlayerMove() => Activate();
}
