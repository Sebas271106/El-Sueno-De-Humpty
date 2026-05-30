using UnityEngine;
using UnityEngine.Events;

public class BathZone : MonoBehaviour
{
    [Header("Zone Settings")]
    [SerializeField] private float zoneRadius = 0.08f;
    [SerializeField] private string playerTag = "Player";

    [Header("Events")]
    [SerializeField] private UnityEvent onPlayerEntered;
    [SerializeField] private UnityEvent onPlayerExited;

    private bool _playerInZone;

    public bool IsPlayerInZone => _playerInZone;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        _playerInZone = true;
        Debug.Log("[BathZone] PJ en zona de batalla");
        onPlayerEntered?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        _playerInZone = false;
        Debug.Log("[BathZone] PJ salió de zona de batalla");
        onPlayerExited?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.3f);
        Gizmos.DrawSphere(transform.position, zoneRadius);
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.8f);
        Gizmos.DrawWireSphere(transform.position, zoneRadius);
    }
}