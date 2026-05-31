using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GuardianBlocker : MonoBehaviour
{
    [Header("Blocker Settings")]
    [SerializeField] private Transform bathtub;
    [SerializeField] private float blockOffset      = 1.2f;  // distancia gato-bañera
    [SerializeField] private float moveSpeed        = 5f;
    [SerializeField] private float rotationSpeed    = 10f;
    [SerializeField] private float smoothTime       = 0.15f;

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
        // Calcula dirección jugador → bañera en plano XZ
        Vector3 playerPos   = _detection.PlayerTransform.position;
        Vector3 bathtubPos  = bathtub.position;

        Vector3 dirPlayerToBath = (bathtubPos - playerPos);
        dirPlayerToBath.y = 0f;

        if (dirPlayerToBath == Vector3.zero) return;

        dirPlayerToBath.Normalize();

        // Posición objetivo: delante de la bañera, mirando al jugador
        Vector3 blockTarget = new Vector3(
            bathtubPos.x - dirPlayerToBath.x * blockOffset,
            transform.position.y,
            bathtubPos.z - dirPlayerToBath.z * blockOffset
        );

        // Suavizado para evitar teletransporte
        Vector3 smoothedPos = Vector3.SmoothDamp(
            transform.position, blockTarget, ref _velocity,
            smoothTime, moveSpeed
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
}