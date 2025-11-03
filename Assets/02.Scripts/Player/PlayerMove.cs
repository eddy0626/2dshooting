using UnityEngine;


// 플레이어 이동
public class PlayerMove : MonoBehaviour // 클래스 이름 PlayermOVE를 PlayerMove로 수정하여 파일 이름과 일치시킵니다.
{
    // 목표
    // "키보드 입력"에 따라 "방향"을 구하고 그 방향으로 이동시키고 싶다.
    // 추가 목표: 지정된 영역 내에서만 이동하며, Q/E 키로 속도를 조절한다.
    // 최종 목표: 화면 양쪽 끝으로 가면 반대쪽 끝에서 다시 나타나는 워프 기능을 추가한다.


    // 구현 순서 :
    // 1. 키보드 입력
    // 2. 방향 구하는 방법
    // 3. 이동 및 워프 처리
    // 4. 속도 조절 (Q, E 키)

    // 필요 속성 :
    [Header("능력치")]
    public float Speed = 3;             // 현재 이동 속도
    public float maxSpeed = 10f;        // 플레이어의 최대 이동 속도
    public float minSpeed = 1f;         // 플레이어의 최소 이동 속도
    public float speedIncrement = 0.5f; // Q/E 키를 눌렀을 때 속도 증감량

    // 이동 범위 (Hierarchy 창에서 조절 가능하도록 public으로 설정)
    [Header("이동범위")]
    public float minX = -8f;            // 이동 가능한 최소 X 좌표       
    public float maxX = 8f;             // 이동 가능한 최대 X 좌표
    public float minY = -4.5f;          // 이동 가능한 최소 Y 좌표
    public float maxY = 4.5f;           // 이동 가능한 최대 Y 좌표

    // 게임 오브젝트가 게임을 시작 후 최대한 많이
    void Update()
    {
        // 1. 키보드 입력을 감지한다.
        // 유니티에서는 Input이라고 하는 모듈이 입력에 관한 모든것을 담당하다.
        float h = Input.GetAxis("Horizontal"); // 수평 입력에 대한 값을 -1, 0, 1로 가져온다. (A/D 또는 왼쪽/오른쪽 화살표)
        float v = Input.GetAxis("Vertical");   // 수직 입력에 대한 값을 -1, 0, 1로 가져온다. (W/S 또는 위/아래 화살표)
        // Debug.Log($"h: {h}, v:{v}"); // 디버그 로그는 게임 실행 중 필요에 따라 주석 처리하거나 제거할 수 있습니다.

        // 2. 입력으로부터 방향을 구한다.
        // 백터 : 크기와 방향을 표현하는 물리 개념
        Vector2 direction = new Vector2(h, v).normalized; // 방향 벡터를 정규화하여 대각선 이동 시에도 속도가 일정하게 유지되도록 합니다.
        // Debug.Log ($"direction: {direction.x}, {direction.y}"); // 디버그 로그는 게임 실행 중 필요에 따라 주석 처리하거나 제거할 수 있습니다.

        // 3. 그 방향으로 이동한다.
        Vector2 currentPosition = this.transform.position; // 현재 위치

        // 새로운 위치 = 현재 위치 + (방향 * 속력) * 시간
        Vector2 newPosition = currentPosition + direction * Speed * Time.deltaTime;  // 프레임 독립적인 새로운 위치 계산


        //Time.deltaTime : 이전 프레임으로부터 현재 프레임까지 시간이 얼마나 흘렀는지 나타내는 값
        //                 1초/ fps 값과 비슷하다

        // 이동속도 : 10
        // 컴퓨터1 : 50FPS : Update -> 초당 50번 실행 -> 10 * 50 = 500 * Time.deltaTime = 두개의 값이 같아진다.
        // 컴퓨터2 : 100FPS : -> 초당 100번 실행 -> 10 * 100 = 1000 * Time.deltaTime

        // 3.1. 이동 제한 영역을 벗어나면 반대편에서 나타나도록 처리 (워프 기능)
        // 기존의 Clamp 로직을 제거하고, 경계를 벗어나는 경우 위치를 재설정합니다.
        if (newPosition.x < minX) // 왼쪽 경계를 벗어나면
        {
            newPosition.x = maxX; // 오른쪽 끝으로 워프
        }
        else if (newPosition.x > maxX) // 오른쪽 경계를 벗어나면
        {
            newPosition.x = minX; // 왼쪽 끝으로 워프
        }

        if (newPosition.y < minY) // 아래쪽 경계를 벗어나면
        {
            newPosition.y = maxY; // 위쪽 끝으로 워프
        }
        else if (newPosition.y > maxY) // 위쪽 경계를 벗어나면
        {
            newPosition.y = minY; // 아래쪽 끝으로 워프
        }

        transform.position = newPosition;           // 새로운 위치로 갱신

        // 4. 스피드 조작
        if (Input.GetKeyDown(KeyCode.Q)) // Q 키가 눌렸을 때
        {
            // 현재 속도를 speedIncrement만큼 증가시키고 maxSpeed를 초과하지 않도록 제한합니다.
            Speed = Mathf.Min(Speed + speedIncrement, maxSpeed);
            Debug.Log($"Speed increased to: {Speed}"); // 속도 증가를 디버그 로그로 출력
        }
        if (Input.GetKeyDown(KeyCode.E)) // E 키가 눌렸을 때
        {
            // 현재 속도를 speedIncrement만큼 감소시키고 minSpeed 미만으로 내려가지 않도록 제한합니다.
            Speed = Mathf.Max(Speed - speedIncrement, minSpeed);
            Debug.Log($"Speed decreased to: {Speed}"); // 속도 감소를 디버그 로그로 출력
        }
    }
}
