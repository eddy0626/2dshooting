using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private EnemyType _enemyType = EnemyType.Straight;
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _targetSpeed = 4f;
    [SerializeField] private GameObject[] _itemPrefabs;
    [SerializeField] private float _dropChance = 0.5f;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private GameObject hitEffect;
    
    private float _currentHealth;
    private Transform _playerTransform;
    private bool _isDead = false;
    
    private void Start()
    {
        _currentHealth = _maxHealth;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            _playerTransform = player.transform;
    }
    
    private void Update()
    {
        if (_isDead) return;
        
        if (_enemyType == EnemyType.Straight)
        {
            transform.Translate(Vector3.down * _moveSpeed * Time.deltaTime);
        }
        else if (_enemyType == EnemyType.Target && _playerTransform != null)
        {
            Vector3 direction = (_playerTransform.position - transform.position).normalized;
            transform.position += direction * _targetSpeed * Time.deltaTime;
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }
    }
    
    public void TakeDamage(float damage)
    {
        if (_isDead) return;
        
        _currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Health: {_currentHealth}/{_maxHealth}");
        
        // 히트 이펙트 생성
        if (hitEffect != null)
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        
        if (_currentHealth <= 0)
            Die();
    }
    
    private void Die()
    {
        _isDead = true;
        Debug.Log($"{gameObject.name} died!");
        
        // 점수 추가
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.AddScore(10);
        }
        
        CameraShake cameraShake = Camera.main?.GetComponent<CameraShake>();
        if (cameraShake != null)
            cameraShake.Shake(0.3f, 0.2f);
        
        // 폭발 이펙트 생성
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        
        DropItem();
        Destroy(gameObject);
    }
    
    private void DropItem()
    {
        if (_itemPrefabs == null || _itemPrefabs.Length == 0) return;
        if (Random.value > _dropChance) return;
        
        float rand = Random.value;
        int itemIndex = rand < 0.7f ? 0 : (rand < 0.9f ? 1 : 2);
        
        if (itemIndex < _itemPrefabs.Length && _itemPrefabs[itemIndex] != null)
            Instantiate(_itemPrefabs[itemIndex], transform.position, Quaternion.identity);
    }
    
    public void SetEnemyType(EnemyType type) => _enemyType = type;
}

public enum EnemyType { Straight, Target }