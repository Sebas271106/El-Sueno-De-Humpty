using UnityEngine;

[RequireComponent(typeof(BubblePool))]
public class BubbleSpawner : MonoBehaviour
{
    [Header("Spawn Area")]
    [SerializeField] private Vector2 spawnAreaHalfExtents = new Vector2(5f, 5f);
    [SerializeField] private float spawnHeightOffset = 0.1f;

    [Header("Spawn Rate")]
    [SerializeField, Range(0.1f, 50f)] private float bubblesPerSecond = 5f;
    [SerializeField, Range(1, 10)] private int bubblesPerBatch = 1;

    [Header("Limits")]
    [SerializeField] private int maxActiveBubbles = 40;

    [Header("Gizmos")]
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private Color gizmoColor = new Color(0f, 0.8f, 1f, 0.25f);

    // ─────────────────────────────────────────────────────────────────────────────
    // Estado interno
    // ─────────────────────────────────────────────────────────────────────────────

    private BubblePool _pool;
    private int        _activeBubbles;
    private float      _spawnTimer;
    private float      _spawnInterval;
    private bool       _deactivated = false; // ← flag de control

    // ─────────────────────────────────────────────────────────────────────────────
    // Unity
    // ─────────────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        _pool          = GetComponent<BubblePool>();
        _spawnInterval = 1f / Mathf.Max(bubblesPerSecond, 0.01f);
    }

    private void Update()
    {
        if (_deactivated) return; // ← corta todo si está desactivado

        _spawnTimer += Time.deltaTime;

        if (_spawnTimer >= _spawnInterval)
        {
            _spawnTimer -= _spawnInterval;
            TrySpawnBatch();
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // API Pública
    // ─────────────────────────────────────────────────────────────────────────────

    public void OnBubbleReturned()
    {
        _activeBubbles = Mathf.Max(0, _activeBubbles - 1);
    }

    public void Deactivate()
    {
        _deactivated = true;
        _spawnTimer  = 0f;
        Debug.Log("[BubbleSpawner] Spawner desactivado.");
    }

    public void Activate()
    {
        _deactivated = false;
        Debug.Log("[BubbleSpawner] Spawner activado.");
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // Privados
    // ─────────────────────────────────────────────────────────────────────────────

    private void TrySpawnBatch()
    {
        for (int i = 0; i < bubblesPerBatch; i++)
        {
            if (maxActiveBubbles > 0 && _activeBubbles >= maxActiveBubbles) break;

            Vector3    spawnPos = GetRandomSpawnPosition();
            GameObject bubble   = _pool.Get(spawnPos, Quaternion.identity);

            if (bubble == null) break;

            BubbleLife life = bubble.GetComponent<BubbleLife>();
            if (life != null) life.Initialize(this, _pool);

            _activeBubbles++;
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(-spawnAreaHalfExtents.x, spawnAreaHalfExtents.x);
        float z = Random.Range(-spawnAreaHalfExtents.y, spawnAreaHalfExtents.y);
        return transform.position + new Vector3(x, spawnHeightOffset, z);
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // Gizmos
    // ─────────────────────────────────────────────────────────────────────────────

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = gizmoColor;
        Vector3 center = transform.position + Vector3.up * spawnHeightOffset;
        Vector3 size   = new Vector3(spawnAreaHalfExtents.x * 2f, 0.05f, spawnAreaHalfExtents.y * 2f);
        Gizmos.DrawCube(center, size);

        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 1f);
        Gizmos.DrawWireCube(center, size);
    }
}