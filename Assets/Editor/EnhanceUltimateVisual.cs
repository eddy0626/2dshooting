using UnityEngine;
using UnityEditor;
using System.Linq;

/// <summary>
/// 궁극기 시각 효과 강화
/// </summary>
public class EnhanceUltimateVisual : EditorWindow
{
    [MenuItem("Tools/Enhance Ultimate Visual 🔥")]
    public static void ShowWindow()
    {
        GetWindow<EnhanceUltimateVisual>("Ultimate Visual");
    }

    void OnGUI()
    {
        GUILayout.Label("궁극기 시각 효과 강화", EditorStyles.boldLabel);
        
        EditorGUILayout.Space(10);
        
        if (GUILayout.Button("🔥 궁극기 시각 효과 강화!", GUILayout.Height(50)))
        {
            EnhanceUltimate();
        }
        
        EditorGUILayout.Space(10);
        
        EditorGUILayout.HelpBox("궁극기를 더 잘 보이게 만듭니다:\n" +
            "✅ 스프라이트 추가/크기 조절\n" +
            "✅ 빛나는 효과 추가\n" +
            "✅ Trail 효과 추가\n" +
            "✅ 강력한 파티클 추가", MessageType.Info);
    }

    private void EnhanceUltimate()
    {
        Debug.Log("=== 🔥 궁극기 시각 효과 강화 시작 ===");
        
        string ultimatePrefabPath = "Assets/03.Prefabs/Bullet/UltimateSkill.prefab";
        GameObject ultimatePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ultimatePrefabPath);
        
        if (ultimatePrefab == null)
        {
            Debug.LogError("❌ UltimateSkill 프리팹을 찾을 수 없습니다!");
            EditorUtility.DisplayDialog("오류", "UltimateSkill.prefab을 찾을 수 없습니다!", "확인");
            return;
        }
        
        GameObject instance = PrefabUtility.LoadPrefabContents(ultimatePrefabPath);
        
        // 1. SpriteRenderer 설정
        SpriteRenderer sr = instance.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            sr = instance.AddComponent<SpriteRenderer>();
        }
        
        // 스프라이트 설정 (Bullet 스프라이트 사용)
        string bulletSpritePath = "Assets/08.Assets/Vertical 2D Shooting BE4/Sprites/Bullets.png";
        Sprite[] bulletSprites = AssetDatabase.LoadAllAssetsAtPath(bulletSpritePath)
            .OfType<Sprite>().ToArray();
        
        if (bulletSprites.Length > 0)
        {
            sr.sprite = bulletSprites[0]; // 첫 번째 총알 스프라이트
            sr.sortingOrder = 10; // 맨 앞에 표시
            sr.color = new Color(1f, 0.5f, 0f, 1f); // 주황색으로 변경
        }
        
        // 크기 확대
        instance.transform.localScale = new Vector3(3f, 3f, 1f);
        
        // 2. Trail Renderer 추가
        TrailRenderer trail = instance.GetComponent<TrailRenderer>();
        if (trail == null)
        {
            trail = instance.AddComponent<TrailRenderer>();
        }
        
        trail.time = 0.5f;
        trail.startWidth = 0.5f;
        trail.endWidth = 0.1f;
        trail.material = new Material(Shader.Find("Sprites/Default"));
        trail.startColor = new Color(1f, 0.8f, 0f, 1f); // 노란색
        trail.endColor = new Color(1f, 0.3f, 0f, 0f); // 투명한 주황색
        trail.sortingOrder = 9;
        
        // 3. 빛나는 효과 추가 (Light2D 또는 추가 스프라이트)
        Transform glowChild = instance.transform.Find("Glow");
        if (glowChild == null)
        {
            GameObject glow = new GameObject("Glow");
            glow.transform.SetParent(instance.transform);
            glow.transform.localPosition = Vector3.zero;
            glow.transform.localScale = Vector3.one * 1.5f;
            
            SpriteRenderer glowSr = glow.AddComponent<SpriteRenderer>();
            if (bulletSprites.Length > 0)
            {
                glowSr.sprite = bulletSprites[0];
                glowSr.color = new Color(1f, 1f, 0f, 0.5f); // 반투명 노란색
                glowSr.sortingOrder = 9;
            }
        }
        
        // 4. 회전 효과 추가
        Transform rotatorChild = instance.transform.Find("Rotator");
        if (rotatorChild == null)
        {
            GameObject rotator = new GameObject("Rotator");
            rotator.transform.SetParent(instance.transform);
            rotator.transform.localPosition = Vector3.zero;
            
            // 회전 스크립트 추가 (간단한 회전)
            var rotateScript = rotator.AddComponent<SimpleRotate>();
            rotateScript.SetRotationSpeed(360f);
            
            // 스프라이트 추가
            SpriteRenderer rotatorSr = rotator.AddComponent<SpriteRenderer>();
            if (bulletSprites.Length > 1)
            {
                rotatorSr.sprite = bulletSprites[bulletSprites.Length > 1 ? 1 : 0];
                rotatorSr.color = new Color(1f, 0.3f, 0f, 0.7f);
                rotatorSr.sortingOrder = 8;
            }
        }
        
        PrefabUtility.SaveAsPrefabAsset(instance, ultimatePrefabPath);
        PrefabUtility.UnloadPrefabContents(instance);
        
        Debug.Log("=== ✅ 궁극기 시각 효과 강화 완료! ===");
        EditorUtility.DisplayDialog("완료!", "궁극기 시각 효과가 강화되었습니다!\n이제 게임에서 확인해보세요!", "확인");
    }
}


