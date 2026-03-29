using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class ParticlePoolManager : MonoBehaviour
{
    public static ParticlePoolManager instance;
    private Dictionary<GameObject, ObjectPool<GameObject>> pools = new Dictionary<GameObject, ObjectPool<GameObject>>();

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public void SpawnParticle(GameObject prefab, Vector3 position, Vector3 normal)
    {
        if (prefab == null) return;

        if (!pools.ContainsKey(prefab))
        {
            pools.Add(prefab, new ObjectPool<GameObject>(
                () => Instantiate(prefab),
                (obj) => obj.SetActive(true),
                (obj) => obj.SetActive(false),
                (obj) => Destroy(obj),
                false, 10, 50
            ));
        }

        GameObject poolObj = pools[prefab].Get();
        poolObj.transform.position = position;
        poolObj.transform.forward = normal;

        ParticlePoolHelper helper = poolObj.GetComponent<ParticlePoolHelper>();
        if (helper == null) helper = poolObj.AddComponent<ParticlePoolHelper>();
        helper.Setup(pools[prefab]);
    }
}

public class ParticlePoolHelper : MonoBehaviour
{
    private ObjectPool<GameObject> myPool;
    private ParticleSystem ps;

    public void Setup(ObjectPool<GameObject> pool)
    {
        myPool = pool;
        if (ps == null) ps = GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            main.stopAction = ParticleSystemStopAction.Callback;
        }
    }

    private void OnParticleSystemStopped()
    {
        if (myPool != null) myPool.Release(gameObject);
    }
}
