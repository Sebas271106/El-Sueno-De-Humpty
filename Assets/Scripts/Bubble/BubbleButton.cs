using UnityEngine;

public class BubbleButton : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private string playerTag = "Player";

    [Header("Visual")]
    [Tooltip("Transform de la parte que se hunde al pisar el botón.")]
    [SerializeField] private Transform buttonTop;
    [Tooltip("Cuánto se hunde el botón al pisarlo.")]
    [SerializeField] private float pressDepth = 0.1f;

    // ─────────────────────────────────────────────────────────────────────────────
    // Estado interno
    // ─────────────────────────────────────────────────────────────────────────────

    private BubbleSpawner _spawner;
    private Vector3       _originalTopPos;
    private bool          _pressed;

    // ─────────────────────────────────────────────────────────────────────────────
    // Unity
    // ─────────────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        _spawner = FindFirstObjectByType<BubbleSpawner>();

        if (_spawner == null)
            Debug.LogWarning("[BubbleButton] No se encontró BubbleSpawner en la escena.", this);

        if (buttonTop != null)
            _originalTopPos = buttonTop.localPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_pressed) return;
        if (!other.CompareTag(playerTag)) return;

        Press();
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // Privados
    // ─────────────────────────────────────────────────────────────────────────────

    private void Press()
    {
        _pressed = true;

        // Hundir visualmente el botón
        if (buttonTop != null)
            buttonTop.localPosition = _originalTopPos - new Vector3(0f, pressDepth, 0f);

        // Desactivar spawner
        if (_spawner != null)
            _spawner.Deactivate();

        // Explotar todas las burbujas activas
        BubbleLife[] activeBubbles = FindObjectsByType<BubbleLife>(FindObjectsSortMode.None);
        foreach (BubbleLife bubble in activeBubbles)
            bubble.ForceExpire();

        Debug.Log("[BubbleButton] Burbujas desactivadas.");
    }
}