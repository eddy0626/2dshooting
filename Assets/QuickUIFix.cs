using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 런타임에서도 작동하는 UI 정리 스크립트
/// </summary>
[ExecuteInEditMode]
public class QuickUIFix : MonoBehaviour
{
    [ContextMenu("Fix UI Now")]
    public void FixUIImmediate()
    {
        Debug.Log("=== Quick UI Fix Start ===");
        
        // Canvas 찾기
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas not found!");
            return;
        }
        
        // 중복 텍스트 제거
        RemoveDuplicates(canvas);
        
        // CurrentScoreText 설정
        SetupScoreText(canvas, "CurrentScoreText", new Vector2(20, -20), "Score: 0");
        
        // HighScoreText 설정  
        SetupScoreText(canvas, "HighScoreText", new Vector2(20, -60), "Best: 0");
        
        Debug.Log("=== UI Fixed! ===");
    }
    
    void RemoveDuplicates(Canvas canvas)
    {
        // ScoreText, HighScoreText는 유지
        TextMeshProUGUI[] texts = canvas.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var text in texts)
        {
            string name = text.gameObject.name;
            if (name != "CurrentScoreText" && name != "HighScoreText" && name.ToLower().Contains("score"))
            {
                DestroyImmediate(text.gameObject);
                Debug.Log($"Removed: {name}");
            }
        }
    }
    
    void SetupScoreText(Canvas canvas, string name, Vector2 pos, string defaultText)
    {
        Transform existing = canvas.transform.Find(name);
        GameObject textObj;
        
        if (existing != null)
        {
            textObj = existing.gameObject;
        }
        else
        {
            textObj = new GameObject(name);
            textObj.transform.SetParent(canvas.transform, false);
        }
        
        RectTransform rt = textObj.GetComponent<RectTransform>();
        if (rt == null) rt = textObj.AddComponent<RectTransform>();
        
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(300, 50);
        
        TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
        if (tmp == null) tmp = textObj.AddComponent<TextMeshProUGUI>();
        
        tmp.fontSize = 28;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.TopLeft;
        tmp.text = defaultText;
        tmp.fontStyle = FontStyles.Bold;
        
        // Outline
        var outline = textObj.GetComponent<Outline>();
        if (outline == null) outline = textObj.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2, -2);
        
        Debug.Log($"Setup: {name}");
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(QuickUIFix))]
public class QuickUIFixEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        QuickUIFix script = (QuickUIFix)target;
        
        if (GUILayout.Button("Fix UI Now!", GUILayout.Height(40)))
        {
            script.FixUIImmediate();
        }
    }
}
#endif
