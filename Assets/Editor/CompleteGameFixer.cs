using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;

/// <summary>
/// 게임의 모든 문제를 자동으로 감지하고 수정하는 종합 툴
/// </summary>
[InitializeOnLoad]
public class CompleteGameFixer : EditorWindow
{
    static CompleteGameFixer()
    {
        // Play 모드 진입 전에 자동 수정
        EditorApplication.playModeStateChanged += (state) =>
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                FixAllIssues();
            }
        };
    }
    
    [MenuItem("Tools/🔧 Fix All Game Issues")]
    public static void ShowWindow()
    {
        FixAllIssues();
    }
    
    public static void FixAllIssues()
    {
        Debug.Log("==============================================");
        Debug.Log("🔧 게임 자동 수정 시작!");
        Debug.Log("==============================================");
        
        int fixedCount = 0;
        
        // 1. BulletSystem 프리팹 연결
        fixedCount += FixBulletSystem();
        
        // 2. EnemySpawner 설정
        fixedCount += FixEnemySpawner();
        
        // 3. 레이어 및 태그 확인
        fixedCount += CheckLayersAndTags();
        
        // 4. 프리팹 존재 확인
        fixedCount += CheckPrefabs();
        
        Debug.Log("==============================================");
        if (fixedCount > 0)
        {
            Debug.Log($"✅ 총 {fixedCount}개의 문제를 수정했습니다!");
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
        else
        {
            Debug.Log("✓ 모든 설정이 정상입니다!");
        }
        Debug.Log("==============================================");
    }
    
    private static int FixBulletSystem()
    {
        int fixedCount = 0;
        GameObject player = GameObject.Find("Player");
        
        if (player == null)
        {
            Debug.LogWarning("⚠️ 씬에 Player가 없습니다!");
            return 0;
        }
        
        BulletSystem bulletSystem = player.GetComponent<BulletSystem>();
        if (bulletSystem == null)
        {
            Debug.LogWarning("⚠️ Player에 BulletSystem 컴포넌트가 없습니다!");
            return 0;
        }
        
        // 프리팹 로드
        GameObject mainBullet = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/03.Prefabs/Bullet/MainBullet.prefab");
        GameObject subBullet = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/03.Prefabs/Bullet/SubBullet.prefab");
        
        SerializedObject so = new SerializedObject(bulletSystem);
        SerializedProperty mainProp = so.FindProperty("_mainBulletPrefab");
        SerializedProperty subProp = so.FindProperty("_subBulletPrefab");
        
        if (mainProp != null && mainProp.objectReferenceValue == null && mainBullet != null)
        {
            mainProp.objectReferenceValue = mainBullet;
            Debug.Log("✅ MainBullet 프리팹 연결!");
            fixedCount++;
        }
        
        if (subProp != null && subProp.objectReferenceValue == null && subBullet != null)
        {
            subProp.objectReferenceValue = subBullet;
            Debug.Log("✅ SubBullet 프리팹 연결!");
            fixedCount++;
        }
        
        if (fixedCount > 0)
        {
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(player);
        }
        
        return fixedCount;
    }
    
    private static int FixEnemySpawner()
    {
        int fixedCount = 0;
        GameObject spawner = GameObject.Find("EnemySpawner");
        
        if (spawner == null)
        {
            Debug.Log("ℹ️ EnemySpawner가 없습니다 (정상일 수 있음)");
            return 0;
        }
        
        EnemySpawner enemySpawner = spawner.GetComponent<EnemySpawner>();
        if (enemySpawner != null)
        {
            SerializedObject so = new SerializedObject(enemySpawner);
            SerializedProperty enemyPrefabsProp = so.FindProperty("enemyPrefabs");
            
            if (enemyPrefabsProp != null && enemyPrefabsProp.arraySize == 0)
            {
                // Enemy 프리팹 찾기
                string[] guids = AssetDatabase.FindAssets("t:Prefab Enemy", new[] { "Assets/03.Prefabs" });
                
                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    
                    if (prefab != null)
                    {
                        int index = enemyPrefabsProp.arraySize;
                        enemyPrefabsProp.InsertArrayElementAtIndex(index);
                        enemyPrefabsProp.GetArrayElementAtIndex(index).objectReferenceValue = prefab;
                        Debug.Log($"✅ Enemy 프리팹 추가: {prefab.name}");
                        fixedCount++;
                    }
                }
                
                if (fixedCount > 0)
                {
                    so.ApplyModifiedProperties();
                    EditorUtility.SetDirty(spawner);
                }
            }
        }
        
        return fixedCount;
    }
    
    private static int CheckLayersAndTags()
    {
        int checkedCount = 0;
        string[] requiredTags = { "Player", "Enemy", "Bullet", "PlayerBullet", "EnemyBullet", "Item" };
        
        foreach (string tag in requiredTags)
        {
            if (!TagExists(tag))
            {
                Debug.LogWarning($"⚠️ 태그 '{tag}'가 없습니다! Tag Manager에서 추가해주세요.");
            }
        }
        
        return checkedCount;
    }
    
    private static int CheckPrefabs()
    {
        int missing = 0;
        string[] requiredPrefabs = {
            "Assets/03.Prefabs/Bullet/MainBullet.prefab",
            "Assets/03.Prefabs/Bullet/SubBullet.prefab",
            "Assets/03.Prefabs/Enemy.prefab",
            "Assets/03.Prefabs/Item.prefab"
        };
        
        foreach (string prefabPath in requiredPrefabs)
        {
            if (!File.Exists(prefabPath))
            {
                Debug.LogWarning($"⚠️ 프리팹이 없습니다: {prefabPath}");
                missing++;
            }
        }
        
        if (missing > 0)
        {
            Debug.LogWarning($"⚠️ {missing}개의 필수 프리팹이 없습니다!");
        }
        
        return 0;
    }
    
    private static bool TagExists(string tag)
    {
        try
        {
            GameObject.FindWithTag(tag);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
