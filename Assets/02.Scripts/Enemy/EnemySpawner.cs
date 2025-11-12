using UnityEngine;

/// <summary>
/// 적 몬스터를 주기적으로 스폰합니다.
/// 3가지 타입의 적을 확률적으로 스폰합니다.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    public GameObject normalEnemyPrefab;    // 일반 적 프리팹
    public GameObject straightEnemyPrefab;  // 직선형 적 프리팹
    public GameObject targetEnemyPrefab;    // 타겟형 적 프리팹
    public Transform spawnPoint;            // 적이 스폰될 위치

    [Header("스폰 주기 (랜덤)")]
    public float minSpawnTime = 1.0f;
    public float maxSpawnTime = 3.0f;

    [Header("스폰 확률 (합계 100%)")]
    [Range(0f, 100f)]
    public float normalEnemyProbability = 33f;   // 일반 적 스폰 확률 (%)
    [Range(0f, 100f)]
    public float straightEnemyProbability = 33f; // 직선형 적 스폰 확률 (%)
    [Range(0f, 100f)]
    public float targetEnemyProbability = 34f;   // 타겟형 적 스폰 확률 (%)

    private float _nextSpawnTime;

    void Start()
    {
        SetNextSpawnTime();
    }

    void Update()
    {
        if (Time.time >= _nextSpawnTime)
        {
            SpawnEnemy();
            SetNextSpawnTime();
        }
    }

    /// <summary>
    /// 다음 스폰 시간을 랜덤하게 계산하여 설정합니다.
    /// </summary>
    private void SetNextSpawnTime()
    {
        float randomDelay = Random.Range(minSpawnTime, maxSpawnTime);
        _nextSpawnTime = Time.time + randomDelay;
        Debug.Log($"Next spawn in {randomDelay:F2} seconds.");
    }

    /// <summary>
    /// 확률에 따라 일반, 직선형 또는 타겟형 적을 스폰합니다.
    /// </summary>
    void SpawnEnemy()
    {
        if (spawnPoint == null)
        {
            Debug.LogWarning("EnemySpawner: Spawn Point가 설정되지 않았습니다.");
            return;
        }

        // 0~100 사이의 랜덤 값 생성
        float randomValue = Random.Range(0f, 100f);
        GameObject enemyToSpawn = null;
        string enemyType = "";

        // 확률에 따라 적 선택
        if (randomValue < normalEnemyProbability)
        {
            // 일반 적
            enemyToSpawn = normalEnemyPrefab;
            enemyType = "Normal";
        }
        else if (randomValue < normalEnemyProbability + straightEnemyProbability)
        {
            // 직선형 적
            enemyToSpawn = straightEnemyPrefab;
            enemyType = "Straight";
        }
        else
        {
            // 타겟형 적
            enemyToSpawn = targetEnemyPrefab;
            enemyType = "Target";
        }

        // 선택된 적 스폰
        if (enemyToSpawn != null)
        {
            Instantiate(enemyToSpawn, spawnPoint.position, spawnPoint.rotation);
            Debug.Log($"Spawning {enemyType} Enemy (확률값: {randomValue:F2})");
        }
        else
        {
            Debug.LogWarning($"EnemySpawner: 스폰할 {enemyType} 적 프리팹이 설정되지 않았습니다.");
        }
    }
}