using UnityEngine;

public class FakePlatform : MonoBehaviour
{
    [Header("Configuración")]
    public float delayBeforeHide = 0.5f;
    public bool shakeBeforeHide = true;

    private bool triggered = false;
    private MeshRenderer meshRenderer;
    private Collider platformCollider;

    void Start()
    {
        meshRenderer      = GetComponent<MeshRenderer>();
        platformCollider  = GetComponent<Collider>();
    }

    public void TriggerDestroy()
    {
        if (!triggered)
        {
            triggered = true;
            StartCoroutine(HideSequence());
        }
    }

    // Llamado por el Checkpoint System cuando el jugador reaparece
    public void ResetPlatform()
    {
        triggered                = false;
        meshRenderer.enabled     = true;
        platformCollider.enabled = true;
    }

    System.Collections.IEnumerator HideSequence()
    {
        if (shakeBeforeHide)
        {
            float elapsed     = 0f;
            Vector3 originPos = transform.position;

            while (elapsed < delayBeforeHide)
            {
                float shakeX = Random.Range(-0.05f, 0.05f);
                float shakeZ = Random.Range(-0.05f, 0.05f);
                transform.position = originPos + new Vector3(shakeX, 0, shakeZ);

                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = originPos;
        }
        else
        {
            yield return new WaitForSeconds(delayBeforeHide);
        }

        // Oculta la plataforma sin destruirla
        meshRenderer.enabled     = false;
        platformCollider.enabled = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}