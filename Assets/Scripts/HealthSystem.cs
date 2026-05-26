using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int maxLives = 3;
    private int currentLives;
    private bool isDead = false;

    private CheckpointSystem checkpointSystem;
    private Movimiento_PJ playerMovement;

    void Start()
    {
        currentLives = maxLives;
        checkpointSystem = GetComponent<CheckpointSystem>();
        playerMovement   = GetComponent<Movimiento_PJ>();
    }

    public void LoseLife()
    {
        if (isDead) return;

        currentLives--;
        Debug.Log($"Vidas actuales: {currentLives}/{maxLives}");

        if (currentLives <= 0)
        {
            // Sin vidas — reinicia desde el principio
            Debug.Log("Sin vidas: reiniciando al inicio");
            isDead = true;
            checkpointSystem.RespawnAtStart();
            currentLives = maxLives;
            isDead = false;
        }
        else
        {
            Debug.Log("Volviendo al último checkpoint");
            checkpointSystem.RespawnAtLastCheckpoint();
        }
    }

    public int GetCurrentLives() => currentLives;
}