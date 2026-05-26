using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movimiento")]
    public Vector3 direction = Vector3.up;
    public float minDistance = -1f;  // reemplaza distance
    public float maxDistance = 1f;  //
    public float speed = 2f;

    [Header("Pausa en los extremos")]
    public float waitTime = 0f;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private Vector3 previousPosition;  // para calcular el delta
    private float waitCounter;
    private bool waiting = false;
    private bool goingToEnd = true;

    private CharacterController playerOnPlatform; // referencia al jugador encima

    void Start()
    {
        Vector3 origin = transform.position;
        startPosition = origin + direction.normalized * minDistance;
        endPosition = origin + direction.normalized * maxDistance;
        previousPosition = transform.position;
    }

    void Update()
    {
        if (waiting)
        {
            waitCounter -= Time.deltaTime;
            if (waitCounter <= 0f)
                waiting = false;
            return;
        }

        Vector3 target = goingToEnd ? endPosition : startPosition;

        transform.position = Vector3.MoveTowards(
            transform.position, target, speed * Time.deltaTime
        );

        // Delta real que se movió la plataforma este frame
        Vector3 delta = transform.position - previousPosition;

        // Empuja al jugador exactamente lo mismo que se movió la plataforma
        if (playerOnPlatform != null)
            playerOnPlatform.Move(delta);

        previousPosition = transform.position;

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            goingToEnd = !goingToEnd;

            if (waitTime > 0f)
            {
                waiting = true;
                waitCounter = waitTime;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerOnPlatform = other.GetComponent<CharacterController>();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerOnPlatform = null;
    }

    void OnDrawGizmos()
    {
        Vector3 start = Application.isPlaying ? startPosition : transform.position;
        Vector3 end = start + direction.normalized * maxDistance;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(start, 0.2f);
        Gizmos.DrawWireSphere(end, 0.2f);
        Gizmos.DrawLine(start, end);
    }
}