using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Visual opcional")]
    public GameObject activeVisual;

    private bool activated = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Siempre guarda la posición aunque ya estuviera activado
        CheckpointSystem cs = other.GetComponent<CheckpointSystem>();
        if (cs != null)
            cs.SetCheckpoint(transform.position);

        // Solo activa el visual la primera vez
        if (!activated)
        {
            activated = true;
            if (activeVisual != null)
                activeVisual.SetActive(true);
            Debug.Log($"Checkpoint activado: {gameObject.name}");
        }
    }

    // Llamado desde CheckpointSystem cuando se reinicia el nivel
    public void ResetCheckpoint()
    {
        activated = false;
        if (activeVisual != null)
            activeVisual.SetActive(false);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = activated ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}