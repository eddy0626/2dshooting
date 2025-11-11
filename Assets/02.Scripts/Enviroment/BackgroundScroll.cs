using UnityEngine;

/// <summary>
/// 배경을 무한으로 스크롤시켜 플레이어가 앞으로 나아가는 느낌을 줍니다.
/// 배경 이미지가 아래로 이동하다가 화면 밖으로 나가면 다시 위로 재배치됩니다.
/// </summary>
public class BackgroundScroll : MonoBehaviour
{
    [Header("스크롤 설정")]
    [SerializeField] private float ScrollSpeed = 2f; // 배경 스크롤 속도

    [Header("재배치 설정")]
    [SerializeField] private float BackgroundHeight = 20f; // 배경 이미지의 세로 길이 (Unity 유닛)
    [SerializeField] private float ResetPositionY = 20f;   // 재배치될 위치 (Y 좌표)
    [SerializeField] private float DestroyPositionY = -20f; // 재배치 트리거 위치 (Y 좌표)

    [Header("다중 배경 (선택사항)")]
    [SerializeField] private Transform[] BackgroundObjects; // 여러 배경 오브젝트들 (2개 이상 권장)

    /// <summary>
    /// 초기화
    /// </summary>
    void Start()
    {
        // BackgroundObjects가 할당되지 않았다면, 자식 오브젝트들을 자동으로 찾음
        if (BackgroundObjects == null || BackgroundObjects.Length == 0)
        {
            int childCount = transform.childCount;
            if (childCount > 0)
            {
                BackgroundObjects = new Transform[childCount];
                for (int i = 0; i < childCount; i++)
                {
                    BackgroundObjects[i] = transform.GetChild(i);
                }
                Debug.Log($"BackgroundScroll: {childCount}개의 배경 오브젝트를 자동으로 찾았습니다.");
            }
            else
            {
                Debug.LogWarning("BackgroundScroll: 배경 오브젝트가 없습니다. 자식 오브젝트를 추가하거나 BackgroundObjects를 할당하세요.");
            }
        }
    }

    /// <summary>
    /// 매 프레임 배경을 아래로 스크롤
    /// </summary>
    void Update()
    {
        // 배경 오브젝트들을 아래로 이동
        if (BackgroundObjects != null && BackgroundObjects.Length > 0)
        {
            foreach (Transform bg in BackgroundObjects)
            {
                if (bg == null) continue;

                // 아래로 이동
                bg.position += Vector3.down * ScrollSpeed * Time.deltaTime;

                // 화면 아래로 완전히 벗어났는지 체크
                if (bg.position.y <= DestroyPositionY)
                {
                    // 위쪽으로 재배치
                    Vector3 newPosition = bg.position;
                    newPosition.y = ResetPositionY;
                    bg.position = newPosition;
                }
            }
        }
        else
        {
            // 단일 배경 모드 (이 스크립트가 붙은 오브젝트 자체를 스크롤)
            transform.position += Vector3.down * ScrollSpeed * Time.deltaTime;

            // 화면 아래로 벗어나면 위로 재배치
            if (transform.position.y <= DestroyPositionY)
            {
                Vector3 newPosition = transform.position;
                newPosition.y = ResetPositionY;
                transform.position = newPosition;
            }
        }
    }
}
