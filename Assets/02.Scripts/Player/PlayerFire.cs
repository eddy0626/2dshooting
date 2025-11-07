using UnityEngine;
using System.Collections.Generic; // List를 사용하기 위해 추가

/// <summary>
/// 플레이어의 총알 발사를 담당하는 스크립트입니다.
/// 스페이스바를 누르거나 자동 공격 모드에서 설정된 총알 프리팹들을 생성하여 발사합니다.
/// 발사에는 쿨타임이 적용되며, 숫자 키 1 또는 2를 통해 자동/수동 공격 모드를 전환할 수 있습니다.
/// </summary>
public class PlayerFire : MonoBehaviour
{
    [Header("총알 프리팹")]
    public GameObject BulletPrefab; // 기본으로 발사할 총알 게임 오브젝트의 프리팹
    public GameObject[] AdditionalBulletPrefabs; // 추가로 발사할 총알 게임 오브젝트들의 프리팹 배열

    [Header("총구")]
    public Transform FirePosition;  // 기본 총알이 생성될 위치와 방향을 나타내는 Transform

    [Header("보조 총구")]
    public Transform SubFirePosition1; // 보조 총알이 생성될 첫 번째 총구 위치와 방향
    public Transform SubFirePosition2; // 보조 총알이 생성될 두 번째 총구 위치와 방향

    [Header("발사 설정")]
    public float FireCooldown = 0.6f; // 총알 발사 간의 최소 시간 (쿨타임)
    public float MinFireCooldown = 0.1f; // 최소 발사 쿨타임 (아이템으로 이보다 빠르게 할 수 없음)

    private float _lastFireTime = 0f; // 마지막으로 총알을 발사한 게임 시간
    private bool _isAutoFire = true;  // 자동 공격 모드 활성화 여부 (기본값: true)

    /// <summary>
    /// MonoBehaviour가 생성된 후 첫 번째 Update 호출 전에 한 번 호출됩니다.
    /// </summary>
    void Start()
    {
        _isAutoFire = true;
        _lastFireTime = -FireCooldown; // 게임 시작 즉시 발사 가능하도록 초기화
    }

    /// <summary>
    /// 매 프레임마다 호출되어 플레이어의 총알 발사 입력을 감지하고 처리합니다.
    /// </summary>
    private void Update()
    {
        // 자동 공격 모드 전환 키 입력 처리
        HandleAutoFireToggle();

        // 발사 조건 확인 및 총알 발사 처리
        HandleFireInput();
    }

    /// <summary>
    /// 숫자 키 1 또는 2를 눌러 자동 공격 모드를 전환합니다.
    /// </summary>
    private void HandleAutoFireToggle()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) // 숫자 1 키를 누르면
        {
            _isAutoFire = true; // 자동 공격 모드 활성화
            Debug.Log("Auto Fire ON");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) // 숫자 2 키를 누르면
        {
            _isAutoFire = false; // 자동 공격 모드 비활성화
            Debug.Log("Auto Fire OFF (Hold Space to fire)");
        }
    }

    /// <summary>
    /// 플레이어의 발사 입력을 감지하고, 쿨타임을 확인하여 총알을 발사합니다.
    /// 자동 공격 모드일 경우 계속 발사하며, 아닐 경우 스페이스바 입력을 기다립니다.
    /// </summary>
    private void HandleFireInput()
    {
        // 쿨타임이 지났는지 확인합니다.
        if (Time.time >= _lastFireTime + FireCooldown)
        {
            // 자동 공격 모드이거나, 스페이스바를 '누르고 있을' 경우 발사합니다.
            // ▼▼▼▼▼ [수정된 부분] ▼▼▼▼▼
            if (_isAutoFire || Input.GetKey(KeyCode.Space)) // GetKeyDown -> GetKey
            // ▲▲▲▲▲ [수정된 부분] ▲▲▲▲▲
            {
                FireAllBullets(); // 모든 총알 발사 메서드 호출
                _lastFireTime = Time.time; // 마지막 발사 시간 업데이트
            }
        }
    }

    /// <summary>
    /// 기본 총알 및 추가 총알 프리팹들을 생성하여 지정된 총구 위치와 방향에서 발사합니다.
    /// </summary>
    private void FireAllBullets()
    {
        // 1. 기본 총알 발사 (메인 총구에서)
        if (BulletPrefab != null)
        {
            Instantiate(BulletPrefab, FirePosition.position, FirePosition.rotation);
        }
        else
        {
            Debug.LogWarning("PlayerFire: BulletPrefab이 할당되지 않았습니다.");
        }

        // 2. 추가 총알들 발사 (보조 총구들에서)
        if (AdditionalBulletPrefabs != null && AdditionalBulletPrefabs.Length > 0)
        {
            // 활성화된 보조 총구 리스트를 만듭니다.
            List<Transform> activeSubFirePositions = new List<Transform>();
            if (SubFirePosition1 != null)
            {
                activeSubFirePositions.Add(SubFirePosition1);
            }
            if (SubFirePosition2 != null)
            {
                activeSubFirePositions.Add(SubFirePosition2);
            }

            if (activeSubFirePositions.Count == 0)
            {
                Debug.LogWarning("PlayerFire: AdditionalBulletPrefabs이 있지만 할당된 보조 총구가 없습니다. 총알이 발사되지 않습니다.");
                return;
            }

            // 각 추가 총알 프리팹을 활성화된 모든 보조 총구에서 발사합니다.
            foreach (GameObject additionalBulletPrefab in AdditionalBulletPrefabs)
            {
                if (additionalBulletPrefab != null)
                {
                    foreach (Transform subFirePos in activeSubFirePositions)
                    {
                        Instantiate(additionalBulletPrefab, subFirePos.position, subFirePos.rotation);
                    }
                }
                else
                {
                    Debug.LogWarning("PlayerFire: AdditionalBulletPrefabs 배열에 null 요소가 있습니다. 확인해 주세요.");
                }
            }
        }
    }
}