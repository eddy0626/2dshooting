using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

/// <summary>
/// Player의 BulletSystem에 총알 프리팹을 자동으로 연결
/// </summary>
public class FixBulletSystemPrefabs : EditorWindow
{
    [MenuItem("Tools/Fix Bullet System Prefabs")]
    public static void ShowWindow()
    {
        FixPrefabs();
    }

    public static void FixPrefabs()
    {
        Debug.Log("=== 🔫 BulletSystem 프리팹 연결 시작 ===");
        
        // 씬에서 Player 찾기
        GameObject player = GameObject.Find("Player");
        
        if (player == null)
        {
            Debug.LogError("❌ 씬에서 Player를 찾을 수 없습니다!");
            EditorUtility.DisplayDialog("오류", "씬에서 Player를 찾을 수 없습니다!", "확인");
            return;
        }
        
        // BulletSystem 컴포넌트 가져오기
        BulletSystem bulletSystem = player.GetComponent<BulletSystem>();
        
        if (bulletSystem == null)
        {
            Debug.LogError("❌ Player에 BulletSystem 컴포넌트가 없습니다!");
            EditorUtility.DisplayDialog("오류", "Player에 BulletSystem이 없습니다!", "확인");
            return;
        }
        
        // 프리팹 로드
        GameObject mainBulletPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/03.Prefabs/Bullet/MainBullet.prefab");
        GameObject subBulletPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/03.Prefabs/Bullet/SubBullet.prefab");
        
        if (mainBulletPrefab == null)
        {
            Debug.LogError("❌ MainBullet.prefab을 찾을 수 없습니다!");
            return;
        }
        
        if (subBulletPrefab == null)
        {
            Debug.LogError("❌ SubBullet.prefab을 찾을 수 없습니다!");
            return;
        }
        
        // SerializedObject를 사용하여 private 필드에 접근
        SerializedObject so = new SerializedObject(bulletSystem);
        
        so.FindProperty("_mainBulletPrefab").objectReferenceValue = mainBulletPrefab;
        so.FindProperty("_subBulletPrefab").objectReferenceValue = subBulletPrefab;
        
        so.ApplyModifiedProperties();
        
        // 씬 저장
        EditorUtility.SetDirty(player);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        
        Debug.Log("✅ MainBullet 프리팹 연결 완료!");
        Debug.Log("✅ SubBullet 프리팹 연결 완료!");
        Debug.Log("=== ✅ BulletSystem 프리팹 연결 완료! ===");
        
        EditorUtility.DisplayDialog("완료!", 
            "총알 프리팹이 Player의 BulletSystem에 연결되었습니다!\n\n" +
            "이제 경고 없이 총알을 발사할 수 있습니다!", "확인");
    }
}
