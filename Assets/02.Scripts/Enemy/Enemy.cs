using UnityEngine;

/// <summary>
/// 적 몬스터의 생명력과 전반적인 상태를 관리합니다. (생명 총괄)
/// 외부로부터 대미지를 받거나, 다른 스크립트의 요청(맵 이탈 등)에 의해 파괴됩니다.
/// 사망 시 설정된 확률에 따라 아이템을 드랍합니다.
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("능력치")]
    [SerializeField] public int MaxHealth = 100;

    private int _currentHealth;
    private bool _isDying = false; // 중복 사망 방지 플래그

    // ▼▼▼ [새로 추가할 변수] 아이템 드랍 설정 ▼▼▼
    [Header("아이템 드랍")]
    public GameObject SpeedItemPrefab;      // 드랍할 이동속도 아이템 프리팹
    public GameObject HealthItemPrefab;     // 드랍할 체력 아이템 프리팹
    public GameObject AttackSpeedItemPrefab; // 드랍할 공격속도 아이템 프리팹

    [Header("아이템 드랍 확률 (0.0 ~ 1.0)")]
    public float ItemDropChance = 0.5f; // 아이템을 드랍할 기본 확률 (50%)

    // (아래 3개 확률의 합은 1.0 (100%) 이어야 합니다)
    public float SpeedItemChance = 0.7f;      // 드랍 시 속도 아이템 확률 (70%)
    public float HealthItemChance = 0.2f;     // 드랍 시 체력 아이템 확률 (20%)
    // (공격속도 아이템 확률은 나머지 10%가 됩니다)
    // ▲▲▲ [새로 추가할 변수] ▲▲▲

    /// <summary>
    /// 적의 초기 체력을 설정합니다.
    /// </summary>
    void Start()
    {
        _currentHealth = MaxHealth;
    }

    /// <summary>
    /// 외부로부터 대미지를 받아 체력을 감소시킵니다.
    /// </summary>
    public void TakeDamage(int damage)
    {
        // 이미 죽었다면 대미지 받지 않음
        if (_isDying) return;

        _currentHealth -= damage;
        Debug.Log($"Enemy {gameObject.name} took {damage} damage. Current Health: {_currentHealth}/{MaxHealth}");

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 적 몬스터가 파괴될 때 호출되는 메서드입니다. (맵 이탈, 체력 0 등)
    /// </summary>
    public void Die()
    {
        // 이미 죽었다면 중복 실행 방지
        if (_isDying) return;
        _isDying = true;

        Debug.Log($"Enemy {gameObject.name} died!");

        // ▼▼▼ [수정된 부분] 아이템 드랍 로직 ▼▼▼
        TryDropItem();
        // ▲▲▲ [수정된 부분] ▲▲▲

        // 여기에 파괴 이펙트, 사운드, 점수 추가 로직을 넣을 수 있습니다.
        Destroy(gameObject);
    }

    /// <summary>
    /// 설정된 확률에 따라 아이템을 드랍할지 시도합니다.
    /// </summary>
    private void TryDropItem()
    {
        // 1. 50% 확률로 아이템 드랍 시도
        if (Random.value <= ItemDropChance) // Random.value는 0.0 ~ 1.0 사이의 난수
        {
            // 2. 드랍하기로 결정됨. 이제 어떤 아이템인지 결정 (70/20/10)
            GameObject itemToDrop = null;
            float itemRoll = Random.value; // 0.0 ~ 1.0 사이의 난수

            if (itemRoll <= SpeedItemChance) // 0.0 ~ 0.7 (70%)
            {
                itemToDrop = SpeedItemPrefab;
            }
            // 0.7 ~ 0.9 (20%)
            else if (itemRoll <= SpeedItemChance + HealthItemChance)
            {
                itemToDrop = HealthItemPrefab;
            }
            else // 0.9 ~ 1.0 (10%)
            {
                itemToDrop = AttackSpeedItemPrefab;
            }

            // 3. 선택된 아이템을 현재 위치에 생성
            if (itemToDrop != null)
            {
                Instantiate(itemToDrop, transform.position, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("Enemy: 아이템을 드랍하려 했으나, 할당된 아이템 프리팹이 없습니다.");
            }
        }
    }

    /// <summary>
    /// 현재 사망/소멸 처리 중인지 여부를 반환합니다.
    /// </summary>
    public bool IsDying()
    {
        return _isDying;
    }
}