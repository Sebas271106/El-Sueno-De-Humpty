using UnityEngine;

public class DeathZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("DeathZone tocada por: " + other.gameObject.name);

        if (!other.CompareTag("Player")) return;

        HealthSystem hs = other.GetComponent<HealthSystem>();
        if (hs != null)
            hs.LoseLife();

    }

}