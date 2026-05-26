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
        Debug.Log($"Checkpoint guardado en {position}");
    }

    public void RespawnAtLastCheckpoint()
    {
        Teleport(lastCheckpointPosition);
    }

    public void RespawnAtStart()
    {
        // Resetea la posición
        lastCheckpointPosition = startPosition != null
            ? startPosition.position
            : Vector3.zero;

        // Resetea todos los checkpoints de la escena
        Checkpoint[] allCheckpoints = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);
        foreach (Checkpoint cp in allCheckpoints)
            cp.ResetCheckpoint();

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