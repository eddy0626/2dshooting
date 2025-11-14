using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject _straightEnemyPrefab;
    [SerializeField] private GameObject _targetEnemyPrefab;
    [SerializeField] private float _minSpawnInterval = 1f;
    [SerializeField] private float _maxSpawnInterval = 3f;
    [SerializeField] private float _straightEnemyChance = 0.7f;
    [SerializeField] private float _spawnMinX = -8f;
    [SerializeField] private float _spawnMaxX = 8f;
    [SerializeField] private float _spawnY = 6f;
    [SerializeField] private bool _isSpawning = true;
    [SerializeField] private int _maxEnemyCount = 20;
    
    private float _nextSpawnTime;
    
    private void Start()
    {
        SetNextSpawnTime();
    }
    
    private void Update()
    {
        if (!_isSpawning) return;
        
        if (Time.time >= _nextSpawnTime)
        {
            SpawnEnemy();
            SetNextSpawnTime();
        }
    }
    
    private void SetNextSpawnTime()
    {
        float interval = Random.Range(_minSpawnInterval, _maxSpawnInterval);
        _nextSpawnTime = Time.time + interval;
    }
    
    private void SpawnEnemy()
    {
        int currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (currentEnemyCount >= _maxEnemyCount) return;
        
        Vector3 spawnPosition = new Vector3(
            Random.Range(_spawnMinX, _spawnMaxX),
            _spawnY,
            0f
        );
        
        bool spawnStraight = Random.value < _straightEnemyChance;
        GameObject enemyPrefab = spawnStraight ? _straightEnemyPrefab : _targetEnemyPrefab;
        
        if (enemyPrefab == null) return;
        
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        enemy.tag = "Enemy";
        
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
            enemyScript.SetEnemyType(spawnStraight ? EnemyType.Straight : EnemyType.Target);
    }
}