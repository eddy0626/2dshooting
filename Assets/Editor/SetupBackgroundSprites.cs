using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// 배경 스프라이트 설정
/// </summary>
public class SetupBackgroundSprites : EditorWindow
{
    [MenuItem("Tools/Setup Background 🌌")]
    public static void ShowWindow()
    {
        GetWindow<SetupBackgroundSprites>("Setup Background");
    }

    void OnGUI()
    {
        GUILayout.Label("배경 스프라이트 설정", EditorStyles.boldLabel);
        
        EditorGUILayout.Space(10);
        
        if (GUILayout.Button("🌌 배경 스프라이트 설정!", GUILayout.Height(50)))
        {
            SetupBackground();
        }
        
        EditorGUILayout.Space(10);
        
        EditorGUILayout.HelpBox("현재 씬의 Background 오브젝트에\nVertical 2D Shooting BE4 배경 스프라이트를 설정합니다.", MessageType.Info);
    }

    private void SetupBackground()
    {
        Debug.Log("=== 🌌 배경 설정 시작 ===");
        
        string spritePath = "Assets/08.Assets/Vertical 2D Shooting BE4/Sprites/Backgrounds.png";
        Sprite[] sprites = LoadSprites(spritePath);
        
        if (sprites.Length == 0)
        {
            Debug.LogError("❌ Backgrounds.png 스프라이트를 찾을 수 없습니다!");
            EditorUtility.DisplayDialog("오류", "Backgrounds.png를 먼저 Multiple 모드로 설정해주세요!", "확인");
            return;
        }
        
        // Canvas 찾기
        Canvas canvas = GameObject.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("❌ Canvas를 찾을 수 없습니다!");
            return;
        }
        
        // Background 오브젝트 찾기 또는 생성
        Transform bgTransform = canvas.transform.Find("Background");
        GameObject background;
        
        if (bgTransform == null)
        {
            background = new GameObject("Background");
            background.transform.SetParent(canvas.transform);
            background.transform.SetAsFirstSibling(); // 맨 뒤로
            
            RectTransform rt = background.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 1);
            rt.sizeDelta = Vector2.zero;
            rt.anchoredPosition = Vector2.zero;
            
            Debug.Log("✅ Background 오브젝트 생성!");
        }
        else
        {
            background = bgTransform.gameObject;
        }
        
        // Image 컴포넌트 추가 또는 가져오기
        Image bgImage = background.GetComponent<Image>();
        if (bgImage == null)
        {
            bgImage = background.AddComponent<Image>();
        }
        
        // 배경 스프라이트 설정 (파란색 배경)
        if (sprites.Length > 0)
        {
            bgImage.sprite = sprites[0]; // 첫 번째 배경 사용
            bgImage.type = Image.Type.Tiled;
            Debug.Log($"✅ 배경 스프라이트 설정 완료! (스프라이트 개수: {sprites.Length})");
        }
        
        Debug.Log("=== ✅ 배경 설정 완료! ===");
        EditorUtility.DisplayDialog("완료!", "배경이 설정되었습니다!", "확인");
    }

    private Sprite[] LoadSprites(string path)
    {
        Object[] objects = AssetDatabase.LoadAllAssetsAtPath(path);
        return objects.Where(o => o is Sprite).Cast<Sprite>().ToArray();
    }
}
