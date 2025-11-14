using UnityEngine;

public enum BulletPattern
{
    Straight,
    SShape,
    Spiral
}

public class AdvancedBullet : MonoBehaviour
{
    [Header("Basic Settings")]
    [SerializeField] private float damage = 60f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lifeTime = 5f;
    
    [Header("Pattern Settings")]
    [SerializeField] private BulletPattern pattern = BulletPattern.Straight;
    [SerializeField] private float amplitude = 2f; // S자 진폭
    [SerializeField] private float frequency = 2f; // S자 주파수
    [SerializeField] private float spiralSpeed = 100f; // 나선 회전 속도
    [SerializeField] private float spiralRadius = 0.5f; // 나선 반지름
    
    private Vector3 direction;
    private Vector3 startPosition;
    private float elapsedTime;
    private float angle;
    
    private void Start()
    {
        startPosition = transform.position;
        Destroy(gameObject, lifeTime);
    }
    
    public void Initialize(float dmg, Vector3 dir, BulletPattern bulletPattern)
    {
        damage = dmg;
        direction = dir.normalized;
        pattern = bulletPattern;
        startPosition = transform.position;
    }
    
    private void Update()
    {
        elapsedTime += Time.deltaTime;
        
        switch (pattern)
        {
            case BulletPattern.Straight:
                MoveStraight();
                break;
            case BulletPattern.SShape:
                MoveSShape();
                break;
            case BulletPattern.Spiral:
                MoveSpiral();
                break;
        }
    }
    
    private void MoveStraight()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
    
    private void MoveSShape()
    {
        // 전진 방향
        Vector3 forward = direction * speed * Time.deltaTime;
        
        // 좌우 진동 (S자)
        Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0);
        float sway = Mathf.Sin(elapsedTime * frequency) * amplitude;
        Vector3 sideways = perpendicular * sway * Time.deltaTime;
        
        transform.position += forward + sideways;
    }
    
    private void MoveSpiral()
    {
        // 전진
        transform.position += direction * speed * Time.deltaTime;
        
        // 나선 회전
        angle += spiralSpeed * Time.deltaTime;
        Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0);
        Vector3 spiral = perpendicular * Mathf.Sin(angle * Mathf.Deg2Rad) * spiralRadius;
        
        transform.position += spiral;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
                enemy.TakeDamage(damage);
            
            Destroy(gameObject);
        }
    }
    
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
