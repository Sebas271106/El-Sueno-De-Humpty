// GuardianBlocker.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GuardianBlocker : MonoBehaviour
{
    [Header("Blocker Settings")]
    [SerializeField] private Transform bathtub;
    [SerializeField] private float moveSpeed     = 6f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float smoothTime    = 0.1f;

    [Header("Prediction Settings")]
    [SerializeField] private float predictionTime  = 0.4f;
    [SerializeField] private float minPlayerSpeed  = 0.3f;
    [SerializeField] private float maxPredictDist  = 4f;

    [Header("Block Position")]
    // Qué fracción del camino jugador→bañera ocupa el guardián (0=jugador, 1=bañera)
    [SerializeField] private float blockFraction = 0.6f;
    // Distancia mínima que mantiene respecto al jugador
    [SerializeField] private float minDistFromPlayer = 1.5f;

    private Rigidbody _rb;
    private GuardianDetection _detection;
    private Vector3 _velocity = Vector3.zero;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotation
                        | RigidbodyConstraints.FreezePositionY;
        _rb.useGravity = false;
        _detection = GetComponent<GuardianDetection>();
    }

    private void FixedUpdate()
    {
        if (bathtub == null || _detection == null) return;
        if (_detection.PlayerTransform == null) return;

        MoveToBlockPosition();
        FacePlayer();
    }

    private void MoveToBlockPosition()
    {
        Vector3 playerPos  = _detection.PlayerTransform.position;
        Vector3 playerVel  = _detection.PlayerVelocity;
        Vector3 bathtubPos = new Vector3(bathtub.position.x, transform.position.y, bathtub.position.z);

        // Predice posición futura del jugador
        Vector3 predictedPos = playerPos;
        if (playerVel.magnitude > minPlayerSpeed)
        {
            Vector3 delta = playerVel * predictionTime;
            if (delta.magnitude > maxPredictDist)
                delta = delta.normalized * maxPredictDist;
            predictedPos = playerPos + delta;
        }
        predictedPos.y = transform.position.y;

        // Interpola entre jugador predicho y bañera según blockFraction
        // blockFraction = 0.5 → exactamente en el medio
        // blockFraction = 0.7 → más cerca de la bañera
        Vector3 blockTarget = Vector3.Lerp(predictedPos, bathtubPos, blockFraction);

        // Si el target queda demasiado cerca del jugador, lo empuja más hacia la bañera
        if (Vector3.Distance(blockTarget, predictedPos) < minDistFromPlayer)
        {
            Vector3 dir = (bathtubPos - predictedPos).normalized;
            blockTarget = predictedPos + dir * minDistFromPlayer;
        }

        blockTarget.y = transform.position.y;

        Vector3 smoothedPos = Vector3.SmoothDamp(
            transform.position, blockTarget, ref _velocity, smoothTime, moveSpeed
        );
        _rb.MovePosition(smoothedPos);
    }

    private void FacePlayer()
    {
        Vector3 dirToPlayer = _detection.PlayerTransform.position - transform.position;
        dirToPlayer.y = 0f;
        if (dirToPlayer == Vector3.zero) return;

        Quaternion targetRot = Quaternion.LookRotation(dirToPlayer);
        transform.rotation = Quaternion.Slerp(
            transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || _detection?.PlayerTransform == null || bathtub == null) return;

        Vector3 playerPos  = _detection.PlayerTransform.position;
        Vector3 predicted  = playerPos + _detection.PlayerVelocity * predictionTime;
        Vector3 bathtubPos = bathtub.position;
        Vector3 blockTarget = Vector3.Lerp(predicted, bathtubPos, blockFraction);

        // Jugador → bañera
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(playerPos, bathtubPos);

        // Posición predicha
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(predicted, 0.3f);

        // Posición objetivo del guardián
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(blockTarget, 0.4f);
    }
}