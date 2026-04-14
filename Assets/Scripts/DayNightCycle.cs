using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Light sunLight;
    public float dayDurationInSeconds = 300f;

    [Header("Ambient Lighting")]
    public Color dayAmbientColor = new Color(0.8f, 0.85f, 0.9f);
    public Color nightAmbientColor = new Color(0.05f, 0.05f, 0.1f);
    public float minSunIntensity = 0.05f;

    [Header("Skybox Control")]
    public Material skyboxMaterial;
    public float daySkyExposure = 1.0f;
    public float nightSkyExposure = 0.1f;

    [Header("Initial State")]
    [Range(0, 24)]
    public float startTimeInHours = 10.0f;

    private float currentRotation = 0f;
    private SimpleWeatherManager weather;

    private void Start()
    {
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        if (skyboxMaterial == null) skyboxMaterial = RenderSettings.skybox;

        weather = FindAnyObjectByType<SimpleWeatherManager>();

        currentRotation = (startTimeInHours / 24f) * 360f - 90f;

        if (sunLight != null) RenderSettings.sun = sunLight;
    }

    private void Update()
    {
        if (sunLight == null) return;

        float rotationSpeed = 360f / dayDurationInSeconds;
        currentRotation += rotationSpeed * Time.deltaTime;
        sunLight.transform.rotation = Quaternion.Euler(currentRotation, -90f, 0f);

        float sunAltitude = Vector3.Dot(sunLight.transform.forward, Vector3.down);

        float targetFactor = Mathf.Clamp01((sunAltitude + 0.15f) / 0.6f);
        float dayFactor = Mathf.SmoothStep(0f, 1f, targetFactor);

        float baseIntensity = (weather != null) ? weather.WeatherIntensity : 1.0f;

        sunLight.intensity = Mathf.Lerp(minSunIntensity, baseIntensity, dayFactor);

        RenderSettings.ambientLight = Color.Lerp(nightAmbientColor, dayAmbientColor, dayFactor);

        if (skyboxMaterial != null)
        {
            if (skyboxMaterial.HasProperty("_Exposure"))
                skyboxMaterial.SetFloat("_Exposure", Mathf.Lerp(nightSkyExposure, daySkyExposure, dayFactor));
            else if (skyboxMaterial.HasProperty("_Intensity"))
                skyboxMaterial.SetFloat("_Intensity", Mathf.Lerp(nightSkyExposure, daySkyExposure, dayFactor));

            if (skyboxMaterial.HasProperty("_Tint"))
                skyboxMaterial.SetColor("_Tint", Color.Lerp(Color.gray, Color.white, dayFactor));
        }

        RenderSettings.reflectionIntensity = Mathf.Lerp(0.1f, 1.0f, dayFactor);
    }
}