using UnityEngine;

/// <summary>
/// 총알의 이동 및 소멸을 담당하는 스크립트입니다.
/// 총알은 생성된 방향으로 가속하여 전진하며, 지정된 맵 경계를 벗어나면 자동으로 소멸합니다.
/// </summary>
public class Bullet : MonoBehaviour
{
    [Header("총알 능력치")]
    public float InitialSpeed = 1f;       // 총알 발사 시 초기 속도
    public float FinalSpeed = 7f;         // 총알이 도달할 최종 속도
    public float AccelerationTime = 1.2f; // 초기 속도에서 최종 속도까지 도달하는 데 걸리는 시간

    private float _currentSpeed;          // 현재 총알의 이동 속도 (내부적으로 계속 업데이트)
    private float _elapsedTime = 0f;      // 총알이 생성된 후 경과된 시간

    [Header("소멸 범위")]
    // 이 값들은 플레이어 이동 범위보다 약간 더 넓게 설정하여
    // 화면 밖으로 충분히 나갔을 때 소멸되도록 합니다.
    public float DestroyBoundaryX = 10f; // 총알이 사라질 X축 경계 (절대값)
    public float DestroyBoundaryY = 6f;  // 총알이 사라질 Y축 경계 (절대값)

    /// <summary>
    /// MonoBehaviour가 생성된 후 첫 번째 Update 호출 전에 한 번 호출됩니다.
    /// 총알의 초기 속도를 설정합니다.
    /// </summary>
    void Start()
    {
        _currentSpeed = InitialSpeed; // 총알의 초기 속도를 설정합니다.
    }

    /// <summary>
    /// 매 프레임마다 호출되어 총알의 가속 이동 및 소멸 로직을 처리합니다.
    /// </summary>
    void Update()
    {
        // 경과된 시간을 업데이트합니다.
        _elapsedTime += Time.deltaTime;

        // 경과 시간에 따라 총알의 현재 속도를 보간(Lerp)하여 가속을 적용합니다.
        // _elapsedTime이 AccelerationTime을 초과하면 FinalSpeed에 고정됩니다.
        _currentSpeed = Mathf.Lerp(InitialSpeed, FinalSpeed, _elapsedTime / AccelerationTime);

        // 1. 총알 이동
        // transform.up은 현재 게임 오브젝트의 '위쪽' 방향(Local Y-axis)을 나타냅니다.
        // 총알이 이 방향으로 _currentSpeed 만큼 이동하도록 합니다.
        // Time.deltaTime을 곱하여 프레임 속도에 독립적인 이동을 보장합니다.
        transform.Translate(Vector2.up * _currentSpeed * Time.deltaTime);

        // 2. 맵 바깥으로 나갔는지 확인 후 소멸
        // 총알의 현재 위치가 설정된 소멸 경계를 벗어났는지 확인합니다.
        // Mathf.Abs를 사용하여 X, Y 좌표의 절대값이 경계값보다 큰지 검사합니다.
        if (Mathf.Abs(transform.position.x) > DestroyBoundaryX ||
            Mathf.Abs(transform.position.y) > DestroyBoundaryY)
        {
            // 경계를 벗어났다면 현재 총알 게임 오브젝트를 파괴합니다.
            Destroy(gameObject);
        }
    }
}
