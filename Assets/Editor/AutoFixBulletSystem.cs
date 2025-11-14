using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// <summary>
/// 에디터가 시작될 때와 씬이 열릴 때 자동으로 Player의 BulletSystem 프리팹을 연결
/// </summary>
[InitializeOnLoad]
public class AutoFixBulletSystem
{
    static AutoFixBulletSystem()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        EditorSceneManager.sceneOpened += OnSceneOpened;
    }
    
    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        // Play 모드 진입 전에 자동 수정
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            FixBulletSystem();
        }
    }
    
    private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
    {
        // 씬이 열릴 때 자동 수정
        EditorApplication.delayCall += FixBulletSystem;
    }
    
    [MenuItem("Tools/Auto Fix All Issues")]
    public static void FixBulletSystem()
    {
        Debug.Log("=== 🔧 자동 수정 시작 ===");
        
        // 씬에서 Player 찾기
        GameObject player = GameObject.Find("Player");
        
        if (player == null)
        {
            Debug.Log("씬에 Player가 없습니다. 건너뜁니다.");
            return;
        }
        
        // BulletSystem 컴포넌트 확인
        BulletSystem bulletSystem = player.GetComponent<BulletSystem>();
        
        if (bulletSystem == null)
        {
            Debug.Log("Player에 BulletSystem이 없습니다. 건너뜁니다.");
            return;
        }
        
        // 프리팹 로드
        GameObject mainBulletPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/03.Prefabs/Bullet/MainBullet.prefab");
        GameObject subBulletPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/03.Prefabs/Bullet/SubBullet.prefab");
        
        if (mainBulletPrefab == null)
        {
            Debug.LogWarning("MainBullet.prefab을 찾을 수 없습니다!");
            return;
        }
        
        if (subBulletPrefab == null)
        {
            Debug.LogWarning("SubBullet.prefab을 찾을 수 없습니다!");
            return;
        }
        
        // SerializedObject를 사용하여 private 필드 접근
        SerializedObject so = new SerializedObject(bulletSystem);
        
        SerializedProperty mainProp = so.FindProperty("_mainBulletPrefab");
        SerializedProperty subProp = so.FindProperty("_subBulletPrefab");
        
        bool needsFix = false;
        
        if (mainProp.objectReferenceValue == null)
        {
            mainProp.objectReferenceValue = mainBulletPrefab;
            needsFix = true;
            Debug.Log("✅ MainBullet 프리팹 연결!");
        }
        
        if (subProp.objectReferenceValue == null)
        {
            subProp.objectReferenceValue = subBulletPrefab;
            needsFix = true;
            Debug.Log("✅ SubBullet 프리팹 연결!");
        }
        
        if (needsFix)
        {
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(player);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Debug.Log("=== ✅ BulletSystem 자동 수정 완료! ===");
        }
        else
        {
            Debug.Log("=== ✓ BulletSystem은 이미 올바르게 설정되어 있습니다! ===");
        }
    }
}
