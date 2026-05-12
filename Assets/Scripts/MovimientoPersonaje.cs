using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 1f;
    public float velocidadRotacion = 5f;
    public Animator animator;

    [Header("Salto")]
    public float fuerzaSalto = 1.5f;
    public float gravedad = -20f; // Antes era -9.81, auméntala para caer más rápido
    public float anclaSuelo = -5f;

    private CharacterController controller;
    private Vector3 velocidadGravedad;
    private Transform camTransform;
    private float previousYaw;
    private float currentYaw;
    private bool moving;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        camTransform = Camera.main ? Camera.main.transform : null;
        previousYaw = transform.eulerAngles.y;

        if (animator == null)
            Debug.LogWarning("PlayerMovement: Animator no asignado. Las animaciones no se actualizarán.");
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (camTransform == null && Camera.main != null)
            camTransform = Camera.main.transform;

        Vector3 frente = Vector3.forward;
        Vector3 derecha = Vector3.right;

        if (camTransform != null)
        {
            frente = new Vector3(camTransform.forward.x, 0, camTransform.forward.z).normalized;
            derecha = new Vector3(camTransform.right.x, 0, camTransform.right.z).normalized;
        }

        Vector3 direccion = frente * vertical + derecha * horizontal;
        Vector3 movimiento = direccion.normalized * velocidad;

        if (controller.isGrounded && velocidadGravedad.y < 0)
            velocidadGravedad.y = anclaSuelo;

        if (Input.GetButtonDown("Jump") && controller.isGrounded)
            velocidadGravedad.y = Mathf.Sqrt(fuerzaSalto * -2f * gravedad);

        velocidadGravedad.y += gravedad * Time.deltaTime;
        controller.Move((movimiento + velocidadGravedad) * Time.deltaTime);

        if (direccion.magnitude > 0.1f)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo,
                                                  velocidadRotacion * Time.deltaTime);
        }

        moving = direccion.magnitude > 0.1f;
        float currentEulerYaw = transform.eulerAngles.y;
        currentYaw = Mathf.DeltaAngle(previousYaw, currentEulerYaw);
        previousYaw = currentEulerYaw;

        if (animator != null)
        {
            animator.SetFloat("Speed", direccion.magnitude);
            animator.SetBool("IsJumping", !controller.isGrounded);
            animator.SetBool("IsGrounded", controller.isGrounded);
            animator.SetFloat("Horizontal", horizontal);
            animator.SetFloat("Vertical", vertical);
        }
    }

    public bool isMoving()
    {
        return moving;
    }

    public float CurrentYaw
    {
        get { return currentYaw; }
    }
}