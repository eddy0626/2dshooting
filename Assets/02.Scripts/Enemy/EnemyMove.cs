using UnityEngine;

/// <summary>
/// 적을 설정된 방향(기본값: 아래)으로 직선 이동시킵니다.
/// 또한 맵 이탈 시 자신을 파괴합니다.
/// </summary>
[RequireComponent(typeof(Enemy), typeof(Rigidbody2D))] // Enemy와 Rigidbody2D가 반드시 필요
public class EnemyMoveStraight : MonoBehaviour
{
    [Header("이동 능력치")]
    public float MoveSpeed = 3f;
    public Vector2 MoveDirection = Vector2.down; // 이동할 방향 (기본: 아래)

    [Header("소멸 범위")]
    public float DestroyBoundaryX = 10f;
    public float DestroyBoundaryY = 6f;

    private Rigidbody2D _rb;
    private Enemy _enemy; // '본체' Enemy.cs 스크립트 참조

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _enemy = GetComponent<Enemy>();

        // 시작과 동시에 설정된 방향으로 일정한 속도를 부여합니다.
        _rb.linearVelocity = MoveDirection.normalized * MoveSpeed;
    }

    void FixedUpdate()
    {
        // 만약 '본체'가 죽었다면(체력이 0이 되거나 맵 이탈)
        if (_enemy.IsDying())
        {
            // 이동을 즉시 멈춥니다.
            _rb.linearVelocity = Vector2.zero;
        }
        // (살아있다면 Start에서 설정한 속도로 계속 직선 이동합니다)
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