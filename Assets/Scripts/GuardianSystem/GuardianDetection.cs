using UnityEngine;

public class GuardianDetection : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRadius = 8f;
    [SerializeField] private float loseTrackRadius = 12f;

    [Header("Prediction Settings")]
    [SerializeField] private float velocitySampleRate = 0.1f; // cada cuánto muestrea la posición

    private bool _isPlayerDetected;
    private float _distanceToPlayer = float.MaxValue;

    // Velocidad calculada del jugador
    private Vector3 _playerVelocity = Vector3.zero;
    private Vector3 _lastPlayerPosition;
    private float _sampleTimer = 0f;
    private bool _firstSample = true;

    public bool IsPlayerDetected => _isPlayerDetected;
    public float DistanceToPlayer => _distanceToPlayer;
    public Transform PlayerTransform => player;
    public Vector3 PlayerVelocity => _playerVelocity;

    private void Update()
    {
        if (player == null) return;
        UpdateDetection();
        SamplePlayerVelocity();
    }

    private void UpdateDetection()
    {
        Vector3 selfFlat   = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 playerFlat = new Vector3(player.position.x,    0f, player.position.z);

        _distanceToPlayer = Vector3.Distance(selfFlat, playerFlat);

        if (!_isPlayerDetected && _distanceToPlayer <= detectionRadius)
            _isPlayerDetected = true;
        else if (_isPlayerDetected && _distanceToPlayer >= loseTrackRadius)
            _isPlayerDetected = false;
    }

    private void SamplePlayerVelocity()
    {
        // Primera muestra — solo guarda posición sin calcular velocidad
        if (_firstSample)
        {
            _lastPlayerPosition = player.position;
            _firstSample = false;
            return;
        }

        _sampleTimer += Time.deltaTime;

        if (_sampleTimer >= velocitySampleRate)
        {
            Vector3 delta = player.position - _lastPlayerPosition;
            delta.y = 0f;

            // Velocidad en unidades/segundo
            _playerVelocity = delta / _sampleTimer;

            _lastPlayerPosition = player.position;
            _sampleTimer = 0f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseTrackRadius);

        // Muestra la dirección predicha del jugador
        if (Application.isPlaying && player != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(player.position, _playerVelocity);
        }
    }
}