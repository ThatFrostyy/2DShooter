using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    #region Singleton
    public static ObjectPooler Instance;
    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); poolsParent = new GameObject("--- Object Pools ---"); DontDestroyOnLoad(poolsParent); }
        else { Destroy(gameObject); }
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

    public GameObject SpawnFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null) return null; 

        string tag = prefab.name;

        // If the pool for this prefab doesn't exist yet, create it
        if (!poolDictionary.ContainsKey(tag))
        {
            CreateNewPool(tag, prefab, defaultIntialSize);
        }

        Pool pool = poolDictionary[tag];
        GameObject objectToSpawn = null;

        foreach (var obj in pool.PooledObjects)
        {
            if (obj != null && !obj.activeInHierarchy)
            {
                objectToSpawn = obj;
                break;
            }
        }

        // If no inactive object is found, instantiate a new one and add it to the pool
        if (objectToSpawn == null)
        {
            objectToSpawn = Instantiate(pool.Prefab, pool.PoolHolder);
            objectToSpawn.SetActive(false); 
            pool.PooledObjects.Add(objectToSpawn);
        }
      
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.SetPositionAndRotation(position, rotation);

        return objectToSpawn;
    }

    public void ReturnToPool(GameObject objectToReturn)
    {
        if (objectToReturn != null)
        {
            objectToReturn.SetActive(false);
        }
    }

    public void PrewarmPool(GameObject prefab, int size)
    {
        if (prefab == null) return;

        string tag = prefab.name;

        if (poolDictionary.ContainsKey(tag)) return;

        CreateNewPool(tag, prefab, size);
    }

    private void CreateNewPool(string tag, GameObject prefab, int size)
    {
        Pool newPool = new()
        {
            Prefab = prefab, 
            PooledObjects = new List<GameObject>(), 
            PoolHolder = new GameObject($"{tag}_Pool").transform 
        };

        newPool.PoolHolder.SetParent(poolsParent.transform);

        // Pre-instantiate the specified number of objects
        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab, newPool.PoolHolder);
            obj.SetActive(false);
            newPool.PooledObjects.Add(obj);
        }
        poolDictionary.Add(tag, newPool);
    }
}