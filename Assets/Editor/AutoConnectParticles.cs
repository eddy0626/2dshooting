using UnityEngine;
using UnityEditor;

public class AutoConnectParticles
{
    [MenuItem("Tools/Particle Effects/자동 연결 실행")]
    public static void Execute()
    {
        Debug.Log("=== 파티클 효과 자동 연결 시작 ===");
        
        // 파티클 프리팹 로드
        var explosion = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/03.Prefabs/Effects/ExplosionEffect.prefab");
        var hit = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/03.Prefabs/Effects/HitEffect.prefab");
        var collect = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/03.Prefabs/Effects/CollectEffect.prefab");
        
        if (!explosion || !hit || !collect)
        {
            Debug.LogError("파티클 프리팹을 찾을 수 없습니다!");
            return;
        }
        
        Debug.Log("✓ 파티클 프리팹 로드 완료");
        
        // Enemy 프리팹들 처리
        string[] enemies = { "Enemy", "EnemyB", "EnemyC" };
        int enemyCount = 0;
        
        foreach (var name in enemies)
        {
            var path = $"Assets/03.Prefabs/Enemy/{name}.prefab";
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (!prefab) continue;
            
            var enemy = prefab.GetComponent<Enemy>();
            if (!enemy) continue;
            
            var so = new SerializedObject(enemy);
            so.FindProperty("explosionEffect").objectReferenceValue = explosion;
            so.FindProperty("hitEffect").objectReferenceValue = hit;
            so.ApplyModifiedProperties();
            
            EditorUtility.SetDirty(prefab);
            enemyCount++;
            Debug.Log($"✓ {name}에 파티클 연결");
        }
        
        // Item 프리팹들 처리
        string[] items = { "Health", "Speed", "AttackSpeed" };
        int itemCount = 0;
        
        foreach (var name in items)
        {
            var path = $"Assets/03.Prefabs/Item/{name}.prefab";
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (!prefab) continue;
            
            var item = prefab.GetComponent<Item>();
            if (!item) continue;
            
            var so = new SerializedObject(item);
            so.FindProperty("_pickupParticlePrefab").objectReferenceValue = collect;
            so.ApplyModifiedProperties();
            
            EditorUtility.SetDirty(prefab);
            itemCount++;
            Debug.Log($"✓ {name}에 파티클 연결");
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"\n=== 완료! Enemy {enemyCount}개, Item {itemCount}개 ===");
        EditorUtility.DisplayDialog("완료!", 
            $"파티클 효과 연결 완료!\n\nEnemy: {enemyCount}개\nItem: {itemCount}개\n\n플레이해보세요!", "확인");
    }
}
