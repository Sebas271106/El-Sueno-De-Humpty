using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Textos")]
    [SerializeField] private TMP_Text objectiveText;
    [SerializeField] private TMP_Text checkpointText;

    [Header("Configuración")]
    [SerializeField] private int totalCheckpoints = 5;

    private int currentCheckpoints = 0;

    private void Awake()
    {
        Instance = this;
        Debug.Log("UIManager Awake");
    }

    private void Start()
    {
        Debug.Log("UIManager Start");

        if (objectiveText == null)
            Debug.LogError("ObjectiveText NO está asignado");

        if (checkpointText == null)
            Debug.LogError("CheckpointText NO está asignado");
        else
            Debug.Log("CheckpointText asignado a: " + checkpointText.name);

        UpdateCheckpointText();
        SetObjective("Llegar a la salida");
    }

    public void CheckpointReached()
    {
        currentCheckpoints++;

        if (currentCheckpoints > totalCheckpoints)
            currentCheckpoints = totalCheckpoints;

        Debug.Log($"CheckpointReached() -> {currentCheckpoints}/{totalCheckpoints}");

        UpdateCheckpointText();

        if (currentCheckpoints >= totalCheckpoints)
        {
            SetObjective("Objetivo");
        }
    }

    private void UpdateCheckpointText()
    {
        if (checkpointText == null)
        {
            Debug.LogError("CheckpointText es NULL");
            return;
        }

        string newText = $"Checkpoint:\n{currentCheckpoints}/{totalCheckpoints}";

        checkpointText.text = newText;

        Debug.Log("Texto actualizado a: " + newText);
    }

    public void SetObjective(string objective)
    {
        if (objectiveText == null)
        {
            Debug.LogError("ObjectiveText es NULL");
            return;
        }

        objectiveText.text = objective;
    }
}