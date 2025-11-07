using UnityEngine;

/// <summary>
/// 적을 플레이어를 향해 추적하도록 이동시킵니다.
/// 또한 맵 이탈 시 자신을 파괴합니다.
/// </summary>
[RequireComponent(typeof(Enemy), typeof(Rigidbody2D))]
public class EnemyMoveTarget : MonoBehaviour
{
    [Header("이동 능력치")]
    public float MoveSpeed = 2f; // 타겟형은 직선형보다 조금 느리게

    [Header("소멸 범위")]
    public float DestroyBoundaryX = 10f;
    public float DestroyBoundaryY = 6f;

    private Rigidbody2D _rb;
    private Enemy _enemy;
    private Transform _playerTransform;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _enemy = GetComponent<Enemy>();

        // 플레이어 찾기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("EnemyMoveTarget: 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다!");
        }
    }

    void FixedUpdate()
    {
        // 이미 죽었다면 이동 정지
        if (_enemy.IsDying())
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        // 플레이어가 없으면 이동 안 함
        if (_playerTransform == null)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        // 플레이어를 향한 방향 계산
        Vector2 direction = (_playerTransform.position - transform.position).normalized;

        // 플레이어를 향해 이동
        _rb.linearVelocity = direction * MoveSpeed;
    }

    void Update()
    {
        // 이미 죽었다면 맵 이탈 검사 안 함
        if (_enemy.IsDying()) return;

        // 몬스터가 맵 바깥으로 나갔는지 확인
        if (Mathf.Abs(transform.position.x) > DestroyBoundaryX ||
            Mathf.Abs(transform.position.y) > DestroyBoundaryY)
        {
            // Enemy.cs의 Die()를 호출하여 안전하게 파괴
            _enemy.Die();
        }
    }
}