using UnityEngine;
using System.Collections;

public class SimpleSpawnerTest : MonoBehaviour
{
    // Change this to one of your actual enemy prefabs
    public GameObject enemyPrefabToSpawn;

    void Start()
    {
        Debug.Log("--- Starting Pooler Spawner Test ---");
        // Make sure your pool is ready for this enemy type
        if (ObjectPooler.Instance != null && enemyPrefabToSpawn != null)
        {
            ObjectPooler.Instance.PrewarmPool(enemyPrefabToSpawn, 20);
        }
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // Wait for 2 seconds
            yield return new WaitForSeconds(2f);

            Debug.Log("Attempting to spawn from pool...");

            if (ObjectPooler.Instance == null)
            {
                Debug.LogError("ObjectPooler.Instance is NULL!");
                yield break; // Stop the test if the pooler is missing
            }

            // Spawn the object using the pooler and log the result
            GameObject spawnedObject = ObjectPooler.Instance.SpawnFromPool(enemyPrefabToSpawn, transform.position, Quaternion.identity);

            if (spawnedObject == null)
            {
                Debug.LogWarning("...SpawnFromPool returned NULL. The pool is likely empty.");
            }
            else
            {
                Debug.Log("...Successfully spawned an enemy from the pool!");
            }
        }
    }
}