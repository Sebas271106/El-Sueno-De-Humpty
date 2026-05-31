using UnityEngine;

[RequireComponent(typeof(BubbleMovement))]
public class BubbleTrap : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private string playerTag = "Player";

    [Header("Capture Settings")]
    [SerializeField, Range(0.5f, 5f)] private float captureDelay = 1.5f;
    [SerializeField] private Vector3 playerOffset = Vector3.zero;

    // ─────────────────────────────────────────────────────────────────────────────
    // Estado interno
    // ─────────────────────────────────────────────────────────────────────────────

    private BubbleMovement      _movement;
    private BubbleLife          _life;
    private Transform           _capturedPlayer;
    private CharacterController _capturedCC;
    private HealthSystem        _playerHealth;      // ← referencia directa
    private Behaviour[]         _playerScripts;
    private bool                _hasPlayer;
    private float               _captureTimer;

    // ─────────────────────────────────────────────────────────────────────────────
    // Unity
    // ─────────────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        _movement = GetComponent<BubbleMovement>();
        _life     = GetComponent<BubbleLife>();
    }

    private void OnEnable()
    {
        _hasPlayer      = false;
        _capturedPlayer = null;
        _capturedCC     = null;
        _playerHealth   = null;
        _captureTimer   = 0f;
    }

    private void Update()
    {
        if (!_hasPlayer) return;

        _captureTimer += Time.deltaTime;

        // Pegar al jugador al centro de la burbuja
        if (_capturedPlayer != null)
        {
            _capturedPlayer.position = transform.position + playerOffset;
        }

        if (_captureTimer >= captureDelay)
        {
            TriggerDeath();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasPlayer) return;
        if (!other.CompareTag(playerTag)) return;

        CharacterController cc = other.GetComponent<CharacterController>();
        if (cc == null) return;

        HealthSystem health = other.GetComponent<HealthSystem>();
        if (health == null)
        {
            Debug.LogWarning("[BubbleTrap] El jugador no tiene HealthSystem.", this);
            return;
        }

        CapturePlayer(other.transform, cc, health);
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // Privados
    // ─────────────────────────────────────────────────────────────────────────────

    private void CapturePlayer(Transform player, CharacterController cc, HealthSystem health)
    {
        _hasPlayer      = true;
        _capturedPlayer = player;
        _capturedCC     = cc;
        _playerHealth   = health;
        _captureTimer   = 0f;

        // Deshabilitar scripts de movimiento/input
        _playerScripts = player.GetComponents<Behaviour>();
        foreach (Behaviour b in _playerScripts)
        {
            if (b == null) continue;
            string typeName = b.GetType().Name;
            if (typeName.Contains("Controller") ||
                typeName.Contains("Movement")   ||
                typeName.Contains("Input")      ||
                typeName.Contains("Motor"))
            {
                b.enabled = false;
            }
        }

        // Deshabilitar CC para mover por transform
        _capturedCC.enabled = false;

        // SetTrapped(false) para que BubbleMovement siga subiendo
        _movement.SetTrapped(false);
        if (_life != null) _life.PauseExpiration(true);

        Debug.Log($"[BubbleTrap] Jugador atrapado por {gameObject.name}");
    }

    private void TriggerDeath()
    {
        if (_capturedPlayer == null) return;

        // Re-habilitar CC antes del respawn para que Teleport() funcione
        if (_capturedCC != null)
            _capturedCC.enabled = true;

        // Re-habilitar scripts de movimiento
        if (_playerScripts != null)
        {
            foreach (Behaviour b in _playerScripts)
            {
                if (b != null) b.enabled = true;
            }
        }

        // ► Perder vida y volver al checkpoint
        if (_playerHealth != null)
            _playerHealth.LoseLife();

        // Limpiar estado
        _hasPlayer      = false;
        _capturedPlayer = null;
        _capturedCC     = null;
        _playerHealth   = null;
        _movement.SetTrapped(false);

        if (_life != null) _life.ForceExpire();
    }
}