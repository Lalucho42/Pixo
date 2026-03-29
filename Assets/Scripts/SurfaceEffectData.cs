using UnityEngine;

[CreateAssetMenu(fileName = "SurfaceEffect", menuName = "VFX/SurfaceEffectData")]
public class SurfaceEffectData : ScriptableObject
{
    public GameObject particlePrefab;
    public AudioClip impactSound;
    public GameObject decalPrefab;
}
