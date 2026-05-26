using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BubbleTrap : MonoBehaviour
{
    [Header("Captura")]
    public Transform capturePoint;

    [Header("Tiempo dentro de la burbuja")]
    public float trappedTime = 4f;

    [Header("Lanzamiento")]
    public float launchForce = 15f;

    private bool playerCaptured = false;
    private GameObject currentPlayer;
    private BubbleMovement bubbleMovement;

    void Start()
    {
        bubbleMovement = GetComponent<BubbleMovement>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (playerCaptured) return;

        if (other.CompareTag("Player"))
            CapturePlayer(other.gameObject);
    }

    void CapturePlayer(GameObject player)
    {
        playerCaptured = true;
        currentPlayer = player;

        // Desactivar CharacterController
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // Rigidbody temporal
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb == null) rb = player.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;

        // Meter jugador en la burbuja
        player.transform.position = capturePoint.position;
        player.transform.SetParent(transform);

        // Arrancar movimiento
        bubbleMovement.StartMoving();

        StartCoroutine(ReleasePlayer());
    }

    IEnumerator ReleasePlayer()
    {
        yield return new WaitForSeconds(trappedTime);

        currentPlayer.transform.SetParent(null);

        // Destruir Rigidbody temporal
        Rigidbody rb = currentPlayer.GetComponent<Rigidbody>();
        Destroy(rb);

        // Reactivar CharacterController para que el OnTriggerEnter de DeathZone funcione
        CharacterController cc = currentPlayer.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = true;

        Destroy(gameObject);

        StartCoroutine(PlayerDeathRoutine());
    }
    IEnumerator PlayerDeathRoutine()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}