using System.Collections.Generic;
using UnityEngine;

public class BubblePool : MonoBehaviour
{
    [Header("Pool Configuration")]
    [SerializeField] private GameObject bubblePrefab;
    [SerializeField] private int initialPoolSize = 30;
    [SerializeField] private bool allowGrowth = true;

    private readonly Queue<GameObject> _available = new Queue<GameObject>();
    private readonly List<GameObject>  _all       = new List<GameObject>();

    // ← Escala hardcodeada, modifica estos valores según necesites
    private readonly Vector3 _bubbleScale = new Vector3(0.8f, 0.8f, 0.8f);

    // ─────────────────────────────────────────────────────────────────────────────
    // Unity
    // ─────────────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (bubblePrefab == null)
        {
            Debug.LogError("[BubblePool] bubblePrefab no asignado.", this);
            return;
        }

        Prewarm(initialPoolSize);
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // API Pública
    // ─────────────────────────────────────────────────────────────────────────────

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        GameObject bubble;

        if (_available.Count > 0)
        {
            bubble = _available.Dequeue();
        }
        else if (allowGrowth)
        {
            bubble = CreateInstance();
            Debug.LogWarning("[BubblePool] Pool agotado – se creó una instancia nueva. Considera aumentar initialPoolSize.", this);
        }
        else
        {
            return null;
        }

        // Posición y escala ANTES de SetActive para que OnEnable no las pise
        bubble.transform.SetPositionAndRotation(position, rotation);
        bubble.transform.localScale = _bubbleScale;
        bubble.SetActive(true);
        return bubble;
    }

    public void Return(GameObject bubble)
    {
        if (bubble == null) return;
        bubble.SetActive(false);
        _available.Enqueue(bubble);
    }

    public int AvailableCount => _available.Count;
    public int TotalCount     => _all.Count;

    // ─────────────────────────────────────────────────────────────────────────────
    // Privados
    // ─────────────────────────────────────────────────────────────────────────────

    private void Prewarm(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject b = CreateInstance();
            _available.Enqueue(b);
        }
    }

    private GameObject CreateInstance()
    {
        GameObject b = Instantiate(bubblePrefab, transform);
        b.SetActive(false);
        _all.Add(b);
        return b;
    }
}