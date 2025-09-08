using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Configuration")]
    public List<EnemyData> enemyTypes = new();
    public int initialPoolSize = 20;
    public float initialSpawnInterval = 2f;
    public float minimumSpawnInterval = 0.5f;
    [Tooltip("How quickly the spawn interval decreases over time to increase difficulty.")]
    public float difficultyIncreaseRate = 0.01f;

    [Header("Spawn Area")]
    public float spawnPadding = 1f;

    private float currentSpawnInterval;
    private float screenLeft, screenRight, screenTop, screenBottom;

    private void Start()
    {
        if (enemyTypes.Count == 0)
        {
            Debug.LogError("Enemy Spawner has no enemy types configured.", this);
            this.enabled = false;
            return;
        }

        // Pre-warm the object pools for each enemy type
        foreach (var enemyData in enemyTypes)
        {
            if (enemyData.enemyPrefab != null)
            {
                ObjectPooler.Instance.PrewarmPool(enemyData.enemyPrefab, initialPoolSize);
            }
        }

        currentSpawnInterval = initialSpawnInterval;
        CalculateScreenBounds();
        StartCoroutine(SpawnEnemyRoutine());
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState == GameManager.GameState.Playing)
        {
            if (currentSpawnInterval > minimumSpawnInterval)
            {
                currentSpawnInterval -= difficultyIncreaseRate * Time.deltaTime;
            }
        }
    }

    private void CalculateScreenBounds()
    {
        Camera mainCam = Camera.main;
        Vector3 lowerLeft = mainCam.ViewportToWorldPoint(new Vector3(0, 0, mainCam.nearClipPlane));
        Vector3 upperRight = mainCam.ViewportToWorldPoint(new Vector3(1, 1, mainCam.nearClipPlane));

        screenLeft = lowerLeft.x - spawnPadding;
        screenRight = upperRight.x + spawnPadding;
        screenBottom = lowerLeft.y - spawnPadding;
        screenTop = upperRight.y + spawnPadding;
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentSpawnInterval);

            if (GameManager.Instance.CurrentState == GameManager.GameState.Playing)
            {
                SpawnEnemy();
            }
        }
    }

    private void SpawnEnemy()
    {
        EnemyData randomEnemyData = enemyTypes[Random.Range(0, enemyTypes.Count)];
        if (randomEnemyData.enemyPrefab == null) return;

        Vector2 spawnPosition = GetRandomOffScreenPosition();

        GameObject enemyObject = ObjectPooler.Instance.SpawnFromPool(randomEnemyData.enemyPrefab, spawnPosition, Quaternion.identity);

        // The prefab is already configured, but we need to ensure its Enemy component
        // knows which data asset it's using (for score, etc.).
        if (enemyObject != null)
        {
            // Get the Enemy component and call its one, central Initialize method.
            if (enemyObject.TryGetComponent<Enemy>(out var enemyComponent))
            {
                enemyComponent.Initialize(randomEnemyData);
            }
        }
    }

    private Vector2 GetRandomOffScreenPosition()
    {
        int edge = Random.Range(0, 4);
        float x = 0, y = 0;
        switch (edge)
        {
            case 0: y = screenTop; x = Random.Range(screenLeft, screenRight); break;
            case 1: y = screenBottom; x = Random.Range(screenLeft, screenRight); break;
            case 2: x = screenLeft; y = Random.Range(screenBottom, screenTop); break;
            case 3: x = screenRight; y = Random.Range(screenBottom, screenTop); break;
        }
        return new Vector2(x, y);
    }
}
