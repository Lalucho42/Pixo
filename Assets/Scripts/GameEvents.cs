using UnityEngine;
using System;

public static class GameEvents
{
    public static event Action<Vector3, Vector3, GameObject> OnSurfaceImpact;
    public static event Action<Vector3, Vector3, GameObject> OnSimpleImpact;

    public static void TriggerSurfaceImpact(Vector3 position, Vector3 normal, GameObject data)
    {
        OnSurfaceImpact?.Invoke(position, normal, data);
    }

    public static void TriggerSimpleImpact(Vector3 position, Vector3 normal, GameObject prefab)
    {
        OnSimpleImpact?.Invoke(position, normal, prefab);
    }
}
