using UnityEngine;


/// <summary>
/// 플레이어의 이동을 담당하는 스크립트입니다.
/// 키보드 입력에 따라 플레이어를 이동시키고, 지정된 영역 내에서만 움직임을 허용하며,
/// Q/E 키로 속도 조절, Shift 키로 일시적인 속도 증가, R 키로 원점 복귀 기능을 제공합니다.
/// </summary>
public class PlayerMove : MonoBehaviour
{
    // 목표 설명:
    // 1. 키보드 입력에 따라 플레이어를 이동시킵니다.
    // 2. 지정된 이동 범위 내에서만 플레이어가 움직이도록 합니다. (워프 기능 포함)
    // 3. Q/E 키로 플레이어의 기본 이동 속도를 조절합니다.
    // 4. Shift 키를 누르는 동안 이동 속도를 증가시킵니다.
    // 5. R 키를 길게 누르면 플레이어를 서서히 원점으로 복귀시킵니다.

    [Header("능력치")]
    public float Speed = 3f;             // 현재 플레이어의 기본 이동 속도
    public float maxSpeed = 10f;        // 플레이어의 최대 이동 속도
    public float minSpeed = 1f;         // 플레이어의 최소 이동 속도
    public float speedIncrement = 0.5f; // Q/E 키를 눌렀을 때 속도 증감량
    public float shiftSpeedMultiplier = 1.2f; // Shift 키를 눌렀을 때 기본 속도에 곱해지는 배율
    public float returnToOriginSpeed = 2f;    // R 키를 눌렀을 때 원점으로 돌아가는 속도

    // 이동 범위 (Hierarchy 창에서 조절 가능하도록 public으로 설정)
    [Header("이동범위")]
    public float minX = -8f;            // 이동 가능한 최소 X 좌표
    public float maxX = 8f;             // 이동 가능한 최대 X 좌표
    public float minY = -4.5f;          // 이동 가능한 최소 Y 좌표
    public float maxY = 4.5f;           // 이동 가능한 최대 Y 좌표

    /// <summary>
    /// 매 프레임마다 호출되어 플레이어의 이동과 관련된 로직을 처리합니다.
    /// 키보드 입력 감지, 속도 계산, 이동 및 워프, 속도 조절 기능을 포함합니다.
    /// </summary>
    void Update()
    {
        // R 키가 눌렸는지 확인하고, 눌렸다면 원점 복귀 로직을 수행합니다.
        if (Input.GetKey(KeyCode.R))
        {
            HandleReturnToOrigin();
        }
        else // R 키가 눌리지 않았을 때만 일반 이동 및 워프 로직을 수행합니다.
        {
            // 현재 프레임에 적용될 실제 이동 속도를 계산합니다 (Shift 버스트 포함).
            float effectiveSpeed = GetEffectiveSpeed();

            // 키보드 입력에 따라 플레이어를 이동시킵니다.
            HandleManualMovement(effectiveSpeed);
        }

        // 플레이어 위치가 이동 범위를 벗어나면 반대편으로 워프시킵니다.
        ApplyBoundaryWarp();

        // Q/E 키를 이용한 이동 속도 조절을 처리합니다.
        HandleSpeedAdjustments();
    }

    /// <summary>
    /// 현재 유효 이동 속도를 계산합니다. Shift 키가 눌려있으면 속도 배율을 적용합니다.
    /// </summary>
    /// <returns>적용될 최종 이동 속도</returns>
    private float GetEffectiveSpeed()
    {
        float effectiveSpeed = Speed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            effectiveSpeed *= shiftSpeedMultiplier; // Shift 키가 눌려있으면 속도 증가
        }
        return effectiveSpeed;
    }

    /// <summary>
    /// 키보드 (WASD 또는 화살표) 입력을 받아 플레이어를 이동시킵니다.
    /// </summary>
    /// <param name="effectiveSpeed">현재 적용될 이동 속도</param>
    private void HandleManualMovement(float effectiveSpeed)
    {
        // 수평 및 수직 입력 값을 가져옵니다.
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // 입력으로부터 방향 벡터를 생성하고 정규화합니다.
        // 정규화는 대각선 이동 시 속도가 빨라지는 것을 방지합니다.
        Vector2 direction = new Vector2(horizontalInput, verticalInput).normalized;

        // 새로운 위치를 계산합니다: 현재 위치 + (방향 * 유효 속도 * Time.deltaTime)
        Vector2 newPosition = (Vector2)transform.position + direction * effectiveSpeed * Time.deltaTime;
        transform.position = newPosition; // 계산된 새로운 위치로 플레이어를 이동시킵니다.
    }

    /// <summary>
    /// 'R' 키가 눌렸을 때 플레이어를 서서히 원점(0,0,0)으로 이동시킵니다.
    /// </summary>
    private void HandleReturnToOrigin()
    {
        // 플레이어의 현재 위치에서 원점까지의 방향을 계산합니다.
        Vector3 directionToOrigin = (Vector3.zero - transform.position).normalized;
        // transform.Translate를 사용하여 월드 좌표계에서 원점을 향해 이동합니다.
        transform.Translate(directionToOrigin * returnToOriginSpeed * Time.deltaTime, Space.World);
    }

    /// <summary>
    /// 플레이어가 이동 범위를 벗어나면 반대편에서 나타나도록 워프 효과를 적용합니다.
    /// </summary>
    private void ApplyBoundaryWarp()
    {
        Vector3 currentPosition = transform.position; // 현재 플레이어 위치

        // X축 워프 처리
        if (currentPosition.x < minX) // 왼쪽 경계를 벗어나면
        {
            currentPosition.x = maxX; // 오른쪽 끝으로 워프
        }
        else if (currentPosition.x > maxX) // 오른쪽 경계를 벗어나면
        {
            currentPosition.x = minX; // 왼쪽 끝으로 워프
        }

        // Y축 워프 처리
        if (currentPosition.y < minY) // 아래쪽 경계를 벗어나면
        {
            currentPosition.y = maxY; // 위쪽 끝으로 워프
        }
        else if (currentPosition.y > maxY) // 위쪽 경계를 벗어나면
        {
            currentPosition.y = minY; // 아래쪽 끝으로 워프
        }

        transform.position = currentPosition; // 워프가 적용된 최종 위치로 갱신
    }

    /// <summary>
    /// 'Q' 및 'E' 키 입력을 통해 플레이어의 기본 이동 속도를 조절합니다.
    /// </summary>
    private void HandleSpeedAdjustments()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Q 키가 눌렸을 때 (속도 증가)
        {
            // 현재 속도를 speedIncrement만큼 증가시키고 maxSpeed를 초과하지 않도록 제한합니다.
            Speed = Mathf.Min(Speed + speedIncrement, maxSpeed);
            Debug.Log($"Speed increased to: {Speed}"); // 디버그 로그 출력
        }
        if (Input.GetKeyDown(KeyCode.E)) // E 키가 눌렸을 때 (속도 감소)
        {
            // 현재 속도를 speedIncrement만큼 감소시키고 minSpeed 미만으로 내려가지 않도록 제한합니다.
            Speed = Mathf.Max(Speed - speedIncrement, minSpeed);
            Debug.Log($"Speed decreased to: {Speed}"); // 디버그 로그 출력
        }
    }
}
