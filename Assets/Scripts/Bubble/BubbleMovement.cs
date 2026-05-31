using UnityEngine;

public class BubbleMovement : MonoBehaviour
{
    [Header("Vertical Speed")]
    [SerializeField, Range(0.5f, 10f)] private float minRiseSpeed = 1f;
    [SerializeField, Range(0.5f, 10f)] private float maxRiseSpeed = 3f;

    [Header("Oscillation")]
    [SerializeField, Range(0f, 3f)] private float minAmplitude = 0.2f;
    [SerializeField, Range(0f, 3f)] private float maxAmplitude = 0.8f;
    [SerializeField, Range(0.1f, 5f)] private float minFrequency = 0.5f;
    [SerializeField, Range(0.1f, 5f)] private float maxFrequency = 2f;

    // ─────────────────────────────────────────────────────────────────────────────
    // Estado interno
    // ─────────────────────────────────────────────────────────────────────────────

    private float   _riseSpeed;
    private float   _amplitude;
    private float   _frequency;
    private float   _phaseOffset;
    private Vector3 _oscillationAxis;
    private float   _elapsed;
    private bool    _trapped;

    // ─────────────────────────────────────────────────────────────────────────────
    // Unity
    // ─────────────────────────────────────────────────────────────────────────────

    private void Awake()  => Randomize();
    private void OnEnable() => Randomize();

    private void Update()
    {
        if (_trapped) return;

        _elapsed += Time.deltaTime;

        transform.position += Vector3.up * (_riseSpeed * Time.deltaTime);

        float oscillation = _amplitude * Mathf.Sin(2f * Mathf.PI * _frequency * _elapsed + _phaseOffset);
        transform.position += _oscillationAxis * (oscillation * Time.deltaTime);
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // API Pública
    // ─────────────────────────────────────────────────────────────────────────────

    public void SetTrapped(bool trapped) => _trapped = trapped;
    public float RiseSpeed => _riseSpeed;

    // ─────────────────────────────────────────────────────────────────────────────
    // Privados
    // ─────────────────────────────────────────────────────────────────────────────

    private void Randomize()
    {
        _elapsed  = 0f;
        _trapped  = false;

        _riseSpeed   = Random.Range(minRiseSpeed, maxRiseSpeed);
        _amplitude   = Random.Range(minAmplitude, maxAmplitude);
        _frequency   = Random.Range(minFrequency, maxFrequency);
        _phaseOffset = Random.Range(0f, Mathf.PI * 2f);

        float angle = Random.Range(0f, Mathf.PI * 2f);
        _oscillationAxis = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));

        // ← Escala eliminada, la maneja BubblePool
    }
}