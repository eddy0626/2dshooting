using UnityEngine;

/// <summary>
/// 적의 총알 발사를 담당하는 스크립트입니다.
/// 일정 주기로 플레이어를 향해 총알을 발사합니다.
/// </summary>
[RequireComponent(typeof(Enemy))]
public class EnemyFire : MonoBehaviour
{
    [Header("총알 프리팹")]
    public GameObject BulletPrefab; // 적 총알 프리팹

    [Header("총구")]
    public Transform FirePosition; // 총알이 생성될 위치

    [Header("발사 설정")]
    public float FireCooldown = 2.0f; // 총알 발사 쿨타임 (초)
    public float FireDelay = 1.0f;    // 최초 발사 지연 시간 (초)

    private float _lastFireTime;
    private Enemy _enemy;
    private Transform _playerTransform;
    private bool _hasStartedFiring = false;

    void Start()
    {
        _enemy = GetComponent<Enemy>();

        // 플레이어 찾기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("EnemyFire: 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다!");
        }

        // 최초 발사 시간 설정 (생성 후 FireDelay 초 후)
        _lastFireTime = Time.time + FireDelay - FireCooldown;
    }

    void Update()
    {
        // 죽었거나 플레이어를 찾지 못했으면 발사 안 함
        if (_enemy.IsDying() || _playerTransform == null)
            return;

        // 쿨타임 체크 및 발사
        if (Time.time >= _lastFireTime + FireCooldown)
        {
            FireBullet();
            _lastFireTime = Time.time;
        }
    }

    /// <summary>
    /// 플레이어를 향해 총알을 발사합니다.
    /// </summary>
    void FireBullet()
    {
        if (BulletPrefab == null || FirePosition == null)
        {
            Debug.LogWarning("EnemyFire: BulletPrefab 또는 FirePosition이 설정되지 않았습니다.");
            return;
        }

        // 총알 생성
        GameObject bullet = Instantiate(BulletPrefab, FirePosition.position, Quaternion.identity);

        // 플레이어를 향하는 방향 계산
        Vector2 direction = (_playerTransform.position - FirePosition.position).normalized;

        // 총알 회전 (방향에 맞게)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

        // 총알에 Owner 설정
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Owner = BulletOwner.Enemy;
        }

        Debug.Log($"Enemy {gameObject.name} fired a bullet towards player!");
    }
}