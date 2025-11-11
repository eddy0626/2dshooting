using UnityEngine;
using System.Collections.Generic;

public class VerticalBackgroundScroll : MonoBehaviour
{
    [Header("스크롤 설정")]
    [SerializeField] private float scrollSpeed = 3f;
    
    [Header("배경 설정")]
    [SerializeField] private List<Transform> backgrounds = new List<Transform>();
    
    private Camera mainCamera;
    private float backgroundHeight;

    void Start()
    {
        mainCamera = Camera.main;
        
        // 배경이 설정되지 않았다면 자식 오브젝트들을 자동으로 추가
        if (backgrounds.Count == 0)
        {
            foreach (Transform child in transform)
            {
                backgrounds.Add(child);
            }
            Debug.Log($"VerticalBackgroundScroll: {backgrounds.Count}개의 배경 오브젝트를 자동으로 찾았습니다.");
        }
        
        if (backgrounds.Count < 2)
        {
            Debug.LogWarning("VerticalBackgroundScroll: 최소 2개의 배경이 필요합니다!");
            return;
        }
        
        // 첫 번째 배경의 높이 계산
        CalculateBackgroundHeight();
        
        // 배경들을 세로로 배치
        ArrangeBackgrounds();
    }

    void CalculateBackgroundHeight()
    {
        if (backgrounds[0].GetComponent<SpriteRenderer>() != null)
        {
            SpriteRenderer sr = backgrounds[0].GetComponent<SpriteRenderer>();
            backgroundHeight = sr.bounds.size.y;
        }
        else if (backgrounds[0].GetComponent<RectTransform>() != null)
        {
            RectTransform rt = backgrounds[0].GetComponent<RectTransform>();
            backgroundHeight = rt.rect.height * rt.lossyScale.y;
        }
        else
        {
            backgroundHeight = 10f;
        }
        
        Debug.Log($"VerticalBackgroundScroll: 배경 높이 = {backgroundHeight}");
    }

    void ArrangeBackgrounds()
    {
        // 첫 번째 배경의 Y 위치
        float yPos = backgrounds[0].position.y;
        
        // 나머지 배경들을 위쪽에 연속으로 배치
        for (int i = 1; i < backgrounds.Count; i++)
        {
            Vector3 newPos = backgrounds[i].position;
            newPos.y = yPos + (backgroundHeight * i);
            backgrounds[i].position = newPos;
            Debug.Log($"배경 {i} 배치: y = {newPos.y}");
        }
    }

    void Update()
    {
        if (backgrounds.Count < 2) return;
        
        // 모든 배경을 아래로 이동 (플레이어가 위로 가는 느낌)
        foreach (Transform bg in backgrounds)
        {
            bg.position += Vector3.down * scrollSpeed * Time.deltaTime;
        }
        
        // 가장 아래쪽 배경이 화면 밖으로 나갔는지 확인
        Transform bottommostBg = GetBottommostBackground();
        float cameraBottomEdge = mainCamera.transform.position.y - mainCamera.orthographicSize - backgroundHeight;
        
        if (bottommostBg.position.y < cameraBottomEdge)
        {
            // 가장 위쪽 배경 찾기
            Transform topmostBg = GetTopmostBackground();
            
            // 아래쪽 배경을 위쪽 끝으로 이동
            Vector3 newPos = bottommostBg.position;
            newPos.y = topmostBg.position.y + backgroundHeight;
            bottommostBg.position = newPos;
            
            Debug.Log($"배경 재배치: {bottommostBg.name}을(를) y={newPos.y}로 이동");
        }
    }

    Transform GetBottommostBackground()
    {
        Transform bottommost = backgrounds[0];
        foreach (Transform bg in backgrounds)
        {
            if (bg.position.y < bottommost.position.y)
                bottommost = bg;
        }
        return bottommost;
    }

    Transform GetTopmostBackground()
    {
        Transform topmost = backgrounds[0];
        foreach (Transform bg in backgrounds)
        {
            if (bg.position.y > topmost.position.y)
                topmost = bg;
        }
        return topmost;
    }
    
    // Inspector에서 속도 조정
    void OnValidate()
    {
        if (scrollSpeed < 0)
            scrollSpeed = 0;
    }
}