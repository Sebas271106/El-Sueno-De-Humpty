using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GuardianOrbit : MonoBehaviour
{
    [Header("Orbit Settings")]
    [SerializeField] private Transform bathtub;
    [SerializeField] private float orbitSpeed = 3f;
    [SerializeField] private float rotationSpeed = 8f;

    [Header("Rectangle Size")]
    [SerializeField] private float halfWidth  = 2f;  // mitad del ancho (eje X local)
    [SerializeField] private float halfLength = 3f;  // mitad del largo (eje Z local)
    [SerializeField] private float offsetFromEdge = 0.8f; // distancia extra al borde

    private Rigidbody _rb;
    private Vector3[] _waypoints;
    private int _currentWaypoint = 0;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        GenerateWaypoints();
    }

    private void GenerateWaypoints()
    {
        if (bathtub == null) return;

        float hw = halfWidth  + offsetFromEdge;
        float hl = halfLength + offsetFromEdge;
        float y  = transform.position.y;

        // Calcula las 4 esquinas en espacio LOCAL de la bañera
        // y las convierte a espacio mundial respetando su rotación
        Vector3[] localCorners = new Vector3[]
        {
            new Vector3(-hw, 0f, -hl),
            new Vector3(-hw, 0f,  hl),
            new Vector3( hw, 0f,  hl),
            new Vector3( hw, 0f, -hl),
        };

        _waypoints = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            Vector3 world = bathtub.TransformPoint(localCorners[i]);
            _waypoints[i] = new Vector3(world.x, y, world.z);
        }

        _currentWaypoint = GetClosestWaypoint();
    }

    private int GetClosestWaypoint()
    {
        int closest = 0;
        float minDist = float.MaxValue;
        for (int i = 0; i < _waypoints.Length; i++)
        {
            float dist = Vector3.Distance(transform.position, _waypoints[i]);
            if (dist < minDist) { minDist = dist; closest = i; }
        }
        return closest;
    }

    private void FixedUpdate()
    {
        if (_waypoints == null || _waypoints.Length == 0) return;
        PatrolRectangle();
    }

    private void PatrolRectangle()
    {
        Vector3 target = _waypoints[_currentWaypoint];
        target.y = transform.position.y;

        Vector3 newPos = Vector3.MoveTowards(
            transform.position, target, orbitSpeed * Time.fixedDeltaTime
        );
        _rb.MovePosition(newPos);

        if (Vector3.Distance(transform.position, target) < 0.15f)
            _currentWaypoint = (_currentWaypoint + 1) % _waypoints.Length;

        Vector3 dir = (target - transform.position);
        dir.y = 0f;
        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            _rb.MoveRotation(Quaternion.Slerp(
                transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime
            ));
        }
    }

    private void OnDrawGizmos()
    {
        if (_waypoints == null) return;
        Gizmos.color = Color.cyan;
        for (int i = 0; i < _waypoints.Length; i++)
        {
            Gizmos.DrawSphere(_waypoints[i], 0.15f);
            Gizmos.DrawLine(_waypoints[i], _waypoints[(i + 1) % _waypoints.Length]);
        }
    }
}