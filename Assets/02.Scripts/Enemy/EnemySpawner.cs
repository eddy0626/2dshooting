using UnityEngine;

/// <summary>
/// 적 몬스터를 주기적으로 스폰합니다.
/// 70% 확률로 직선형, 30% 확률로 타겟형을 스폰합니다.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    public GameObject StraightEnemyPrefab;  // 직선형 적 프리팹 (70%)
    public GameObject TargetEnemyPrefab;    // 타겟형 적 프리팹 (30%)
    public Transform SpawnPoint;            // 적이 스폰될 위치

    [Header("스폰 주기 (랜덤)")]
    public float MinSpawnTime = 1.0f;
    public float MaxSpawnTime = 3.0f;

    [Header("스폰 확률")]
    [Range(0f, 100f)]
    public float StraightEnemyProbability = 70f; // 직선형 적 스폰 확률 (%)

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
        float randomDelay = Random.Range(MinSpawnTime, MaxSpawnTime);
        _nextSpawnTime = Time.time + randomDelay;
        Debug.Log($"Next spawn in {randomDelay:F2} seconds.");
    }

    /// <summary>
    /// 확률에 따라 직선형 또는 타겟형 적을 스폰합니다.
    /// </summary>
    void SpawnEnemy()
    {
        if (SpawnPoint == null)
        {
            Debug.LogWarning("EnemySpawner: Spawn Point가 설정되지 않았습니다.");
            return;
        }

        // 0~100 사이의 랜덤 값 생성
        float randomValue = Random.Range(0f, 100f);
        GameObject enemyToSpawn = null;

        // 확률에 따라 적 선택
        if (randomValue < StraightEnemyProbability)
        {
            // 70% - 직선형 적
            enemyToSpawn = StraightEnemyPrefab;
            Debug.Log("Spawning Straight Enemy (70%)");
        }
        else
        {
            // 30% - 타겟형 적
            enemyToSpawn = TargetEnemyPrefab;
            Debug.Log("Spawning Target Enemy (30%)");
        }

        // 선택된 적 스폰
        if (enemyToSpawn != null)
        {
            Instantiate(enemyToSpawn, SpawnPoint.position, SpawnPoint.rotation);
        }
        else
        {
            Debug.LogWarning($"EnemySpawner: 스폰할 적 프리팹이 설정되지 않았습니다. (확률값: {randomValue})");
        }
    }
}