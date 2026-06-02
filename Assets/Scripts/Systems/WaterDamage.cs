using UnityEngine;

public class WaterDamage : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private bool victoryEnabled = false;
    private bool victoryTriggered = false; // ← nueva bandera

    public void EnableVictory()
    {
        victoryEnabled = true;
    }

    private void TriggerVictory()
    {
        if (victoryTriggered) return;
        victoryTriggered = true;
        WinUI.Instance.ShowWin();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (victoryEnabled)
        {
            TriggerVictory();
            return;
        }

        HealthSystem health = other.GetComponent<HealthSystem>();
        if (health != null) health.LoseLife();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (victoryEnabled)
        {
            TriggerVictory(); // ← cubre el caso de caer ya dentro del agua
            return;
        }

        HealthSystem health = other.GetComponent<HealthSystem>();
        if (health != null) health.LoseLife();
    }
}