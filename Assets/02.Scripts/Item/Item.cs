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
/// </summary>
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
    }

    /// <summary>
    /// 매 프레임마다 호출됩니다.
    /// </summary>
    void Update()
    {
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
}