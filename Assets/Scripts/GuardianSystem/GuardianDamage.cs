using UnityEngine;

public class GuardianDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private string playerTag = "Player";

    private GuardianController _controller;

    private void Awake()
    {
        _controller = GetComponent<GuardianController>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!hit.collider.CompareTag(playerTag)) return;
        DamagePlayer(hit.collider);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag(playerTag)) return;
        DamagePlayer(collision.collider);
    }

    private void DamagePlayer(Collider playerCollider)
    {
        HealthSystem health = playerCollider.GetComponent<HealthSystem>();
        if (health == null)
            health = playerCollider.GetComponentInParent<HealthSystem>();

        if (health == null) return;

        health.LoseLife();
        Debug.Log("[GuardianDamage] Jugador tocó al gato — pierde una vida");
    }
}