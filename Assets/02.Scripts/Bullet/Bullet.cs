using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _damage = 60f;
    [SerializeField] private float _lifeTime = 5f;
    [SerializeField] private BulletType _bulletType = BulletType.Main;
    [SerializeField] private float _initialSpeed = 1f;
    [SerializeField] private float _finalSpeed = 7f;
    [SerializeField] private float _accelerationTime = 1.2f;
    [SerializeField] private GameObject explosionEffect;
    
    private Vector3 _direction;
    private float _currentSpeed;
    private float _elapsedTime = 0f;
    
    private void Start()
    {
        _currentSpeed = _initialSpeed;
        Destroy(gameObject, _lifeTime);
    }
    
    public void Initialize(float damage, Vector3 direction, BulletType type)
    {
        _damage = damage;
        _direction = direction.normalized;
        _bulletType = type;
        _currentSpeed = _initialSpeed;
    }
    
    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        
        if (_elapsedTime < _accelerationTime)
        {
            float t = _elapsedTime / _accelerationTime;
            _currentSpeed = Mathf.Lerp(_initialSpeed, _finalSpeed, t);
        }
        else
        {
            _currentSpeed = _finalSpeed;
        }
        
        transform.position += _direction * _currentSpeed * Time.deltaTime;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
                enemy.TakeDamage(_damage);
            
            // 폭발 이펙트 생성
            if (explosionEffect != null)
                Instantiate(explosionEffect, transform.position, Quaternion.identity);
            
            Destroy(gameObject);
        }
    }
    
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}