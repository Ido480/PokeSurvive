using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public GameObject enemyPrefab;
        public float spawnTimer;
        public float spawnInterval;
        public int enemiesPerWave;
        public int spawnedEnemyCount;
    }

    [Header("Wave Settings")]
    public List<Wave> waves;
    public int waveNumber;
    public Transform minPos;
    public Transform maxPos;

    [Header("Boss Settings")]
    public GameObject miniBossPrefab;
    public GameObject bigBossPrefab;
    public GameObject miniBoss2Prefab;
    [Header("Spawn Timings")]
    public float miniBoss1Time = 120f;
    public float miniBoss2Time = 180f;
    public float bigBossSpawnTime = 240f; // 300 for stage 3

    private bool miniBoss2Spawned = false; private bool miniBossSpawned = false;
    private bool bigBossSpawned = false;

    [Header("Hierarchy Organization")]
    [SerializeField] private Transform spawnContainer;

    void Start()
    {
        if (spawnContainer == null)
        {
            spawnContainer = new GameObject("--- ENEMIES ---").transform;
        }
    }

    void Update()
    {
        if (!PlayerController.Instance.gameObject.activeSelf) return;

        float currentTime = GameManager.Instance.gameTime;

        // 1. FIRST MINI BOSS
        if (currentTime >= miniBoss1Time && !miniBossSpawned)
        {
            SpawnSpecialEnemy(miniBossPrefab);
            miniBossSpawned = true;
        }

        // 2. SECOND MINI BOSS
        if (currentTime >= miniBoss2Time && !miniBoss2Spawned && miniBoss2Prefab != null)
        {
            SpawnSpecialEnemy(miniBoss2Prefab);
            miniBoss2Spawned = true;
        }

        // 3. BIG BOSS
        if (currentTime >= bigBossSpawnTime && !bigBossSpawned)
        {
            WipeAllNormalEnemies();
            SpawnSpecialEnemy(bigBossPrefab);
            bigBossSpawned = true;
        }

        if (!bigBossSpawned)
        {
            HandleWaveLogic();
        }
    }
    private void HandleWaveLogic()
    {
        // Advance the timer for the current wave
        waves[waveNumber].spawnTimer += Time.deltaTime;

        // Time to spawn an enemy?
        if (waves[waveNumber].spawnTimer >= waves[waveNumber].spawnInterval)
        {
            waves[waveNumber].spawnTimer = 0;
            SpawnEnemy();
        }

        // Is the wave finished?
        if (waves[waveNumber].spawnedEnemyCount >= waves[waveNumber].enemiesPerWave)
        {
            waves[waveNumber].spawnedEnemyCount = 0;

            // Speed up the next wave slightly
            if (waves[waveNumber].spawnInterval > 0.15f)
            {
                waves[waveNumber].spawnInterval *= 0.8f;
            }

            // Move to next wave or loop back to 0
            waveNumber++;
            if (waveNumber >= waves.Count)
            {
                waveNumber = 0;
            }
        }
    }

    private void SpawnEnemy()
    {
        Instantiate(waves[waveNumber].enemyPrefab, RandomSpawnPoint(), transform.rotation, spawnContainer);
        waves[waveNumber].spawnedEnemyCount++;
    }

    private void SpawnSpecialEnemy(GameObject prefab)
    {
        if (prefab != null)
        {
            Instantiate(prefab, RandomSpawnPoint(), transform.rotation, spawnContainer);
        }
    }

    private void WipeAllNormalEnemies()
    {
        GameObject[] activeEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in activeEnemies)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                // PROTECT MINI BOSSES AND BIG BOSSES
                // Dialga and Palkia are marked as isMiniBoss
                if (enemyScript.isBigBoss || enemyScript.isMiniBoss)
                {
                    continue; // Skip this enemy, don't destroy it
                }

                Destroy(enemy);
            }
        }
    }
    private Vector2 RandomSpawnPoint()
    {
        Vector2 spawnPoint;
        if (Random.Range(0f, 1f) > 0.5f)
        {
            spawnPoint.x = Random.Range(minPos.position.x, maxPos.position.x);
            spawnPoint.y = Random.Range(0f, 1f) > 0.5f ? minPos.position.y : maxPos.position.y;
        }
        else
        {
            spawnPoint.y = Random.Range(minPos.position.y, maxPos.position.y);
            spawnPoint.x = Random.Range(0f, 1f) > 0.5f ? minPos.position.x : maxPos.position.x;
        }
        return spawnPoint;
    }
}