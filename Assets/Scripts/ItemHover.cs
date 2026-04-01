using UnityEngine;

public class ItemHover : MonoBehaviour
{
    [Header("Hover Settings")]
    public float rotationSpeed = 90f;
    public float hoverAmplitude = 0.3f;
    public float hoverFrequency = 1.0f;

    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);

        float newY = initialPosition.y + Mathf.Abs(Mathf.Sin(Time.time * Mathf.PI * hoverFrequency)) * hoverAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
