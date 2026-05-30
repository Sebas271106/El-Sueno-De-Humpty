using UnityEngine;

public enum GuardianState
{
    Idle,
    Orbiting,
    Blocking
}

[RequireComponent(typeof(GuardianOrbit))]
[RequireComponent(typeof(GuardianDetection))]
[RequireComponent(typeof(GuardianBlocker))]
public class GuardianController : MonoBehaviour
{
    [Header("State Settings")]
    [SerializeField] private float idleTimeBeforeOrbit = 2f;
    [SerializeField] private float blockingDistance = 2.5f;

    private GuardianState _currentState = GuardianState.Idle;
    private GuardianOrbit _orbit;
    private GuardianDetection _detection;
    private GuardianBlocker _blocker;
    private float _idleTimer;

    public GuardianState CurrentState => _currentState;

    private void Awake()
    {
        _orbit     = GetComponent<GuardianOrbit>();
        _detection = GetComponent<GuardianDetection>();
        _blocker   = GetComponent<GuardianBlocker>();
    }

    private void Update()
    {
        EvaluateState();
        ExecuteState();
    }

    private void EvaluateState()
    {
        bool playerDetected = _detection.IsPlayerDetected;
        float distanceToPlayer = _detection.DistanceToPlayer;

        if (!playerDetected)
        {
            _idleTimer += Time.deltaTime;
            if (_idleTimer >= idleTimeBeforeOrbit)
                TransitionTo(GuardianState.Orbiting);
            else
                TransitionTo(GuardianState.Idle);
            return;
        }

        _idleTimer = 0f;

        if (distanceToPlayer <= blockingDistance)
            TransitionTo(GuardianState.Blocking);
        else
            TransitionTo(GuardianState.Orbiting);
    }

    private void ExecuteState()
    {
        _orbit.enabled   = _currentState == GuardianState.Orbiting;
        _blocker.enabled = _currentState == GuardianState.Blocking;
    }

    private void TransitionTo(GuardianState newState)
    {
        if (_currentState == newState) return;
        _currentState = newState;
        _idleTimer = 0f;
    }

    public GuardianState GetState() => _currentState;
}