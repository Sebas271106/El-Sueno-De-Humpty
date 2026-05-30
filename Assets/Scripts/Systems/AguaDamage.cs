using UnityEngine;

public class AguaDamage : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        HealthSystem health = other.GetComponent<HealthSystem>();
        if (health != null)
            health.LoseLife();
    }
}