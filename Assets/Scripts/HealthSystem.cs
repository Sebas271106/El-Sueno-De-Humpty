using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int maxLives = 3;
    private int currentLives;
    private bool isDead = false;
    private float respawnCooldown = 0f;

    private CheckpointSystem checkpointSystem;
    private Movimiento_PJ playerMovement;

    void Start()
    {
        currentLives = maxLives;
        checkpointSystem = GetComponent<CheckpointSystem>();
        playerMovement = GetComponent<Movimiento_PJ>();
    }

    public void LoseLife()
    {
        // Bloquea si está muerto o si el cooldown no ha pasado
        if (isDead) return;
        if (Time.time < respawnCooldown) return;

        respawnCooldown = Time.time + 1f;
        currentLives--;
        Debug.Log($"Vidas actuales: {currentLives}/{maxLives}");

        if (currentLives <= 0)
        {
            isDead = true;
            checkpointSystem.RespawnAtStart();
            currentLives = maxLives;
            isDead = false;
        }
        else
        {
            checkpointSystem.RespawnAtLastCheckpoint();
        }
    }

    public int GetCurrentLives() => currentLives;
}