using UnityEngine;

public class BubbleButton : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private string playerTag = "Player";

    [Header("Visual")]
    [SerializeField] private Transform buttonTop;
    [SerializeField] private float pressDepth = 0.1f;

    [Header("Reset")]
    [SerializeField] private float resetDelay = 2f;

    // ── NUEVO ────────────────────────────────────────────────────────────────
    [Header("Win")]
    [Tooltip("Referencia al GameObject del PJ para quitarle el daño del agua.")]
    [SerializeField] private GameObject player;
    // ─────────────────────────────────────────────────────────────────────────

    private BubbleSpawner _spawner;
    private Vector3       _originalTopPos;
    private bool          _pressed;

    private void Awake()
    {
        _spawner = FindFirstObjectByType<BubbleSpawner>();

        if (_spawner == null)
            Debug.LogWarning("[BubbleButton] No se encontró BubbleSpawner.", this);

        if (buttonTop != null)
            _originalTopPos = buttonTop.localPosition;

        // Auto-buscar PJ si no se asignó en el Inspector
        if (player == null)
        {
            GameObject found = GameObject.FindWithTag(playerTag);
            if (found != null) player = found;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_pressed) return;
        if (!other.CompareTag(playerTag)) return;
        Press();
    }

    private void Press()
    {
        _pressed = true;

        // Hundir visualmente
        if (buttonTop != null)
            buttonTop.localPosition = _originalTopPos - new Vector3(0f, pressDepth, 0f);

        // Desactivar spawner
        if (_spawner != null)
            _spawner.Deactivate();

        // Explotar burbujas
        BubbleLife[] activeBubbles = FindObjectsByType<BubbleLife>(FindObjectsSortMode.None);
        foreach (BubbleLife bubble in activeBubbles)
            bubble.ForceExpire();

        // ── NUEVO: desactivar daño del agua + ganar ──────────────────────────
        if (player != null)
        {
            player.GetComponent<HealthSystem>()?.SetInvincible(true);
            Debug.Log("[BubbleButton] Daño de agua desactivado.");
        }

        WinGame();
        // ─────────────────────────────────────────────────────────────────────

        Debug.Log("[BubbleButton] Botón presionado.");

        if (resetDelay > 0f)
            Invoke(nameof(ResetButton), resetDelay);
    }

    private void ResetButton()
    {
        _pressed = false;
        if (buttonTop != null)
            buttonTop.localPosition = _originalTopPos;
    }

    // ── NUEVO ────────────────────────────────────────────────────────────────
    private void WinGame()
    {
        Debug.Log("[BubbleButton] ¡Ganaste!");
        // Aquí puedes agregar después: cargar escena de victoria,
        // mostrar UI de win, detener el tiempo, etc.
        // Ejemplo: SceneManager.LoadScene("WinScreen");
        // Ejemplo: Time.timeScale = 0f;
    }
    // ─────────────────────────────────────────────────────────────────────────

    private void OnDisable()
    {
        CancelInvoke(nameof(ResetButton));
    }
}