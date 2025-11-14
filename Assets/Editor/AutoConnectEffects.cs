using UnityEngine;
using UnityEditor;

/// <summary>
/// 게임 스크립트에 이펙트를 자동으로 연결
/// </summary>
public class AutoConnectEffects : EditorWindow
{
    [MenuItem("Tools/Auto Connect Effects ✨")]
    public static void ShowWindow()
    {
        GetWindow<AutoConnectEffects>("Connect Effects");
    }

    void OnGUI()
    {
        GUILayout.Label("이펙트 자동 연결", EditorStyles.boldLabel);
        
        EditorGUILayout.Space(10);
        
        if (GUILayout.Button("✨ 모든 이펙트 자동 연결!", GUILayout.Height(50)))
        {
            ConnectAllEffects();
        }
        
        EditorGUILayout.Space(10);
        
        EditorGUILayout.HelpBox("프리팹들에 이펙트를 자동으로 연결합니다:\n" +
            "• Enemy → 히트/폭발 이펙트\n" +
            "• Bullet → 폭발 이펙트\n" +
            "• Item → 수집 이펙트", MessageType.Info);
    }

    private void ConnectAllEffects()
    {
        Debug.Log("=== ✨ 이펙트 자동 연결 시작 ===");
        
        // 이펙트 프리팹 로드
        GameObject explosionEffect = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/03.Prefabs/ExplosionEffect.prefab");
        GameObject hitEffect = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/03.Prefabs/HitEffect.prefab");
        GameObject collectEffect = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/03.Prefabs/CollectEffect.prefab");
        
        // Enemy 프리팹에 이펙트 연결
        ConnectEnemyEffects(explosionEffect, hitEffect);
        
        // Bullet 프리팹에 이펙트 연결
        ConnectBulletEffects(explosionEffect);
        
        // Item 프리팹에 이펙트 연결
        ConnectItemEffects(collectEffect);
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("=== ✅ 이펙트 자동 연결 완료! ===");
        EditorUtility.DisplayDialog("완료!", "모든 이펙트가 연결되었습니다!", "확인");
    }

    private void ConnectEnemyEffects(GameObject explosionEffect, GameObject hitEffect)
    {
        string prefabPath = "Assets/03.Prefabs/Enemy.prefab";
        GameObject enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (enemyPrefab == null)
        {
            Debug.LogWarning("Enemy 프리팹을 찾을 수 없습니다.");
            return;
        }
        
        GameObject instance = PrefabUtility.LoadPrefabContents(prefabPath);
        Enemy enemyScript = instance.GetComponent<Enemy>();
        
        if (enemyScript != null)
        {
            SerializedObject so = new SerializedObject(enemyScript);
            
            // explosionEffect 필드 찾기 및 설정
            SerializedProperty explosionProp = so.FindProperty("explosionEffect");
            if (explosionProp != null && explosionEffect != null)
            {
                explosionProp.objectReferenceValue = explosionEffect;
            }
            
            // hitEffect 필드 찾기 및 설정
            SerializedProperty hitProp = so.FindProperty("hitEffect");
            if (hitProp != null && hitEffect != null)
            {
                hitProp.objectReferenceValue = hitEffect;
            }
            
            so.ApplyModifiedProperties();
            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            Debug.Log("✅ Enemy 이펙트 연결 완료!");
        }
        
        PrefabUtility.UnloadPrefabContents(instance);
    }

    private void ConnectBulletEffects(GameObject explosionEffect)
    {
        string prefabPath = "Assets/03.Prefabs/Bullet.prefab";
        GameObject bulletPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (bulletPrefab == null)
        {
            Debug.LogWarning("Bullet 프리팹을 찾을 수 없습니다.");
            return;
        }
        
        GameObject instance = PrefabUtility.LoadPrefabContents(prefabPath);
        Bullet bulletScript = instance.GetComponent<Bullet>();
        
        if (bulletScript != null)
        {
            SerializedObject so = new SerializedObject(bulletScript);
            
            SerializedProperty explosionProp = so.FindProperty("explosionEffect");
            if (explosionProp != null && explosionEffect != null)
            {
                explosionProp.objectReferenceValue = explosionEffect;
            }
            
            so.ApplyModifiedProperties();
            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            Debug.Log("✅ Bullet 이펙트 연결 완료!");
        }
        
        PrefabUtility.UnloadPrefabContents(instance);
    }

    private void ConnectItemEffects(GameObject collectEffect)
    {
        string prefabPath = "Assets/03.Prefabs/Item.prefab";
        GameObject itemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (itemPrefab == null)
        {
            Debug.LogWarning("Item 프리팹을 찾을 수 없습니다.");
            return;
        }
        
        GameObject instance = PrefabUtility.LoadPrefabContents(prefabPath);
        Item itemScript = instance.GetComponent<Item>();
        
        if (itemScript != null)
        {
            SerializedObject so = new SerializedObject(itemScript);
            
            SerializedProperty collectProp = so.FindProperty("collectEffect");
            if (collectProp != null && collectEffect != null)
            {
                collectProp.objectReferenceValue = collectEffect;
            }
            
            so.ApplyModifiedProperties();
            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            Debug.Log("✅ Item 이펙트 연결 완료!");
        }
        
        PrefabUtility.UnloadPrefabContents(instance);
    }
}
