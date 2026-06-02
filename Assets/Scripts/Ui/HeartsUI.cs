using UnityEngine;
using TMPro;

public class HeartsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI heartsText;

    private HealthSystem _health;
    private int _lastLives = -1;

    private void Start()
    {
        // Busca el PJ por tag
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            _health = player.GetComponent<HealthSystem>();

        if (_health == null)
            Debug.LogWarning("[HeartsUI] No se encontró HealthSystem en el Player.");

        if (heartsText == null)
            heartsText = GetComponent<TextMeshProUGUI>();

        UpdateHearts();
    }

    private void Update()
    {
        if (_health == null) return;

        // Solo actualiza si cambió para no hacerlo cada frame
        int current = _health.GetCurrentLives();
        if (current != _lastLives)
            UpdateHearts();
    }

    private void UpdateHearts()
    {
        if (_health == null) return;

        int current = _health.GetCurrentLives();
        int max = _health.maxLives;
        _lastLives = current;

        string display = "";
        for (int i = 0; i < max; i++)
        {
            if (i < current)
                display += "<color=red>♥</color> ";
            else
                display += "<color=grey>♡</color> ";
        }

        heartsText.text = display;
    }
}