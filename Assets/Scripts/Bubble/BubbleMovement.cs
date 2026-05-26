using UnityEngine;

public class BubbleMovement : MonoBehaviour
{
    [Header("Movimiento Burbuja")]
    public float upwardSpeed = 2f;
    public float waveAmplitude = 2f;
    public float waveFrequency = 2f;

    private Vector3 startPosition;
    private float timeCounter;
    private bool isMoving = false;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (isMoving)
            MoveBubble();
    }

    public void StartMoving()
    {
        startPosition = transform.position;
        timeCounter = 0f;
        isMoving = true;
    }

    public void StopMoving()
    {
        isMoving = false;
    }

    void MoveBubble()
    {
        timeCounter += Time.deltaTime;

        float x = Mathf.Sin(timeCounter * waveFrequency) * waveAmplitude;
        float y = upwardSpeed * timeCounter;

        transform.position = startPosition + new Vector3(x, y, 0);
    }
}
