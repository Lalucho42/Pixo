using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class SubtleDustEffect : MonoBehaviour
{
    private void Awake()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        Configure(ps);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying) return;
        ParticleSystem ps = GetComponent<ParticleSystem>();
        if (ps != null) Configure(ps);
    }
#endif

    private void Configure(ParticleSystem ps)
    {
        var main = ps.main;
        var emission = ps.emission;
        var shape = ps.shape;
        var colorOverLifetime = ps.colorOverLifetime;

        main.duration = 1.0f;
        main.loop = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.5f, 1.0f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.3f, 0.8f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.5f);
        main.startRotation = new ParticleSystem.MinMaxCurve(0, 360);
        main.startColor = new Color(0.7f, 0.6f, 0.5f, 0.6f);
        main.gravityModifier = -0.1f;
        main.stopAction = ParticleSystemStopAction.Callback;

        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0, 5, 10) });

        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;

        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.0f, 0.0f), new GradientAlphaKey(1.0f, 0.2f), new GradientAlphaKey(0.0f, 1.0f) }
        );
        colorOverLifetime.color = gradient;
    }
}
