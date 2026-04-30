using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    [Header("Ajustes de Rotacion")]
    public float velocidadDeRotacion = 30f; // Puedes cambiar esto en el Inspector

    void Update()
    {
        // Rota el objeto en el eje Y (el eje vertical) todo el tiempo
        // Usamos Time.deltaTime para que la rotacion sea suave
        transform.Rotate(0, velocidadDeRotacion * Time.deltaTime, 0);
    }
}