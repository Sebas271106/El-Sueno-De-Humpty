using UnityEngine;

public class CamaraTP : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform objetivo;          // Arrastra aquí tu PJ

    [Header("Posición")]
    public float distancia  = 5f;
    public float altura     = 2f;
    public float suavizado  = 5f;       // Qué tan suave sigue al PJ

    [Header("Rotación con mouse")]
    public float sensibilidadX = 3f;
    public float sensibilidadY = 2f;
    public float limiteArribaAbajo = 30f; // Ángulo máximo vertical

    private float anguloX = 0f;
    private float anguloY = 0f;

    void Start()
    {
        // Ocultar cursor (quita esta línea si no lo quieres)
        Cursor.lockState = CursorLockMode.Locked;

        Vector3 anglesIniciales = transform.eulerAngles;
        anguloX = anglesIniciales.y;
        anguloY = anglesIniciales.x;
    }

    void LateUpdate()
    {
        if (objetivo == null) return;

        // Leer movimiento del mouse
        anguloX += Input.GetAxis("Mouse X") * sensibilidadX;
        anguloY -= Input.GetAxis("Mouse Y") * sensibilidadY;
        anguloY  = Mathf.Clamp(anguloY, -limiteArribaAbajo, limiteArribaAbajo);

        // Calcular rotación y posición deseada
        Quaternion rotacion = Quaternion.Euler(anguloY, anguloX, 0f);
        Vector3 posicionObjetivo = objetivo.position + Vector3.up * altura;
        Vector3 posicionDeseada  = posicionObjetivo - rotacion * Vector3.forward * distancia;

        // Mover suavemente
        transform.position = Vector3.Lerp(transform.position, posicionDeseada,
                                           suavizado * Time.deltaTime);
        transform.LookAt(posicionObjetivo);
    }
}