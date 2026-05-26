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

    [Header("Suelo")]
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    [Header("Camara")]
    public Transform cameraTransform;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private Vector3 currentMoveVelocity;
    private bool isGrounded;
    private float jumpCooldownCounter;

    private float boostMultiplier = 1f;
    private float boostTimer = 0f;

    // Hashes para mejor rendimiento
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
        // --- Boost timer ---
        if (boostTimer > 0f)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f)
                boostMultiplier = 1f;
        }

        jumpCooldownCounter -= Time.deltaTime;

        isGrounded = jumpCooldownCounter <= 0f && Physics.CheckSphere(
            groundCheck.position, groundDistance, groundMask
        );

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // --- Input de movimiento ---
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 inputDirection = (forward * v + right * h).normalized;
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float targetSpeed = inputDirection.magnitude > 0.1f
                                ? (isRunning ? runSpeed : walkSpeed)
                                : 0f;

        Vector3 targetVelocity = inputDirection * targetSpeed;
        float smoothFactor = inputDirection.magnitude > 0.1f
                                 ? acceleration
                                 : deceleration;

        currentMoveVelocity = Vector3.Lerp(
            currentMoveVelocity, targetVelocity, smoothFactor * Time.deltaTime
        );

        controller.Move(currentMoveVelocity * Time.deltaTime);

        if (currentMoveVelocity.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(currentMoveVelocity);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, targetRot, rotationSpeed * Time.deltaTime
            );
        }

        // --- Salto con boost ---
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && velocity.y <= 0f)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * boostMultiplier * -2f * gravity);
            jumpCooldownCounter = jumpCooldownTime;
            animator.SetBool(IsJumpingHash, true);
            Debug.Log("SALTANDO - IsJumping: " + animator.GetBool(IsJumpingHash));
        }

        if (Input.GetKeyUp(KeyCode.Space) && velocity.y > 0)
            velocity.y *= 0.5f;

        if (velocity.y < 0)
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        else
            velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        // --- Actualizar Animator ---
        float speedNormalized = currentMoveVelocity.magnitude / runSpeed;
        animator.SetFloat(SpeedHash, speedNormalized, 0.1f, Time.deltaTime);
        animator.SetBool(IsGroundedHash, isGrounded);

        if (isGrounded && velocity.y < -0.1f && animator.GetBool(IsJumpingHash))
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
        if (fake != null)
            fake.TriggerDestroy();
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}