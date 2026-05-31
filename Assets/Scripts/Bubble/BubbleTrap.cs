using UnityEngine;

/// <summary>
/// Detecta colisión con el jugador (CharacterController), lo captura dentro de la burbuja
/// y delega la secuencia de muerte al sistema de juego existente.
/// </summary>
[RequireComponent(typeof(BubbleMovement))]
public class BubbleTrap : MonoBehaviour
{
    // ─────────────────────────────────────────────────────────────────────────────
    // Inspector
    // ─────────────────────────────────────────────────────────────────────────────

    [Header("Detection")]
    [Tooltip("Tag del GameObject del jugador.")]
    [SerializeField] private string playerTag = "Player";

    [Header("Capture Settings")]
    [Tooltip("Tiempo en segundos que la burbuja transporta al jugador antes de ejecutar la muerte.")]
    [SerializeField, Range(0.5f, 5f)] private float captureDelay = 1.5f;

    [Tooltip("Offset de posición del jugador dentro de la burbuja (relativo al centro de la burbuja).")]
    [SerializeField] private Vector3 playerOffset = Vector3.zero;

    [Header("Death Event")]
    [Tooltip("Nombre del método que se llamará en el jugador para ejecutar la secuencia de muerte.")]
    [SerializeField] private string deathMethodName = "Die";

    // ─────────────────────────────────────────────────────────────────────────────
    // Estado interno
    // ─────────────────────────────────────────────────────────────────────────────

    private BubbleMovement      _movement;
    private BubbleLife          _life;

    private Transform           _capturedPlayer;
    private CharacterController _capturedCC;        // ← referencia directa al CC
    private Behaviour[]         _playerScripts;     // scripts de movimiento/input a deshabilitar
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
        _captureTimer   = 0f;
    }

    private void Update()
    {
        if (!_hasPlayer) return;

        _captureTimer += Time.deltaTime;

        // Mantener al jugador pegado a la burbuja usando CharacterController.Move
        if (_capturedPlayer != null && _capturedCC != null)
        {
            Vector3 targetPos  = transform.position + playerOffset;
            Vector3 delta      = targetPos - _capturedPlayer.position;
            // Move respeta el CharacterController y evita atravesar geometría
            _capturedCC.Move(delta);
        }

        if (_captureTimer >= captureDelay)
        {
            TriggerDeath();
        }
    }

    // CharacterController usa OnControllerColliderHit, pero para detectar
    // que la BURBUJA toca al jugador necesitamos OnTriggerEnter en la burbuja.
    // Asegúrate de que el Collider de la burbuja tenga isTrigger = true.
    private void OnTriggerEnter(Collider other)
    {
        if (_hasPlayer) return;
        if (!other.CompareTag(playerTag)) return;

        // Verificar que el jugador tiene CharacterController
        CharacterController cc = other.GetComponent<CharacterController>();
        if (cc == null) return;   // no es nuestro jugador con CC

        CapturePlayer(other.transform, cc);
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // Privados
    // ─────────────────────────────────────────────────────────────────────────────

    private void CapturePlayer(Transform player, CharacterController cc)
    {
        _hasPlayer      = true;
        _capturedPlayer = player;
        _capturedCC     = cc;
        _captureTimer   = 0f;

        // Deshabilitar scripts de movimiento/input del jugador.
        // ► Reemplaza los strings o tipos según los nombres reales de tus scripts.
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

        // El CharacterController en sí también se puede deshabilitar para
        // que no interfiera con el posicionamiento manual.
        // OJO: deshabilitar CC lo quita de la simulación física completamente.
        _capturedCC.enabled = false;

        _movement.SetTrapped(true);
        if (_life != null) _life.PauseExpiration(true);

        Debug.Log($"[BubbleTrap] Jugador (CharacterController) atrapado por {gameObject.name}");
    }

    private void TriggerDeath()
    {
        if (_capturedPlayer == null) return;

        // Re-habilitar CharacterController antes de llamar al sistema de muerte
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

        // ► INTEGRA AQUÍ TU SISTEMA DE MUERTE ◄
        // Opción A – SendMessage:
        if (!string.IsNullOrEmpty(deathMethodName))
        {
            _capturedPlayer.SendMessage(deathMethodName, SendMessageOptions.DontRequireReceiver);
        }

        // Opción B – Referencia directa (más eficiente):
        // var health = _capturedPlayer.GetComponent<PlayerHealth>();
        // if (health != null) health.Die();

        // Opción C – Evento global:
        // GameManager.Instance.OnPlayerDeath?.Invoke();

        // Limpiar estado
        _hasPlayer      = false;
        _capturedPlayer = null;
        _capturedCC     = null;
        _movement.SetTrapped(false);

        if (_life != null) _life.ForceExpire();
    }
}