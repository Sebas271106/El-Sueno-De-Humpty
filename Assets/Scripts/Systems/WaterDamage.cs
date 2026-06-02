using UnityEngine;

public class WaterDamage : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        HealthSystem health = other.GetComponent<HealthSystem>();
        if (health != null)
            health.LoseLife();
    }

    // Por si el PJ se queda dentro (ej: cae lento)
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        HealthSystem health = other.GetComponent<HealthSystem>();
        if (health != null)
            health.LoseLife(); // El cooldown de 1s en LoseLife evita spam
    }
}