using UnityEngine;

public class VFXManager : MonoBehaviour
{
    private void OnEnable()
    {
        GameEvents.OnSurfaceImpact += HandleSurfaceImpact;
        GameEvents.OnSimpleImpact += HandleSimpleImpact;
    }

    private void OnDisable()
    {
        GameEvents.OnSurfaceImpact -= HandleSurfaceImpact;
        GameEvents.OnSimpleImpact -= HandleSimpleImpact;
    }

    private void HandleSurfaceImpact(Vector3 position, Vector3 normal, SurfaceEffectData data)
    {
        if (data == null) return;
        if (data.particlePrefab != null && ParticlePoolManager.instance != null) ParticlePoolManager.instance.SpawnParticle(data.particlePrefab, position, normal);
        if (data.impactSound != null) AudioSource.PlayClipAtPoint(data.impactSound, position);
    }

    private void HandleSimpleImpact(Vector3 position, Vector3 normal, GameObject prefab)
    {
        if (prefab != null && ParticlePoolManager.instance != null) ParticlePoolManager.instance.SpawnParticle(prefab, position, normal);
    }
}
