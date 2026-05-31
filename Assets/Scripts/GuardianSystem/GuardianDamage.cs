using UnityEngine;

public class GuardianDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float damageCooldown = 1.5f;
    [SerializeField] private string playerTag = "Player";

    private float _lastDamageTime;
    private GuardianController _controller;

    private void Awake()
    {
        _controller = GetComponent<GuardianController>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!hit.collider.CompareTag(playerTag)) return;
        TryDamagePlayer(hit.collider);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag(playerTag)) return;
        TryDamagePlayer(collision.collider);
    }

    private void TryDamagePlayer(Collider playerCollider)
    {
        // Cooldown para no quitar vidas cada frame
        if (Time.time - _lastDamageTime < damageCooldown) return;

        HealthSystem health = playerCollider.GetComponent<HealthSystem>();
        if (health == null)
            health = playerCollider.GetComponentInParent<HealthSystem>();

        if (health == null) return;

        _lastDamageTime = Time.time;
        health.LoseLife();
        Debug.Log("[GuardianDamage] Jugador tocó al gato — pierde una vida");
    }
}