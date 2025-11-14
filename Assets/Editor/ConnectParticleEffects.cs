using UnityEngine;
using UnityEditor;
using System.IO;

public class ConnectParticleEffects : EditorWindow
{
    [MenuItem("Tools/파티클 효과 자동 연결하기")]
    public static void ConnectAllParticleEffects()
    {
        Debug.Log("=== 파티클 효과 자동 연결 시작 ===\n");
        
        // 파티클 프리팹 로드
        GameObject explosionEffect = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/03.Prefabs/Effects/ExplosionEffect.prefab");
        GameObject hitEffect = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/03.Prefabs/Effects/HitEffect.prefab");
        GameObject collectEffect = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/03.Prefabs/Effects/CollectEffect.prefab");
        
        if (explosionEffect == null || hitEffect == null || collectEffect == null)
        {
            Debug.LogError("파티클 프리팹을 찾을 수 없습니다!");
            EditorUtility.DisplayDialog("오류", "파티클 프리팹을 찾을 수 없습니다!\nAssets/03.Prefabs/Effects/ 폴더를 확인하세요.", "확인");
            return;
        }
        
        Debug.Log("✓ 파티클 프리팹 로드 완료");
        Debug.Log($"  - ExplosionEffect: {explosionEffect.name}");
        Debug.Log($"  - HitEffect: {hitEffect.name}");
        Debug.Log($"  - CollectEffect: {collectEffect.name}\n");
        
        // Enemy 프리팹들에 연결
        int enemyCount = ConnectEnemyEffects(explosionEffect, hitEffect);
        
        // Item 프리팹들에 연결
        int itemCount = ConnectItemEffects(collectEffect);
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("\n=== 파티클 효과 자동 연결 완료! ===");
        EditorUtility.DisplayDialog("완료!", 
            $"파티클 효과가 자동으로 연결되었습니다!\n\n" +
            $"✓ Enemy 프리팹 {enemyCount}개\n" +
            $"✓ Item 프리팹 {itemCount}개\n\n" +
            "이제 플레이하면 파티클이 나옵니다!", "확인");
    }
    
    private static int ConnectEnemyEffects(GameObject explosion, GameObject hit)
    {
        string[] enemyPaths = {
            "Assets/03.Prefabs/Enemy/Enemy.prefab",
            "Assets/03.Prefabs/Enemy/EnemyB.prefab",
            "Assets/03.Prefabs/Enemy/EnemyC.prefab"
        };
        
        int count = 0;
        
        foreach (string path in enemyPaths)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;
            
            Enemy enemy = prefab.GetComponent<Enemy>();
            if (enemy == null) continue;
            
            SerializedObject so = new SerializedObject(enemy);
            so.FindProperty("explosionEffect").objectReferenceValue = explosion;
            so.FindProperty("hitEffect").objectReferenceValue = hit;
            so.ApplyModifiedProperties();
            
            EditorUtility.SetDirty(prefab);
            count++;
            
            Debug.Log($"✓ {prefab.name}에 파티클 연결 완료");
        }
        
        return count;
    }
    
    private static int ConnectItemEffects(GameObject collectEffect)
    {
        string[] itemPaths = {
            "Assets/03.Prefabs/Item/Health.prefab",
            "Assets/03.Prefabs/Item/Speed.prefab",
            "Assets/03.Prefabs/Item/AttackSpeed.prefab"
        };
        
        int count = 0;
        
        foreach (string path in itemPaths)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;
            
            Item item = prefab.GetComponent<Item>();
            if (item == null) continue;
            
            SerializedObject so = new SerializedObject(item);
            so.FindProperty("_pickupParticlePrefab").objectReferenceValue = collectEffect;
            so.ApplyModifiedProperties();
            
            EditorUtility.SetDirty(prefab);
            count++;
            
            Debug.Log($"✓ {prefab.name}에 파티클 연결 완료");
        }
        
        return count;
    }
}
