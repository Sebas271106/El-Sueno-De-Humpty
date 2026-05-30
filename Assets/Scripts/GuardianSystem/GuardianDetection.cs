using UnityEngine;

public class GuardianDetection : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRadius = 8f;
    [SerializeField] private float loseTrackRadius = 12f;

    private bool _isPlayerDetected;
    private float _distanceToPlayer = float.MaxValue;

    public bool IsPlayerDetected => _isPlayerDetected;
    public float DistanceToPlayer => _distanceToPlayer;
    public Transform PlayerTransform => player;

    private void Update()
    {
        if (player == null) return;
        UpdateDetection();
    }

    private void UpdateDetection()
    {
        // Ignora la diferencia en Y — solo calcula distancia en el plano XZ
        Vector3 selfFlat   = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 playerFlat = new Vector3(player.position.x,    0f, player.position.z);

        _distanceToPlayer = Vector3.Distance(selfFlat, playerFlat);

        if (!_isPlayerDetected && _distanceToPlayer <= detectionRadius)
            _isPlayerDetected = true;
        else if (_isPlayerDetected && _distanceToPlayer >= loseTrackRadius)
            _isPlayerDetected = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseTrackRadius);
    }
}