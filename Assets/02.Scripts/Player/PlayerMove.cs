using UnityEngine;

/// <summary>
/// 플레이어의 이동, 체력, 피격을 모두 담당하는 스크립트입니다.
/// 1. 키보드로 이동합니다. (워프, 속도 조절 포함)
/// 2. '적 총알' 또는 '적 몸체'와 충돌하면 체력이 1 감소합니다.
/// 3. 총 체력은 3이며, 0이 되면 플레이어가 파괴됩니다.
/// </summary>
public class PlayerMove : MonoBehaviour
{
    [Header("능력치")]
    public float Speed = 3f;
    public float MaxSpeed = 10f;
    public float MinSpeed = 1f;
    public float SpeedIncrement = 0.5f;
    public float ShiftSpeedMultiplier = 1.2f;
    public float ReturnToOriginSpeed = 2f;

    [Header("시작위치")]
    private Vector2 _originPosition;

    [Header("이동범위")]
    public float minX = -8f;
    public float maxX = 8f;
    public float minY = -4.5f;
    public float maxY = 4.5f;

    [Header("플레이어 체력")]
    public int MaxPlayerHealth = 3;  // 플레이어의 최대 체력 (적과 충돌 횟수)
    private int _currentPlayerHealth; // 현재 플레이어 체력
    private bool _isDead = false;     // 사망 여부 (중복 사망 방지)

    /// <summary>
    /// 현재 체력을 반환합니다.
    /// </summary>
    public int GetCurrentHealth()
    {
        return _currentPlayerHealth;
    }

    /// <summary>
    /// 플레이어의 초기 위치와 체력을 설정합니다.
    /// </summary>
    private void Start()
    {
        _originPosition = transform.position; // 현재 위치를 시작 위치로 저장
        _currentPlayerHealth = MaxPlayerHealth; // 현재 체력을 최대 체력으로 초기화
        Debug.Log($"Player initialized with {MaxPlayerHealth} health.");
    }

    /// <summary>
    /// 매 프레임마다 호출되어 플레이어의 이동 로직을 처리합니다.
    /// </summary>
    void Update()
    {
        // 플레이어가 죽었다면 이동 로직을 실행하지 않습니다.
        if (_isDead) return;

        // R 키 원점 복귀 로직
        if (Input.GetKey(KeyCode.R))
        {
            HandleReturnToOrigin();
        }
        else // 일반 이동
        {
            float effectiveSpeed = GetEffectiveSpeed();
            HandleManualMovement(effectiveSpeed);
        }

        // 워프 및 속도 조절
        ApplyBoundaryWarp();
        HandleSpeedAdjustments();
    }

    /// <summary>
    /// 다른 2D 트리거 콜라이더가 플레이어에 진입했을 때 호출됩니다.
    /// '적 총알'과 '적 몸체' 충돌을 모두 감지합니다.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 이미 죽었다면 아무것도 처리하지 않습니다.
        if (_isDead) return;

        // 1. '적 총알'에 맞았는지 확인
        if (other.CompareTag("Bullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();

            // ▼▼▼▼▼ [여기 수정됨!] ▼▼▼▼▼
            // 'Bullet.'을 지워서 'BulletOwner'로 수정
            if (bullet != null && bullet.Owner == BulletOwner.Enemy)
            // ▲▲▲▲▲ [여기 수정됨!] ▲▲▲▲▲
            {
                Destroy(other.gameObject); // 적 총알 파괴
                TakeDamage(1); // 체력 1 감소
            }
        }

        // 2. '적 몸체'와 부딪혔는지 확인
        if (other.CompareTag("Enemy"))
        {
            // 몸체 충돌 시 체력 1 감소
            TakeDamage(1);

            // 플레이어가 아직 살아있다면, 연속 충돌 방지를 위해 적을 파괴
            if (!_isDead)
            {
                // Enemy 스크립트의 Die()를 호출하는 것이 더 안전함
                Enemy enemy = other.GetComponentInParent<Enemy>();
                if (enemy != null)
                {
                    enemy.Die();
                }
                else
                {
                    // Enemy 스크립트를 못찾으면 그냥 오브젝트 파괴
                    Destroy(other.gameObject);
                }
            }
        }
    }

    /// <summary>
    /// 플레이어의 체력을 감소시키고 사망 여부를 확인합니다.
    /// </summary>
    /// <param name="damage">받은 대미지</param>
    private void TakeDamage(int damage)
    {
        if (_isDead) return; // 중복 방지

        _currentPlayerHealth -= damage;
        Debug.Log($"Player hit! Remaining health: {_currentPlayerHealth}/{MaxPlayerHealth}");

        if (_currentPlayerHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 플레이어가 죽었을 때 호출되는 메서드입니다.
    /// </summary>
    private void Die()
    {
        if (_isDead) return; // 중복 사망 방지
        _isDead = true;

        Debug.Log("Player died!");
        Destroy(gameObject); // 현재 플레이어 게임 오브젝트를 파괴합니다.
    }

    /// <summary>
    /// 플레이어의 체력을 회복합니다.
    /// </summary>
    /// <param name="amount">회복할 체력량</param>
    public void Heal(int amount)
    {
        if (_isDead) return; // 죽었으면 회복 안 됨

        _currentPlayerHealth = Mathf.Min(_currentPlayerHealth + amount, MaxPlayerHealth);
        Debug.Log($"Player healed! Current health: {_currentPlayerHealth}/{MaxPlayerHealth}");
    }

    // --- (아래는 이동 관련 헬퍼 메서드들) ---

    private float GetEffectiveSpeed()
    {
        float effectiveSpeed = Speed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            effectiveSpeed *= ShiftSpeedMultiplier;
        }
        return effectiveSpeed;
    }

    private void HandleManualMovement(float effectiveSpeed)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector2 direction = new Vector2(horizontalInput, verticalInput).normalized;
        Vector2 newPosition = (Vector2)transform.position + direction * effectiveSpeed * Time.deltaTime;
        transform.position = newPosition;
    }

    private void HandleReturnToOrigin()
    {
        Vector3 directionToOrigin = ((Vector3)_originPosition - transform.position).normalized;
        transform.Translate(directionToOrigin * ReturnToOriginSpeed * Time.deltaTime, Space.World);
    }

    private void ApplyBoundaryWarp()
    {
        Vector3 currentPosition = transform.position;
        if (currentPosition.x < minX) currentPosition.x = maxX;
        else if (currentPosition.x > maxX) currentPosition.x = minX;
        if (currentPosition.y < minY) currentPosition.y = maxY;
        else if (currentPosition.y > maxY) currentPosition.y = minY;
        transform.position = currentPosition;
    }

    private void HandleSpeedAdjustments()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Speed = Mathf.Min(Speed + SpeedIncrement, MaxSpeed);
            Debug.Log($"Speed increased to: {Speed}");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Speed = Mathf.Max(Speed - SpeedIncrement, MinSpeed);
            Debug.Log($"Speed decreased to: {Speed}");
        }
    }
}