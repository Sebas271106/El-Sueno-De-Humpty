using UnityEngine;

public class RotacionCheckpoint : MonoBehaviour
{
    [Header("Rotación")]
    public float velocidadRotacion = 90f;  // grados por segundo
    public Vector3 ejeRotacion = Vector3.up; // gira en Y por defecto

    void Update()
    {
        transform.Rotate(ejeRotacion * velocidadRotacion * Time.deltaTime);
    }
}