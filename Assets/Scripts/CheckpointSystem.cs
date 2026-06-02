using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    [Header("Posición inicial del nivel")]
    public Transform startPosition;

    private Vector3 lastCheckpointPosition;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        lastCheckpointPosition = startPosition != null
            ? startPosition.position
            : transform.position;
    }

    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpointPosition = position;
    }

    public void RespawnAtLastCheckpoint()
    {
        // Solo teleporta, NO resetea checkpoints
        Teleport(lastCheckpointPosition);
    }

    public void RespawnAtStart()
    {
        // Resetea la posición al inicio
        lastCheckpointPosition = startPosition != null
            ? startPosition.position
            : Vector3.zero;

        Teleport(lastCheckpointPosition);
    }

    void Teleport(Vector3 position)
    {
        controller.enabled = false;
        transform.position = position;
        controller.enabled = true;

        FakePlatform[] fakePlatforms = FindObjectsByType<FakePlatform>(FindObjectsSortMode.None);
        foreach (FakePlatform platform in fakePlatforms)
            platform.ResetPlatform();
    }
}