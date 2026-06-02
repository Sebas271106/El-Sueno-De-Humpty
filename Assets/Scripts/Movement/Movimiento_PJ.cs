// Movimiento_PJ.cs
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Movimiento_PJ : MonoBehaviour
{
    [Header("Velocidades")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float rotationSpeed = 10f;

    [Header("Salto")]
    public float jumpHeight = 1.5f;
    public float gravity = -25f;
    public float fallMultiplier = 2.5f;
    public float jumpCooldownTime = 0.15f;

    [Header("Suavizado de movimiento")]
    public float acceleration = 8f;
    public float deceleration = 12f;

    [Header("Camara")]
    public Transform cameraTransform;

    [Header("Daño por caída")]
    public float fallDamageThreshold = 20f; // velocidad mínima de caída para recibir daño

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private Vector3 currentMoveVelocity;
    private bool isGrounded;
    private bool wasGrounded;
    private float jumpCooldownCounter;
    private float peakFallSpeed = 0f; // velocidad máxima de caída registrada

    private float boostMultiplier = 1f;
    private float boostTimer = 0f;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsJumpingHash = Animator.StringToHash("IsJumping");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (boostTimer > 0f)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f) boostMultiplier = 1f;
        }

        jumpCooldownCounter -= Time.deltaTime;

        wasGrounded = isGrounded;
        isGrounded = jumpCooldownCounter <= 0f && controller.isGrounded;

        // Registra la velocidad máxima de caída mientras está en el aire
        if (!isGrounded && velocity.y < 0f)
            peakFallSpeed = Mathf.Max(peakFallSpeed, Mathf.Abs(velocity.y));

        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        // Detecta aterrizaje brusco
        if (!wasGrounded && isGrounded)
        {
            if (peakFallSpeed >= fallDamageThreshold)
            {
                HealthSystem hs = GetComponent<HealthSystem>();
                if (hs != null)
                {
                    Debug.Log($"[FallDamage] Caída fuerte detectada — velocidad: {peakFallSpeed:F1}");
                    hs.LoseLife();
                }
            }
            peakFallSpeed = 0f; // resetea siempre al aterrizar
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f; right.y = 0f;
        forward.Normalize(); right.Normalize();

        Vector3 inputDirection = (forward * v + right * h).normalized;
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float targetSpeed = inputDirection.magnitude > 0.1f
                            ? (isRunning ? runSpeed : walkSpeed) : 0f;

        Vector3 targetVelocity = inputDirection * targetSpeed;
        float smoothFactor = inputDirection.magnitude > 0.1f ? acceleration : deceleration;

        currentMoveVelocity = Vector3.Lerp(
            currentMoveVelocity, targetVelocity, smoothFactor * Time.deltaTime);

        controller.Move(currentMoveVelocity * Time.deltaTime);

        if (currentMoveVelocity.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(currentMoveVelocity);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && velocity.y <= 0f)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * boostMultiplier * -2f * gravity);
            jumpCooldownCounter = jumpCooldownTime;
            animator.SetBool(IsJumpingHash, true);
            peakFallSpeed = 0f; // resetea al saltar para no contar el salto como caída
        }

        if (Input.GetKeyUp(KeyCode.Space) && velocity.y > 0f)
            velocity.y *= 0.5f;

        if (velocity.y < 0f)
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        else
            velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        float speedNormalized = currentMoveVelocity.magnitude / runSpeed;
        animator.SetFloat(SpeedHash, speedNormalized, 0.1f, Time.deltaTime);
        animator.SetBool(IsGroundedHash, isGrounded);

        if (!wasGrounded && isGrounded)
            animator.SetBool(IsJumpingHash, false);
    }

    public void ApplyBoost(float multiplier, float duration)
    {
        boostMultiplier = multiplier;
        boostTimer = duration;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        FakePlatform fake = hit.gameObject.GetComponent<FakePlatform>();
        if (fake != null) fake.TriggerDestroy();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.2f);
    }
}