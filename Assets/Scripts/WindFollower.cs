using UnityEngine;

public class WindFollower : MonoBehaviour
{
    public float speedMultiplier = 1.0f;
    public float maxRange = 1000f;
    
    private WindZone globalWind;
    private Vector3 initialPosition;

    void Start()
    {
        globalWind = Object.FindAnyObjectByType<WindZone>();
        initialPosition = transform.position;
    }

    void Update()
    {
        if (globalWind == null) return;

        Vector3 windDirection = globalWind.transform.forward;
        float windIntensity = globalWind.windMain;

        transform.position += windDirection * windIntensity * speedMultiplier * Time.deltaTime;

        Vector3 offset = transform.position - initialPosition;

        if (Mathf.Abs(offset.x) > maxRange) 
            transform.position = new Vector3(initialPosition.x - (Mathf.Sign(offset.x) * maxRange), transform.position.y, transform.position.z);
        
        if (Mathf.Abs(offset.z) > maxRange) 
            transform.position = new Vector3(transform.position.x, transform.position.y, initialPosition.z - (Mathf.Sign(offset.z) * maxRange));
    }
}
