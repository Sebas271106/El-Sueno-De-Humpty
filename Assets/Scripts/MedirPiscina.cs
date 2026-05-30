using UnityEngine;

public class MedirPiscina : MonoBehaviour
{
    private void Start()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Debug.Log($"Total renderers encontrados: {renderers.Length}");

        foreach (Renderer r in renderers)
        {
            Debug.Log($"{r.gameObject.name} → " +
                      $"Size: {r.bounds.size} | " +
                      $"Center: {r.bounds.center} | " +
                      $"Scale: {r.transform.lossyScale}");
        }

        if (renderers.Length == 0) return;

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
            bounds.Encapsulate(r.bounds);

        Debug.Log("=== RESULTADO FINAL ===");
        Debug.Log($"Centro mundial: {bounds.center}");
        Debug.Log($"Half Width (X): {bounds.size.x / 2f}");
        Debug.Log($"Half Length (Z): {bounds.size.z / 2f}");
    }
}