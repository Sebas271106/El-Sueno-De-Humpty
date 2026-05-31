using UnityEngine;

/// <summary>
/// Controla la vida útil de cada burbuja.
/// La desactiva y la devuelve al pool cuando:
/// - Supera la duración máxima configurada.
/// - Alcanza la altura máxima configurada.
/// - BubbleTrap llama ForceExpire() tras ejecutar la muerte.
/// </summary>
public class BubbleLife : MonoBehaviour
{
    // ─────────────────────────────────────────────────────────────────────────────
    // Inspector
    // ─────────────────────────────────────────────────────────────────────────────

    [Header("Lifetime")]
    [Tooltip("Duración mínima de la burbuja en segundos (0 = sin límite de tiempo).")]
    [SerializeField, Range(0f, 30f)] private float minLifetime = 4f;

    [Tooltip("Duración máxima de la burbuja en segundos (0 = sin límite de tiempo).")]
    [SerializeField, Range(0f, 30f)] private float maxLifetime = 8f;

    [Header("Height Limit")]
    [Tooltip("Altura máxima absoluta (world Y) a la que puede llegar la burbuja. " +
             "0 o negativo = sin límite de altura.")]
    [SerializeField] private float maxHeight = 20f;

    // ─────────────────────────────────────────────────────────────────────────────
    // Estado interno
    // ─────────────────────────────────────────────────────────────────────────────

    private BubblePool    _pool;
    private BubbleSpawner _spawner;
    private float         _elapsed;
    private float         _lifetime;
    private bool          _paused;
    private bool          _expired;

    // ─────────────────────────────────────────────────────────────────────────────
    // Unity
    // ─────────────────────────────────────────────────────────────────────────────

    private void Update()
    {
        if (_paused || _expired) return;

        _elapsed += Time.deltaTime;

        // Verificar duración
        bool lifetimeExpired = _lifetime > 0f && _elapsed >= _lifetime;

        // Verificar altura
        bool heightReached = maxHeight > 0f && transform.position.y >= maxHeight;

        if (lifetimeExpired || heightReached)
        {
            Expire();
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // API Pública
    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Llamado por BubbleSpawner al sacar esta burbuja del pool.
    /// Guarda las referencias necesarias y reinicia el temporizador.
    /// </summary>
    public void Initialize(BubbleSpawner spawner, BubblePool pool)
    {
        _spawner  = spawner;
        _pool     = pool;
        _elapsed  = 0f;
        _paused   = false;
        _expired  = false;
        _lifetime = Random.Range(minLifetime, maxLifetime);
    }

    /// <summary>
    /// Pausa o reanuda el temporizador (usado por BubbleTrap durante la captura).
    /// </summary>
    public void PauseExpiration(bool pause) => _paused = pause;

    /// <summary>
    /// Fuerza el retorno inmediato al pool (llamado por BubbleTrap tras la muerte).
    /// </summary>
    public void ForceExpire() => Expire();

    // ─────────────────────────────────────────────────────────────────────────────
    // Privados
    // ─────────────────────────────────────────────────────────────────────────────

    private void Expire()
    {
        if (_expired) return; // Evitar llamadas dobles
        _expired = true;

        // Notificar al spawner para actualizar el conteo de activas
        _spawner?.OnBubbleReturned();

        // Devolver al pool
        _pool?.Return(gameObject);
    }
}