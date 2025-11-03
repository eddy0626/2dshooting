using UnityEngine;


// 플레이어 이동
public class PlayermOVE : MonoBehaviour
{
    // 목표
    // "키보드 입력"에 따라 "방향"을 구하고 그 방향으로 이동시키고 싶다.

    // 구현 순서 :
    // 1. 키보드 입력
    // 2. 방향 구하는 방법
    // 3. 이동

    // 필요 속성 :
    public float Speed = 3;

    // 게임 오브젝트가 게임을 시작 후 최대한 많이
    void Update()
    {
        // 1. 키보드 입력을 감지한다.
        // 유니티에서는 Input이라고 하는 모듈이 입력에 관한 모든것을 담당하다.
        float h = Input.GetAxis("Horizontal"); // 수평 입력에 대한 값을 -1, 0, 1로 가져온다.
        float v = Input.GetAxis("Vertical");   // 수직 입력에 대한 값을 -1, 0, 1로 가져온다.
        Debug.Log($"h: {h}, v:{v}");

        // 2. 입력으로부터 방향을 구한다.
        // 백터 : 크기와 방향을 표현하는 물리 개념
        Vector2 direction = new Vector2(h, v);
        Debug.Log ($"direction: {direction.x}, {direction.y}");

        // 3. 그 방향으로 이동한다.
        Vector2 postion = this.transform.position; // 현재 위치

        // 새로운 위치 = 현재 위치 + (방향 * 속력) * 시간
        // 새로운 위치 = 현재 위치 + 속도 * 시간
        //       새로운 위치 = 현재 위치  방향      속력
        Vector2 newPostion = postion + direction * Speed * Time.deltaTime;  // 새로운 위치


        //Time.deltaTime : 이전 프레임으로부터 현재 프레임까지 시간이 얼마나 흘렀는지 나타내는 값
        //                 1초/ fps 값과 비슷하다

        // 이동속도 : 10
        // 컴퓨터1 : 50FPS : Update -> 초당 50번 실행 -> 10 * 50 = 500 * Time.deltaTime = 두개의 값이 같아진다.
        // 컴퓨터2 : 100FPS : -> 초당 100번 실행 -> 10 * 100 = 1000 * Time.deltaTime

        transform.position = newPostion;           //새로운 위치로 갱신
    }
}
