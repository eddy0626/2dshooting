using UnityEngine;
using UnityEditor;

public class ParticleAutoConnect
{
    [InitializeOnLoadMethod]
    private static void AutoConnect()
    {
        // 이미 실행되었으면 건너뛰기
        string key = "ParticleAutoConnected_v1";
        if (SessionState.GetBool(key, false))
            return;
        
        // 프로젝트 로드 후 약간의 딜레이
        EditorApplication.delayCall += () =>
        {
            SessionState.SetBool(key, true);
            DoConnect();
        };
    }
    
    [MenuItem("Tools/파티클 다시 연결하기")]
    private static void ReConnect()
    {
        SessionState.SetBool("ParticleAutoConnected_v1", false);
        DoConnect();
    }
    
    private static void DoConnect()
    {
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log("🎨 파티클 자동 연결 시작!");
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        
        // 파티클 프리팹 로드
        GameObject explosion = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/03.Prefabs/Effects/ExplosionEffect.prefab");
        GameObject hit = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/03.Prefabs/Effects/HitEffect.prefab");
        GameObject collect = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/03.Prefabs/Effects/CollectEffect.prefab");
        
        if (explosion == null || hit == null || collect == null)
        {
            Debug.LogError("❌ 파티클 프리팹을 찾을 수 없습니다!");
            Debug.LogError("   확인: Assets/03.Prefabs/Effects/");
            return;
        }
        
        Debug.Log($"✅ ExplosionEffect: {explosion.name}");
        Debug.Log($"✅ HitEffect: {hit.name}");
        Debug.Log($"✅ CollectEffect: {collect.name}");
        Debug.Log("");
        
        int successCount = 0;
        
        // Enemy 프리팹들 연결
        Debug.Log("🔧 Enemy 프리팹 연결 중...");
        string[] enemyNames = { "Enemy", "EnemyB", "EnemyC" };
        foreach (string name in enemyNames)
        {
            string path = $"Assets/03.Prefabs/Enemy/{name}.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            if (prefab == null)
            {
                Debug.LogWarning($"  ⚠️  {name}.prefab를 찾을 수 없습니다.");
                continue;
            }
            
            Enemy enemy = prefab.GetComponent<Enemy>();
            if (enemy == null)
            {
                Debug.LogWarning($"  ⚠️  {name}에 Enemy 컴포넌트가 없습니다.");
                continue;
            }
            
            SerializedObject so = new SerializedObject(enemy);
            SerializedProperty explosionProp = so.FindProperty("explosionEffect");
            SerializedProperty hitProp = so.FindProperty("hitEffect");
            
            if (explosionProp != null && hitProp != null)
            {
                explosionProp.objectReferenceValue = explosion;
                hitProp.objectReferenceValue = hit;
                so.ApplyModifiedProperties();
                
                EditorUtility.SetDirty(prefab);
                PrefabUtility.SavePrefabAsset(prefab);
                
                successCount++;
                Debug.Log($"  ✅ {name} → 폭발/히트 파티클 연결");
            }
        }
        
        Debug.Log("");
        
        // Item 프리팹들 연결
        Debug.Log("🔧 Item 프리팹 연결 중...");
        string[] itemNames = { "Health", "Speed", "AttackSpeed" };
        foreach (string name in itemNames)
        {
            string path = $"Assets/03.Prefabs/Item/{name}.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            if (prefab == null)
            {
                Debug.LogWarning($"  ⚠️  {name}.prefab를 찾을 수 없습니다.");
                continue;
            }
            
            Item item = prefab.GetComponent<Item>();
            if (item == null)
            {
                Debug.LogWarning($"  ⚠️  {name}에 Item 컴포넌트가 없습니다.");
                continue;
            }
            
            SerializedObject so = new SerializedObject(item);
            SerializedProperty pickupProp = so.FindProperty("_pickupParticlePrefab");
            
            if (pickupProp != null)
            {
                pickupProp.objectReferenceValue = collect;
                so.ApplyModifiedProperties();
                
                EditorUtility.SetDirty(prefab);
                PrefabUtility.SavePrefabAsset(prefab);
                
                successCount++;
                Debug.Log($"  ✅ {name} → 수집 파티클 연결");
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("");
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log($"🎉 파티클 연결 완료! (총 {successCount}개)");
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log("");
        Debug.Log("💡 이제 플레이하면 파티클 효과가 나옵니다!");
        Debug.Log("   - 적 공격 시: HitEffect");
        Debug.Log("   - 적 죽을 때: ExplosionEffect");  
        Debug.Log("   - 아이템 먹을 때: CollectEffect");
        Debug.Log("");
    }
}
