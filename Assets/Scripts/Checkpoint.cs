using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Visual opcional")]
    public GameObject activeVisual;

    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        CheckpointSystem cs = other.GetComponent<CheckpointSystem>();

        if (cs != null)
        {
            cs.SetCheckpoint(transform.position);
        }

        if (!activated)
        {
            activated = true;

            if (activeVisual != null)
            {
                activeVisual.SetActive(true);
            }

            // Buscar UIManager automáticamente
            UIManager ui = UIManager.Instance;

            if (ui == null)
            {
                ui = FindFirstObjectByType<UIManager>();
            }

            if (ui != null)
            {
                ui.CheckpointReached();
            }
            else
            {
                Debug.LogError(
                    "No existe ningún objeto con el script UIManager en la escena."
                );
            }
        }
    }

    public void ResetCheckpoint()
    {
        activated = false;

        if (activeVisual != null)
        {
            activeVisual.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = activated ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}