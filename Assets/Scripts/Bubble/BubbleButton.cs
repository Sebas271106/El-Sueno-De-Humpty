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

    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private WaterDamage waterDamage;

    private BubbleSpawner _spawner;
    private Vector3 _originalTopPos;
    private bool _pressed;

    private void Awake()
    {
        _spawner = FindFirstObjectByType<BubbleSpawner>();

        if (buttonTop != null)
            _originalTopPos = buttonTop.localPosition;

        if (player == null)
        {
            GameObject found = GameObject.FindWithTag(playerTag);

            if (found != null)
                player = found;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_pressed)
            return;

        if (!other.CompareTag(playerTag))
            return;

        Press();
    }

    private void Press()
    {
        _pressed = true;

        // Hundir botón
        if (buttonTop != null)
        {
            buttonTop.localPosition =
                _originalTopPos - new Vector3(0f, pressDepth, 0f);
        }

        // Desactivar spawner
        if (_spawner != null)
            _spawner.Deactivate();

        // Explotar burbujas existentes
        BubbleLife[] activeBubbles =
            FindObjectsByType<BubbleLife>(FindObjectsSortMode.None);

        foreach (BubbleLife bubble in activeBubbles)
        {
            bubble.ForceExpire();
        }

        // Hacer que el agua sea la meta
        if (waterDamage != null)
        {
            waterDamage.EnableVictory();
            Debug.Log("[BubbleButton] EnableVictory llamado");
        }
        else
        {
            Debug.LogError("[BubbleButton] waterDamage es NULL — asígnalo en el Inspector");
        }

        Debug.Log("Botón activado. El agua ahora es la meta.");

        if (resetDelay > 0f)
            Invoke(nameof(ResetButton), resetDelay);
    }

    private void ResetButton()
    {
        _pressed = false;

        if (buttonTop != null)
            buttonTop.localPosition = _originalTopPos;
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(ResetButton));
    }
}