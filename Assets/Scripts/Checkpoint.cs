using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Visual opcional")]
    public GameObject activeVisual;

    private bool activated = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        CheckpointSystem cs = other.GetComponent<CheckpointSystem>();
        if (cs != null)
            cs.SetCheckpoint(transform.position);

        // Activa el visual solo la primera vez (o tras un reset de nivel)
        if (!activated)
        {
            activated = true;
            if (activeVisual != null)
                activeVisual.SetActive(true);
        }
    }

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