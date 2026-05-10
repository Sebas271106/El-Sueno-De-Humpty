using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 2f;
    public float velocidadRotacion = 10f;

    [Header("Salto")]
    public float fuerzaSalto = 2f;
    public float gravedad = -20f; // Antes era -9.81, auméntala para caer más rápido

    private CharacterController controller;
    private Vector3 velocidadGravedad;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical   = Input.GetAxis("Vertical");

        Transform cam = Camera.main.transform;
        Vector3 frente = new Vector3(cam.forward.x, 0, cam.forward.z).normalized;
        Vector3 derecha = new Vector3(cam.right.x,   0, cam.right.z).normalized;

        Vector3 direccion = frente * vertical + derecha * horizontal;

        if (controller.isGrounded && velocidadGravedad.y < 0)
            velocidadGravedad.y = -5f; // Antes era -2, esto ancla mejor al suelo

        if (Input.GetButtonDown("Jump") && controller.isGrounded)
            velocidadGravedad.y = Mathf.Sqrt(fuerzaSalto * -2f * gravedad);

        velocidadGravedad.y += gravedad * Time.deltaTime;
        controller.Move((direccion * velocidad + velocidadGravedad) * Time.deltaTime);

        if (direccion.magnitude > 0.1f)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo,
                                                   velocidadRotacion * Time.deltaTime);
        }
    }
}