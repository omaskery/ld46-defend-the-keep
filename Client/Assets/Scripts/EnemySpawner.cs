using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private EnemyController enemyPrefab;
    
    [SerializeField] private Transform target;

    [SerializeField] private Transform enemyParent;
    
    [Header("Configurations")]
    [SerializeField] private float spawnDistance;
    [SerializeField] private int enemiesToSpawn = 1;

    [SerializeField] private float difficultyInterval = 10.0f;

    [SerializeField] private float minSpawnInterval = 0.1f;
    [SerializeField] private float maxSpawnInterval = 4.0f;

    private int _enemiesDestroyed;

    public event Action<int> EnemyDestroyed;

    void Start()
    {
        ScheduleSpawn();
        ScheduleDifficultyIncrease();
    }

    private void ScheduleDifficultyIncrease()
    {
        Invoke(nameof(OnIncreaseDifficulty), difficultyInterval);
    }

    private void OnIncreaseDifficulty()
    {
        ScheduleDifficultyIncrease();
        enemiesToSpawn += 1;
    }

    private void ScheduleSpawn()
    {
        var interval = GenerateRandomSpawnInterval();
        Invoke(nameof(OnScheduledSpawn), interval);
    }

    private void OnScheduledSpawn()
    {
        ScheduleSpawn();
        
        for (var i = 0; i < enemiesToSpawn; i++)
        {
            SpawnAtCircumference();
        }
    }

    private void SpawnAtCircumference()
    {
        var theta = Random.value * Mathf.PI * 2.0f;
        var position = new Vector3(
            Mathf.Cos(theta) * spawnDistance,
            0,
            Mathf.Sin(theta) * spawnDistance
        );

        SpawnAt(position);
    }

    private void SpawnAt(Vector3 position)
    {
        var rotation = Quaternion.Euler(0, 360.0f * Random.value, 0);
        var enemy = Instantiate(enemyPrefab, position, rotation, enemyParent);
        enemy.target = target;

        if (enemy.TryGetComponent<Health>(out var health))
        {
            health.Destroyed += OnEnemyDestroyed;
        }
    }

    private void OnEnemyDestroyed(Health health)
    {
        health.Destroyed -= OnEnemyDestroyed;
        _enemiesDestroyed++;
        
        EnemyDestroyed?.Invoke(_enemiesDestroyed);
    }

    private float GenerateRandomSpawnInterval()
    {
        return Random.Range(minSpawnInterval, maxSpawnInterval);
    }
}
