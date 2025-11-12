using UnityEngine;

/// <summary>
/// 아이템의 유형을 정의합니다. (인스펙터에서 선택)
/// </summary>
public enum ItemType
{
    SpeedBoost,     // 이동 속도
    HealthUp,       // 체력 회복
    AttackSpeedUp   // 공격 속도 (쿨타임 감소)
}

/// <summary>
/// 플레이어가 획득할 수 있는 아이템의 동작을 정의합니다.
/// 1. 스폰 후 2초간 대기합니다.
/// 2. 2초 후 플레이어를 향해 날아갑니다.
/// 3. 플레이어와 충돌 시, 설정된 ItemType에 맞는 효과를 적용하고 파괴됩니다.
///
/// 시각 효과: 회전, 스케일 펄스, 부유, 깜빡임, 발광 효과 포함
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Item : MonoBehaviour
{
    [Header("아이템 설정")]
    public ItemType itemType; // 이 아이템의 유형 (인스펙터에서 설정)

    [Header("유도 설정")]
    public float WaitDelay = 2.0f; // 스폰 후 대기 시간 (초)
    public float MoveToPlayerSpeed = 5f; // 플레이어에게 날아가는 속도

    private Transform _playerTransform;
    private float _spawnTime;
    private bool _isMovingToPlayer = false;

    [Header("아이템 효과 수치")]
    // (인스펙터에서 이 아이템 유형에 맞는 수치만 설정하면 됩니다)
    public float SpeedBoostAmount = 0.5f;       // 이동 속도 증가량
    public int HealthUpAmount = 1;              // 체력 회복량
    public float AttackSpeedDecreaseAmount = 0.05f; // 공격 쿨타임 감소량

    [Header("===== 시각 효과 설정 =====")]

    [Header("회전 효과")]
    [SerializeField] private bool EnableRotation = true;
    [SerializeField] private float RotationSpeed = 180f; // 초당 회전 각도

    [Header("스케일 펄스 효과")]
    [SerializeField] private bool EnableScalePulse = true;
    [SerializeField] private float PulseSpeed = 3f; // 펄스 속도
    [SerializeField] private float PulseAmplitude = 0.15f; // 펄스 크기 (±15%)

    [Header("부유 효과 (위아래 움직임)")]
    [SerializeField] private bool EnableFloating = true;
    [SerializeField] private float FloatSpeed = 2f; // 부유 속도
    [SerializeField] private float FloatAmplitude = 0.3f; // 부유 높이

    [Header("깜빡임 효과")]
    [SerializeField] private bool EnableBlink = true;
    [SerializeField] private float BlinkSpeed = 2f; // 깜빡임 속도
    [SerializeField] private float MinAlpha = 0.5f; // 최소 투명도
    [SerializeField] private float MaxAlpha = 1f; // 최대 투명도

    [Header("발광 효과")]
    [SerializeField] private bool EnableGlow = true;
    [SerializeField] private float GlowSpeed = 3f; // 발광 속도
    [SerializeField] private float GlowMin = 0.8f; // 최소 밝기
    [SerializeField] private float GlowMax = 1.3f; // 최대 밝기

    [Header("스프라이트 애니메이션")]
    [SerializeField] private Animator _animator; // 선택적: 스프라이트 애니메이션

    // 시각 효과용 private 변수
    private SpriteRenderer _spriteRenderer;
    private Vector3 _originalScale;
    private Vector3 _startPosition;
    private Color _originalColor;
    private bool _isEffectsInitialized = false;

    /// <summary>
    /// 아이템 생성 시 호출됩니다.
    /// </summary>
    void Start()
    {
        _spawnTime = Time.time; // 생성 시간 저장

        // 플레이어를 미리 찾아둡니다.
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Item: 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다!");
        }

        // 시각 효과 초기화
        InitializeVisualEffects();
    }

    /// <summary>
    /// 시각 효과를 초기화합니다.
    /// </summary>
    private void InitializeVisualEffects()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalScale = transform.localScale;

        if (_spriteRenderer != null)
        {
            _originalColor = _spriteRenderer.color;
        }

        // Animator 자동 찾기
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
    }

    /// <summary>
    /// 매 프레임마다 호출됩니다.
    /// </summary>
    void Update()
    {
        // 시작 위치 저장 (첫 프레임 이후)
        if (!_isEffectsInitialized && Time.frameCount > 1)
        {
            _startPosition = transform.position;
            _isEffectsInitialized = true;
        }

        // === 아이템 이동 로직 ===
        // 아직 유도 상태가 아니고, 대기 시간이 지났다면
        if (!_isMovingToPlayer && Time.time >= _spawnTime + WaitDelay)
        {
            _isMovingToPlayer = true; // 유도 상태로 변경
        }

        // 유도 상태이고, 플레이어가 살아있다면
        if (_isMovingToPlayer && _playerTransform != null)
        {
            // 플레이어를 향하는 방향 계산
            Vector2 direction = (_playerTransform.position - transform.position).normalized;
            // 플레이어에게로 이동
            transform.Translate(direction * MoveToPlayerSpeed * Time.deltaTime, Space.World);
        }

        // === 시각 효과 업데이트 ===
        UpdateVisualEffects();
    }

    /// <summary>
    /// 시각 효과를 매 프레임 업데이트합니다.
    /// </summary>
    private void UpdateVisualEffects()
    {
        // 회전 효과
        if (EnableRotation)
        {
            transform.Rotate(0f, 0f, RotationSpeed * Time.deltaTime);
        }

        // 스케일 펄스 효과
        if (EnableScalePulse)
        {
            float pulse = 1f + Mathf.Sin(Time.time * PulseSpeed) * PulseAmplitude;
            transform.localScale = _originalScale * pulse;
        }

        // 부유 효과 (위아래로 움직임) - 유도 상태가 아닐 때만
        if (EnableFloating && _isEffectsInitialized && !_isMovingToPlayer)
        {
            float yOffset = Mathf.Sin(Time.time * FloatSpeed) * FloatAmplitude;
            Vector3 newPosition = _startPosition;
            newPosition.y += yOffset;
            transform.position = newPosition;
        }

        // 스프라이트 색상 효과
        if (_spriteRenderer != null)
        {
            Color finalColor = _originalColor;

            // 발광 효과
            if (EnableGlow)
            {
                float glow = Mathf.Lerp(GlowMin, GlowMax, (Mathf.Sin(Time.time * GlowSpeed) + 1f) / 2f);
                finalColor.r *= glow;
                finalColor.g *= glow;
                finalColor.b *= glow;
            }

            // 깜빡임 효과 (투명도 변화)
            if (EnableBlink)
            {
                float alpha = Mathf.Lerp(MinAlpha, MaxAlpha, (Mathf.Sin(Time.time * BlinkSpeed) + 1f) / 2f);
                finalColor.a = alpha;
            }

            _spriteRenderer.color = finalColor;
        }
    }

    /// <summary>
    /// 다른 2D 트리거 콜라이더가 이 트리거에 진입했을 때 호출됩니다.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 총알과의 충돌은 무시 (아이템은 총알에 영향받지 않음)
        if (other.CompareTag("Bullet"))
        {
            return; // 총알과 충돌 시 아무것도 하지 않음
        }

        // 플레이어와 충돌했는지 확인
        if (other.CompareTag("Player"))
        {
            // 플레이어의 컴포넌트들을 가져옵니다.
            PlayerMove playerMove = other.GetComponent<PlayerMove>();
            PlayerFire playerFire = other.GetComponent<PlayerFire>();

            // 설정된 아이템 유형(itemType)에 따라 다른 효과 적용
            switch (itemType)
            {
                case ItemType.SpeedBoost:
                    if (playerMove != null)
                    {
                        playerMove.Speed += SpeedBoostAmount;
                        Debug.Log($"이동 속도 증가! 현재 속도: {playerMove.Speed}");
                    }
                    break;

                case ItemType.HealthUp:
                    if (playerMove != null)
                    {
                        // PlayerMove에 새로 추가할 Heal() 메서드 호출
                        playerMove.Heal(HealthUpAmount);
                    }
                    break;

                case ItemType.AttackSpeedUp:
                    if (playerFire != null)
                    {
                        // PlayerFire에 새로 추가할 MinFireCooldown과 비교
                        playerFire.FireCooldown = Mathf.Max(
                            playerFire.FireCooldown - AttackSpeedDecreaseAmount,
                            playerFire.MinFireCooldown
                        );
                        Debug.Log($"공격 속도 증가! 현재 쿨타임: {playerFire.FireCooldown}");
                    }
                    break;
            }

            // 효과를 적용했으므로 아이템 파괴 (플레이어와 충돌 시에만)
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 스크립트가 비활성화될 때 색상과 위치를 원래대로 복구
    /// </summary>
    void OnDisable()
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _originalColor;
        }

        if (_isEffectsInitialized)
        {
            transform.position = _startPosition;
        }
    }
}