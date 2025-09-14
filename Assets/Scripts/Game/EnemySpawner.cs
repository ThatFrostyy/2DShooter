using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Wave Configuration")]
    [Tooltip("The list of wave configurations to spawn in order.")]
    public List<WaveData> waves = new();

    [Tooltip("The time to wait before starting the first wave.")]
    public float initialDelay = 3f;

    [Tooltip("The time to wait between spawning each wave.")]
    public float timeBetweenWaves = 5f;

    [Tooltip("Should the spawner loop back to the first wave after finishing?")]
    public bool loopWaves = true;

    public int prewarmPoolSize = 15;

    [Header("Spawn Area")]
    [Tooltip("How far off-screen enemies should spawn.")]
    public float spawnPadding = 1.5f;

    private int currentWaveIndex = 0;
    private float screenLeft, screenRight, screenTop, screenBottom;

    private void Start()
    {
        if (waves.Count == 0)
        {
            Debug.LogError("No waves configured for the Wave Spawner.", this);
            this.enabled = false;
            return;
        }

        PrewarmPools();
        CalculateScreenBounds();
        StartCoroutine(SpawnAllWavesRoutine());
    }

    private void PrewarmPools()
    {
        var uniquePrefabs = new HashSet<GameObject>();
        foreach (var wave in waves)
        {
            foreach (var subWave in wave.subWaves)
            {
                if (subWave.enemyData != null && subWave.enemyData.enemyPrefab != null)
                {
                    uniquePrefabs.Add(subWave.enemyData.enemyPrefab);
                }
            }
        }

        foreach (var prefab in uniquePrefabs)
        {
            ObjectPooler.Instance.PrewarmPool(prefab, prewarmPoolSize);
        }
    }

    private IEnumerator SpawnAllWavesRoutine()
    {
        yield return new WaitForSeconds(initialDelay);

        while (true) // This loop handles repeating the entire wave sequence if `loopWaves` is true.
        {
            if (GameManager.Instance.CurrentState == GameManager.GameState.Playing)
            {
                // If we've gone through all the waves, either stop or loop.
                if (currentWaveIndex >= waves.Count)
                {
                    if (loopWaves)
                    {
                        currentWaveIndex = 0;
                    }
                    else
                    {
                        Debug.Log("All waves completed.");
                        this.enabled = false;
                        yield break; 
                    }
                }

                // Spawn the current wave and wait for it to complete.
                yield return StartCoroutine(SpawnSingleWaveRoutine(waves[currentWaveIndex]));

                // Wait for the time between waves.
                yield return new WaitForSeconds(timeBetweenWaves);

                currentWaveIndex++;
            }
            else
            {
                // If the game is not in the 'Playing' state, just wait and check again.
                yield return new WaitForSeconds(1f);
            }
        }
    }

    private IEnumerator SpawnSingleWaveRoutine(WaveData wave)
    {
        foreach (var subWave in wave.subWaves)
        {
            // Wait for the delay specified for this particular group.
            yield return new WaitForSeconds(subWave.delayBefore);

            // Spawn each enemy in the group.
            for (int i = 0; i < subWave.count; i++)
            {
                SpawnEnemy(subWave.enemyData);

                // Wait for the interval before spawning the next enemy in this group.
                yield return new WaitForSeconds(subWave.spawnInterval);
            }
        }
    }

    private void SpawnEnemy(EnemyData dataToSpawn)
    {
        if (dataToSpawn == null || dataToSpawn.enemyPrefab == null)
        {
            Debug.LogWarning("Attempted to spawn an enemy with null Data or Prefab.");
            return;
        }

        GameObject prefabToSpawn = dataToSpawn.enemyPrefab;
        Vector2 spawnPosition = GetRandomOffScreenPosition();

        GameObject enemyObject = ObjectPooler.Instance.SpawnFromPool(prefabToSpawn, spawnPosition, Quaternion.identity);

        if (enemyObject != null)
        {
            if (enemyObject.TryGetComponent<Enemy>(out var enemyComponent))
            {
                enemyComponent.Initialize(dataToSpawn);
            }
        }
    }

    #region Helper Methods
    private void CalculateScreenBounds()
    {
        Camera mainCam = Camera.main;
        // The z-coordinate matters for ViewportToWorldPoint.
        float zDistance = Mathf.Abs(mainCam.transform.position.z);
        Vector3 lowerLeft = mainCam.ViewportToWorldPoint(new Vector3(0, 0, zDistance));
        Vector3 upperRight = mainCam.ViewportToWorldPoint(new Vector3(1, 1, zDistance));

        screenLeft = lowerLeft.x - spawnPadding;
        screenRight = upperRight.x + spawnPadding;
        screenBottom = lowerLeft.y - spawnPadding;
        screenTop = upperRight.y + spawnPadding;
    }

    private Vector2 GetRandomOffScreenPosition()
    {
        int edge = Random.Range(0, 4);
        float x = 0, y = 0;
        switch (edge)
        {
            case 0: // Top
                y = screenTop;
                x = Random.Range(screenLeft + spawnPadding, screenRight - spawnPadding);
                break;
            case 1: // Bottom
                y = screenBottom;
                x = Random.Range(screenLeft + spawnPadding, screenRight - spawnPadding);
                break;
            case 2: // Left
                x = screenLeft;
                y = Random.Range(screenBottom + spawnPadding, screenTop - spawnPadding);
                break;
            case 3: // Right
                x = screenRight;
                y = Random.Range(screenBottom + spawnPadding, screenTop - spawnPadding);
                break;
        }
        return new Vector2(x, y);
    }
    #endregion
}