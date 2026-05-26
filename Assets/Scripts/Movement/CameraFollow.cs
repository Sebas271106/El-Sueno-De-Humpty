using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Distancia")]
    public float distance = 6f;
    public float minDistance = 2f;
    public float maxDistance = 12f;
    public float zoomSpeed = 3f;

    [Header("Altura")]
    public float height = 2f;

    [Header("Sensibilidad mouse")]
    public float sensitivityX = 3f;
    public float sensitivityY = 2f;
    public float minPitch = -20f;
    public float maxPitch = 60f;

    [Header("Suavizado")]
    public float smoothSpeed = 10f;

    private float yaw;
    private float pitch = 15f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        // --- Rotación con el mouse ---
        yaw   += Input.GetAxis("Mouse X") * sensitivityX;
        pitch -= Input.GetAxis("Mouse Y") * sensitivityY;
        pitch  = Mathf.Clamp(pitch, minPitch, maxPitch);

        // --- Zoom con la rueda ---
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * zoomSpeed;
        distance  = Mathf.Clamp(distance, minDistance, maxDistance);

        // --- Posición deseada ---
        Quaternion rotation    = Quaternion.Euler(pitch, yaw, 0f);
        Vector3   desiredPos   = target.position 
                                 + Vector3.up * height 
                                 + rotation * Vector3.back * distance;

        // --- Evita que atraviese paredes ---
        RaycastHit hit;
        Vector3 finalPos = desiredPos;
        if (Physics.Linecast(target.position + Vector3.up * height, desiredPos, out hit))
            finalPos = hit.point + hit.normal * 0.2f;

        // --- Aplica posición suavizada ---
        transform.position = Vector3.Lerp(
            transform.position, finalPos, smoothSpeed * Time.deltaTime
        );
        transform.LookAt(target.position + Vector3.up * height);
    }
}
