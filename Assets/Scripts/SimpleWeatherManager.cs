using UnityEngine;
using System.Collections;

public enum WeatherState { Clear, Rain, Wind, HeavyStorm }

public class SimpleWeatherManager : MonoBehaviour
{
    public ParticleSystem rainParticles;
    public WindZone windZone;
    private Terrain[] terrains;
    public Light mainLight;
    public float timeBetweenChanges = 20f;
    public float transitionDuration = 4f;
    public Transform followTarget;
    public Vector3 rainOffset = new Vector3(0, 15f, 0);

    [Header("Wind Intensity")]
    public float clearWindIntensity = 0.4f;
    public float rainWindIntensity = 2.2f;
    public float windWindIntensity = 3.5f;
    public float stormWindIntensity = 5.0f;

    [Header("Sun Settings")]
    public float radiantSunIntensity = 1.8f;
    public float defaultSunIntensity = 1.0f;
    public float rainSunIntensity = 0.4f;
    public Color radiantSunColor = new Color(1f, 0.95f, 0.8f);
    public Color stormSunColor = new Color(0.5f, 0.5f, 0.6f);

    [Header("Rain Settings")]
    public float rainEmissionRate = 3000f;

    private WeatherState currentState;
    private Coroutine transitionCoroutine;

    public float WeatherIntensity { get; private set; } = 1f;

    [Header("Storm Effects")]
    public AudioClip thunderSound;
    public float minThunderInterval = 5f;
    public float maxThunderInterval = 15f;
    private float nextThunderTime;
    private float initialLightIntensity;

    private void Start()
    {
        RenderSettings.fog = false;
        if (mainLight != null) initialLightIntensity = mainLight.intensity;
        else if (RenderSettings.sun != null) initialLightIntensity = RenderSettings.sun.intensity;

        if (rainParticles != null)
        {
            var emission = rainParticles.emission;
            emission.rateOverTime = rainEmissionRate;
        }

        if (mainLight == null) mainLight = RenderSettings.sun;
        terrains = FindObjectsByType<Terrain>(FindObjectsSortMode.None);
        if (followTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) followTarget = player.transform;
            else if (Camera.main != null) followTarget = Camera.main.transform;
        }

        Shader.SetGlobalFloat("_WindEnabled", 1f);
        Shader.EnableKeyword("DR_WIND_ENABLED");

        ChangeWeather(WeatherState.Clear);
        StartCoroutine(WeatherCycle());
    }

    private void Update()
    {
        if (currentState == WeatherState.HeavyStorm)
        {
            if (Time.time >= nextThunderTime)
            {
                StartCoroutine(DoThunderEffect());
                nextThunderTime = Time.time + Random.Range(minThunderInterval, maxThunderInterval);
            }
        }
    }

    private IEnumerator DoThunderEffect()
    {
        if (thunderSound != null && AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(thunderSound);
        }

        if (mainLight != null)
        {
            float flashIntensity = initialLightIntensity * 3f;
            mainLight.intensity = flashIntensity;
            yield return new WaitForSeconds(0.1f);
            mainLight.intensity = initialLightIntensity * WeatherIntensity;
            yield return new WaitForSeconds(0.05f);
            mainLight.intensity = flashIntensity * 0.5f;
            yield return new WaitForSeconds(0.1f);
            mainLight.intensity = initialLightIntensity * WeatherIntensity;
        }
    }

    private IEnumerator WeatherCycle()
    {
        while (true)
        {
            WeatherState newState = (WeatherState)Random.Range(0, 4);
            ChangeWeather(newState);
            yield return new WaitForSeconds(timeBetweenChanges);
        }
    }

    private void ChangeWeather(WeatherState newState)
    {
        currentState = newState;

        if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);

        float targetWind = 0;
        float targetSunIntensity = defaultSunIntensity;
        Color targetSunColor = Color.white;

        switch (newState)
        {
            case WeatherState.Clear:
                targetWind = clearWindIntensity;
                targetSunIntensity = radiantSunIntensity;
                targetSunColor = radiantSunColor;
                if (rainParticles != null) rainParticles.Stop();
                break;
            case WeatherState.Rain:
                targetWind = rainWindIntensity;
                targetSunIntensity = rainSunIntensity;
                targetSunColor = stormSunColor;
                if (rainParticles != null) rainParticles.Play();
                break;
            case WeatherState.Wind:
                targetWind = windWindIntensity;
                targetSunIntensity = defaultSunIntensity;
                targetSunColor = Color.white;
                if (rainParticles != null) rainParticles.Stop();
                break;
            case WeatherState.HeavyStorm:
                targetWind = stormWindIntensity;
                targetSunIntensity = rainSunIntensity * 0.5f;
                targetSunColor = stormSunColor;
                if (rainParticles != null) rainParticles.Play();
                break;
        }

        transitionCoroutine = StartCoroutine(TransitionWeather(targetWind, targetSunIntensity, targetSunColor));
    }

    private IEnumerator TransitionWeather(float targetWind, float targetSunIntensity, Color targetSunColor)
    {
        float startWind = windZone != null ? windZone.windMain : 0;
        float startSun = WeatherIntensity;
        Color startColor = mainLight != null ? mainLight.color : Color.white;
        float elapsed = 0;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;

            if (windZone != null) windZone.windMain = Mathf.Lerp(startWind, targetWind, t);

            float currentWind = Mathf.Lerp(startWind, targetWind, t);
            Shader.SetGlobalFloat("_WindIntensity", currentWind);
            Shader.SetGlobalFloat("_WindSpeed", currentWind * 0.1f);

            WeatherIntensity = Mathf.Lerp(startSun, targetSunIntensity, t);

            if (mainLight != null)
            {
                mainLight.color = Color.Lerp(startColor, targetSunColor, t);
            }

            foreach (var tObj in terrains)
            {
                if (tObj != null && tObj.terrainData != null)
                {
                    tObj.terrainData.wavingGrassAmount = Mathf.Clamp(currentWind * 0.25f, 0.05f, 1f);
                    tObj.terrainData.wavingGrassSpeed = Mathf.Clamp(currentWind * 0.2f, 0.05f, 1f);
                    tObj.terrainData.wavingGrassStrength = Mathf.Clamp(currentWind * 0.25f, 0.05f, 1f);
                }
            }

            yield return null;
        }

        if (windZone != null) windZone.windMain = targetWind;
        if (mainLight != null)
        {
            mainLight.color = targetSunColor;
        }
    }

    private void LateUpdate()
    {
        if (rainParticles != null && followTarget != null && rainParticles.isPlaying)
        {
            rainParticles.transform.position = followTarget.position + rainOffset;
        }
    }
}