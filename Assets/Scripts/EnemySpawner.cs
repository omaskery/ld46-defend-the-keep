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

    [SerializeField] private float minSpawnInterval = 0.1f;
    [SerializeField] private float maxSpawnInterval = 4.0f;

    void Start()
    {
        ScheduleSpawn();
    }

    private void ScheduleSpawn()
    {
        var interval = GenerateRandomSpawnInterval();
        Invoke(nameof(OnScheduledSpawn), interval);
    }

    private void OnScheduledSpawn()
    {
        ScheduleSpawn();
        SpawnAtCircumference();
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
    }

    private float GenerateRandomSpawnInterval()
    {
        return Random.Range(minSpawnInterval, maxSpawnInterval);
    }
}
