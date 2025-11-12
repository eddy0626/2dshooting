using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 통합 배경 무한 스크롤 시스템
/// - 여러 개의 배경을 자동으로 감지하여 무한 스크롤
/// - 3개로 분할된 배경 이미지를 하나로 합쳐서 사용
/// - 자동 배치 및 재배치 기능
/// </summary>
public class UnifiedBackgroundScroll : MonoBehaviour
{
    [Header("스크롤 설정")]
    [Tooltip("배경이 아래로 스크롤되는 속도")]
    [SerializeField] private float scrollSpeed = 3f;

    [Header("배경 오브젝트 (자동 감지)")]
    [Tooltip("비워두면 자식 오브젝트들을 자동으로 감지합니다")]
    [SerializeField] private List<Transform> backgrounds = new List<Transform>();

    [Header("고급 설정")]
    [Tooltip("배경 높이 (자동 계산되지만 수동 설정 가능)")]
    [SerializeField] private float backgroundHeight = 0f;
    
    [Tooltip("자동 배치 활성화 (시작 시 배경을 세로로 정렬)")]
    [SerializeField] private bool autoArrange = true;

    private Camera mainCamera;
    private bool isInitialized = false;

    void Start()
    {
        Initialize();
    }

    /// <summary>
    /// 초기화 - 배경 감지 및 배치
    /// </summary>
    void Initialize()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("UnifiedBackgroundScroll: Main Camera를 찾을 수 없습니다!");
            return;
        }

        // 배경이 설정되지 않았다면 자식 오브젝트들을 자동으로 추가
        if (backgrounds.Count == 0)
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf)
                {
                    backgrounds.Add(child);
                }
            }
            Debug.Log($"[배경 스크롤] {backgrounds.Count}개의 배경 오브젝트를 자동으로 감지했습니다.");
        }

        if (backgrounds.Count == 0)
        {
            Debug.LogWarning("[배경 스크롤] 배경 오브젝트가 없습니다!");
            return;
        }

        // 배경 높이 계산
        if (backgroundHeight <= 0)
        {
            CalculateBackgroundHeight();
        }

        // 자동 배치
        if (autoArrange)
        {
            ArrangeBackgrounds();
        }

        isInitialized = true;
        Debug.Log($"[배경 스크롤] 초기화 완료! 속도: {scrollSpeed}, 높이: {backgroundHeight}");
    }

    /// <summary>
    /// 배경 높이 자동 계산
    /// </summary>
    void CalculateBackgroundHeight()
    {
        if (backgrounds.Count == 0) return;

        Transform firstBg = backgrounds[0];

        // SpriteRenderer가 있는 경우
        SpriteRenderer sr = firstBg.GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
        {
            backgroundHeight = sr.bounds.size.y;
            Debug.Log($"[배경 스크롤] SpriteRenderer로부터 높이 계산: {backgroundHeight}");
            return;
        }

        // RectTransform이 있는 경우 (UI)
        RectTransform rt = firstBg.GetComponent<RectTransform>();
        if (rt != null)
        {
            backgroundHeight = rt.rect.height * rt.lossyScale.y;
            Debug.Log($"[배경 스크롤] RectTransform으로부터 높이 계산: {backgroundHeight}");
            return;
        }

        // 기본값
        backgroundHeight = 12f;
        Debug.LogWarning($"[배경 스크롤] 배경 높이를 자동으로 계산할 수 없어 기본값({backgroundHeight})을 사용합니다.");
    }

    /// <summary>
    /// 배경들을 세로로 이어붙이기
    /// </summary>
    void ArrangeBackgrounds()
    {
        if (backgrounds.Count == 0) return;

        // 첫 번째 배경의 Y 위치를 기준으로 함
        float baseY = backgrounds[0].position.y;

        // 나머지 배경들을 위쪽에 연속으로 배치
        for (int i = 1; i < backgrounds.Count; i++)
        {
            Vector3 newPos = backgrounds[i].position;
            newPos.y = baseY + (backgroundHeight * i);
            backgrounds[i].position = newPos;
            Debug.Log($"[배경 스크롤] {backgrounds[i].name} 배치: Y = {newPos.y:F2}");
        }
    }

    void Update()
    {
        if (!isInitialized || backgrounds.Count == 0) return;

        // 모든 배경을 아래로 이동
        ScrollBackgrounds();

        // 화면 밖으로 나간 배경 재배치
        RepositionBackgrounds();
    }

    /// <summary>
    /// 모든 배경을 아래로 스크롤
    /// </summary>
    void ScrollBackgrounds()
    {
        float moveDistance = scrollSpeed * Time.deltaTime;
        
        foreach (Transform bg in backgrounds)
        {
            if (bg == null) continue;
            bg.position += Vector3.down * moveDistance;
        }
    }

    /// <summary>
    /// 화면 밖으로 나간 배경을 맨 위로 재배치
    /// </summary>
    void RepositionBackgrounds()
    {
        // 카메라 아래쪽 경계 계산 (여유 공간 포함)
        float cameraBottomEdge = mainCamera.transform.position.y - mainCamera.orthographicSize - backgroundHeight;

        // 가장 아래쪽 배경 찾기
        Transform bottommost = GetBottommostBackground();
        
        // 화면 밖으로 나갔는지 확인
        if (bottommost.position.y < cameraBottomEdge)
        {
            // 가장 위쪽 배경 찾기
            Transform topmost = GetTopmostBackground();

            // 아래쪽 배경을 위쪽 끝으로 이동
            Vector3 newPos = bottommost.position;
            newPos.y = topmost.position.y + backgroundHeight;
            bottommost.position = newPos;

            Debug.Log($"[배경 스크롤] {bottommost.name} 재배치: Y = {newPos.y:F2}");
        }
    }

    /// <summary>
    /// 가장 아래쪽 배경 찾기
    /// </summary>
    Transform GetBottommostBackground()
    {
        Transform bottommost = backgrounds[0];
        foreach (Transform bg in backgrounds)
        {
            if (bg != null && bg.position.y < bottommost.position.y)
            {
                bottommost = bg;
            }
        }
        return bottommost;
    }

    /// <summary>
    /// 가장 위쪽 배경 찾기
    /// </summary>
    Transform GetTopmostBackground()
    {
        Transform topmost = backgrounds[0];
        foreach (Transform bg in backgrounds)
        {
            if (bg != null && bg.position.y > topmost.position.y)
            {
                topmost = bg;
            }
        }
        return topmost;
    }

    /// <summary>
    /// 스크롤 속도 변경 (외부 호출용)
    /// </summary>
    public void SetScrollSpeed(float speed)
    {
        scrollSpeed = Mathf.Max(0, speed);
    }

    /// <summary>
    /// 스크롤 일시정지/재개
    /// </summary>
    public void SetScrollEnabled(bool enabled)
    {
        this.enabled = enabled;
    }

    /// <summary>
    /// 배경 수동 재정렬
    /// </summary>
    public void RearrangeBackgrounds()
    {
        ArrangeBackgrounds();
    }

    // Inspector에서 값 변경 시 유효성 검사
    void OnValidate()
    {
        if (scrollSpeed < 0)
            scrollSpeed = 0;
        
        if (backgroundHeight < 0)
            backgroundHeight = 0;
    }

    // 에디터에서 배경 영역 시각화 (Scene 뷰)
    void OnDrawGizmos()
    {
        if (backgrounds == null || backgrounds.Count == 0) return;

        Gizmos.color = Color.cyan;
        
        foreach (Transform bg in backgrounds)
        {
            if (bg == null) continue;
            
            float height = backgroundHeight > 0 ? backgroundHeight : 12f;
            Gizmos.DrawWireCube(bg.position, new Vector3(20, height, 0));
        }
    }
}