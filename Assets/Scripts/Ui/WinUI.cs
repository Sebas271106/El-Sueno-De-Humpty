using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WinUI : MonoBehaviour
{
    public static WinUI Instance;
    private bool _won = false;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowWin()
    {
        if (_won) return;
        _won = true;

        HealthSystem hs = FindFirstObjectByType<HealthSystem>();
        if (hs != null) hs.SetInvincible(true);

        GameObject canvasGO = new GameObject("WinCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject panel = new GameObject("WinPanel");
        panel.transform.SetParent(canvasGO.transform, false);
        Image panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0f, 0f, 0f, 0.85f);
        RectTransform panelRT = panel.GetComponent<RectTransform>();
        panelRT.anchorMin = Vector2.zero;
        panelRT.anchorMax = Vector2.one;
        panelRT.offsetMin = Vector2.zero;
        panelRT.offsetMax = Vector2.zero;

        GameObject textGO = new GameObject("WinText");
        textGO.transform.SetParent(panel.transform, false);
        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = "¡Ganaste!";
        tmp.fontSize = 72;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        RectTransform textRT = textGO.GetComponent<RectTransform>();
        textRT.anchorMin = new Vector2(0, 0.3f);
        textRT.anchorMax = new Vector2(1, 0.7f);
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;

        Time.timeScale = 0f;
        Debug.Log("¡GANASTE!");
    }
}