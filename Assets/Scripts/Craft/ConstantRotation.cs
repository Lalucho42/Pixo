using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    [Header("Ajustes de Rotacion")]
    public float velocidadDeRotacion = 30f; 

    void Update()
    {
       
        transform.Rotate(0, velocidadDeRotacion * Time.deltaTime, 0);
    }
}