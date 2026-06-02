using UnityEngine;

public class DeathZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        HealthSystem hs = other.GetComponent<HealthSystem>();
        if (hs != null)
            hs.LoseLife();
    }
}