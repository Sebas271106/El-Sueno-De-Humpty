using UnityEngine;

public class AguaController : MonoBehaviour
{
    [Header("Tiempos")]
    public float tiempoVisible = 4f;
    public float tiempoOculto  = 1f;

    [Header("Aleatoriedad")]
    public float delayMaximo = 4f;

    private Material aguaMaterial;
    private float timer;
    private bool aguaActiva = false;
    private bool delayTerminado = false;
    private Collider aguaCollider;

    private static readonly int ClipHash = Shader.PropertyToID("_Clip");

    void Start()
    {
        aguaMaterial  = GetComponent<Renderer>().material;
        aguaCollider  = GetComponent<Collider>();

        SetAgua(false);
        float delayInicial = Random.Range(0f, delayMaximo);
        Invoke(nameof(IniciarCiclo), delayInicial);
    }

    void IniciarCiclo()
    {
        delayTerminado = true;
        aguaActiva     = true;
        SetAgua(true);
        timer = tiempoVisible;
    }

    void Update()
    {
        if (!delayTerminado) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            aguaActiva = !aguaActiva;
            SetAgua(aguaActiva);
            timer = aguaActiva ? tiempoVisible : tiempoOculto;
        }
    }

    void SetAgua(bool visible)
    {
        aguaMaterial.SetFloat(ClipHash, visible ? 1f : 0f);

        // El collider solo activa daño cuando el agua es visible
        if (aguaCollider != null)
            aguaCollider.enabled = visible;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        HealthSystem health = other.GetComponent<HealthSystem>();
        if (health != null)
            health.LoseLife();
    }
}