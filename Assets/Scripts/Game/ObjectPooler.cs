using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    #region Singleton
    public static ObjectPooler Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    public int defaultIntialSize = 5;

    private class Pool
    {
        public GameObject Prefab;
        public List<GameObject> PooledObjects;
        public Transform PoolHolder;
    }

    private Dictionary<string, Pool> poolDictionary = new();
    private GameObject poolsParent;

    private void Start()
    {
        poolsParent = new GameObject("--- Object Pools ---");
        DontDestroyOnLoad(poolsParent);
    }

    public GameObject SpawnFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null)
        {
            Debug.LogWarning("Attempted to spawn a null prefab.");
            return null;
        }

        string tag = prefab.name;

        // If the pool doesn't exist, create it before trying to spawn from it.
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.Log($"Pool with tag '{tag}' doesn't exist. Creating a new one.");
            CreateNewPool(tag, prefab, defaultIntialSize); 
        }

        Pool pool = poolDictionary[tag];
        GameObject objectToSpawn = null;

        // Search for an inactive object in the existing pool.
        foreach (var obj in pool.PooledObjects)
        {
            if (!obj.activeInHierarchy)
            {
                objectToSpawn = obj;
                break;
            }
        }

        // If no inactive objects were found, expand the pool.
        if (objectToSpawn == null)
        {
            objectToSpawn = Instantiate(pool.Prefab, pool.PoolHolder);
            pool.PooledObjects.Add(objectToSpawn);
        }

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.SetPositionAndRotation(position, rotation);

        return objectToSpawn;
    }

    public void PrewarmPool(GameObject prefab, int size)
    {
        if (prefab == null) return;
        string tag = prefab.name;

        if (poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag '{tag}' already exists.");
            return;
        }
        CreateNewPool(tag, prefab, size);
    }

    private void CreateNewPool(string tag, GameObject prefab, int size)
    {
        Pool newPool = new Pool
        {
            Prefab = prefab,
            PooledObjects = new List<GameObject>(),
            PoolHolder = new GameObject($"{tag}_Pool").transform
        };
        newPool.PoolHolder.SetParent(poolsParent.transform);

        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab, newPool.PoolHolder);
            obj.SetActive(false);
            newPool.PooledObjects.Add(obj);
        }
        poolDictionary.Add(tag, newPool);
    }
}

