// GuardianController.cs
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
    [SerializeField] private float blockingDistance = 10f;

    private GuardianState _currentState = GuardianState.Orbiting;
    private GuardianOrbit _orbit;
    private GuardianDetection _detection;
    private GuardianBlocker _blocker;

    public GuardianState CurrentState => _currentState;

    private void Awake()
    {
        _orbit = GetComponent<GuardianOrbit>();
        _detection = GetComponent<GuardianDetection>();
        _blocker = GetComponent<GuardianBlocker>();
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
            TransitionTo(GuardianState.Orbiting);
            return;
        }

        // Bloquea en cuanto detecta al jugador, sin importar distancia
        TransitionTo(GuardianState.Blocking);
    }

    private void ExecuteState()
    {
        _orbit.enabled = _currentState == GuardianState.Orbiting;
        _blocker.enabled = _currentState == GuardianState.Blocking;
    }

    private void TransitionTo(GuardianState newState)
    {
        if (_currentState == newState) return;
        _currentState = newState;
    }

    public GuardianState GetState() => _currentState;
}