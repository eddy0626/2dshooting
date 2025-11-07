using UnityEngine;

/// <summary>
/// 이 총알을 쏜 주인을 구분합니다. (Player 또는 Enemy)
/// </summary>
public enum BulletOwner { Player, Enemy }

/// <summary>
/// 총알의 이동 및 소멸, 충돌을 담당하는 스크립트입니다.
/// </summary>
public class Bullet : MonoBehaviour
{
    [Header("총알 능력치")]
    public float InitialSpeed = 1f;
    // ▼▼▼ [수정됨] 요청하신 대로 7f로 변경 ▼▼▼
    public float FinalSpeed = 7f;
    // ▲▲▲ [수정됨] ▲▲▲
    public float AccelerationTime = 1.2f;
    public int Damage = 1; // ❗ 중요: 플레이어 총알 프리팹에서 이 값을 60 또는 40으로 설정하세요!
    public BulletOwner Owner = BulletOwner.Player; // ❗ 중요: 적 총알 프리팹은 이 값을 Enemy로 설정하세요!

    private float _currentSpeed;
    private float _elapsedTime = 0f;

    [Header("소멸 범위")]
    public float DestroyBoundaryX = 10f;
    public float DestroyBoundaryY = 6f;

    void Start()
    {
        _currentSpeed = InitialSpeed;
    }

    void Update()
    {
        _elapsedTime += Time.deltaTime;
        _currentSpeed = Mathf.Lerp(InitialSpeed, FinalSpeed, _elapsedTime / AccelerationTime);

        transform.Translate(Vector2.up * _currentSpeed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x) > DestroyBoundaryX ||
            Mathf.Abs(transform.position.y) > DestroyBoundaryY)
        {
            Destroy(gameObject);
        }
    }

    // ▼▼▼ [새로 추가된 부분] 총알 충돌 처리 ▼▼▼
    /// <summary>
    /// 다른 2D 트리거 콜라이더가 총알에 진입했을 때 호출됩니다.
    /// </summary>
    /// <param name="other">이 트리거에 진입한 다른 Collider2D 컴포넌트</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. 이 총알이 '플레이어'의 총알이고, 충돌한 대상이 'Enemy' 태그인지 확인
        if (Owner == BulletOwner.Player && other.CompareTag("Enemy"))
        {
            // 2. 충돌한 적의 Enemy 컴포넌트를 가져옵니다.
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                // 3. 적에게 이 총알의 Damage만큼 대미지를 줍니다.
                enemy.TakeDamage(Damage);
            }

            // 4. 총알은 적과 충돌했으므로 자신을 파괴합니다.
            Destroy(gameObject);
        }

        // (참고: '적'의 총알이 '플레이어'와 충돌하는 로직은
        //  PlayerMove.cs의 OnTriggerEnter2D에 이미 잘 구현되어 있습니다.)
    }
    // ▲▲▲ [새로 추가된 부분] ▲▲▲
}