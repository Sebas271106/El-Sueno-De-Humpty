using UnityEngine;

public class CameraTP : MonoBehaviour
{
    [Header("Camera SetUp")]
    public Transform player;
    public Transform cameraTarget;
    public Vector3 shoulderOffset = new Vector3(0.3f, 1.7f, -2f);
    public float followSpeed = 5f;
    public float rotationSpeed = 5f;
    public float mouseSensitivity = 0.5f;

    [Header("Orbita")]
    public float minPitch = -20f;
    public float maxPitch = 60f;

    private float yaw;
    private float pitch;
    private PlayerMovement playerMovement;
    private Transform mainCamera;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("CameraTP: player Transform is not assigned.");
            enabled = false;
            return;
        }

        playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogWarning("CameraTP: PlayerMovement component not found on player. Camera will use mouse input for rotation.");
        }

        if (Camera.main == null)
        {
            Debug.LogError("CameraTP: No Camera tagged as MainCamera found in the scene.");
            enabled = false;
            return;
        }

        mainCamera = Camera.main.transform;

        if (cameraTarget == null)
        {
            cameraTarget = player;
        }

        Cursor.lockState = CursorLockMode.Locked;
    }   

    void LateUpdate()
    {
        HandleInput();
        UpdateCameraPosition();
    }

    void HandleInput()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        if (playerMovement != null && playerMovement.isMoving())
        {
            yaw += playerMovement.CurrentYaw;
        }
        else
        {
            yaw += mouseX * rotationSpeed;
        }

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    void UpdateCameraPosition()
    {
        Quaternion rotacion = Quaternion.Euler(pitch, yaw, 0);
        Vector3 posicionObjetivo = player.position + rotacion * shoulderOffset;

        mainCamera.position = Vector3.Lerp(mainCamera.position, posicionObjetivo, followSpeed * Time.deltaTime);
        mainCamera.LookAt(cameraTarget);
    }
}