using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    public static PoolObject instance;
    public enum VFXType
    {
        None,
        Blood,
        HitSpark,
        ParryEffect,
        Dust,
        HealEffect,
        GhostEffect,
        DameText,
        TulenObject,
        FanObject,
    }

    [System.Serializable]
    public class PoolData
    {
        public VFXType type;
        public GameObject prefab;
        public int size = 10;
    }

    [Header("Settings")]
    public List<PoolData> pools;

    private Dictionary<VFXType, Queue<GameObject>> poolDictionary = new Dictionary<VFXType, Queue<GameObject>>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        InitPools();
    }

    private void InitPools()
    {
        foreach (var pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.type, objectPool);
        }
    }

    public GameObject SpawnObject(VFXType type, Vector2 position, Vector2 direction = default, float duration = 0.5f)
    {
        if (!poolDictionary.ContainsKey(type))
        {
            Debug.LogWarning($"Chưa cấu hình Pool cho loại: {type}");
            return null;
        }

        GameObject obj = poolDictionary[type].Dequeue();

        obj.transform.position = position;
        obj.transform.localScale = new Vector3(obj.transform.localScale.x * direction.x > 0 ? 1 : -1, obj.transform.localScale.y);

        obj.SetActive(true);
        if (obj.TryGetComponent(out ParticleSystem particleSystem))
        {
            particleSystem.Play();
        }

        poolDictionary[type].Enqueue(obj);

        StartCoroutine(DisableVFX(obj, duration));
        return obj;
    }
    private void SpawnTulenObject(Transform center)
    {
        GameObject obj = poolDictionary[VFXType.TulenObject].Dequeue();
        obj.GetComponent<RotateChildrenAround>().orbitCenter = center;

        obj.SetActive(true);
        if (obj.TryGetComponent(out ParticleSystem particleSystem))
        {
            particleSystem.Play();
        }

        poolDictionary[VFXType.TulenObject].Enqueue(obj);
    }
    private void SpawnFanObject(Transform center, float timeDiactive)
    {
        GameObject obj = poolDictionary[VFXType.FanObject].Dequeue();
        obj.GetComponent<RotateChildrenAround>().orbitCenter = center;

        obj.SetActive(true);
        if (obj.TryGetComponent(out ParticleSystem particleSystem))
        {
            particleSystem.Play();
        }

        poolDictionary[VFXType.FanObject].Enqueue(obj);
        StartCoroutine(DisableVFX(obj, timeDiactive));
    }
    public void SpawnDamageAbleObject(PoolObject.VFXType type, Transform target, float time = 5)
    {
        switch (type)
        {
            case VFXType.TulenObject:
                SpawnTulenObject(target);
                break;
            case VFXType.FanObject:
                SpawnFanObject(target, time);
                break;
        }
    }
    private IEnumerator DisableVFX(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }
    public void ReturnToPool(GameObject obj, VFXType type)
    {
        poolDictionary[type].Enqueue(obj);
    }
}