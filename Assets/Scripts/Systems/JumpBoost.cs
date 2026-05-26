using UnityEngine;

public class JumpBoostPickup : MonoBehaviour
{
    [Header("Boost")]
    public float multiplier = 2f;   // 2 = duplica, 3 = triplica
    public float duration = 5f;     // segundos que dura

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Movimiento_PJ movement = other.GetComponent<Movimiento_PJ>();
        if (movement != null)
            movement.ApplyBoost(multiplier, duration);

        Destroy(gameObject);
    }
}