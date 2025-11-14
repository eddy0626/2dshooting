using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class ParticleAutoConnector
{
    static ParticleAutoConnector()
    {
        EditorApplication.delayCall += ConnectParticles;
    }
    
    private static void ConnectParticles()
    {
        // 한 번만 실행되도록
        EditorApplication.delayCall -= ConnectParticles;
        
        // 이미 연결되었는지 확인
        if (EditorPrefs.GetBool("ParticlesConnected", false))
            return;
        
        Debug.Log("=== 파티클 자동 연결 시작 ===");
        
        var explosion = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/03.Prefabs/Effects/ExplosionEffect.prefab");
        var hit = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/03.Prefabs/Effects/HitEffect.prefab");
        var collect = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/03.Prefabs/Effects/CollectEffect.prefab");
        
        if (!explosion || !hit || !collect)
        {
            Debug.LogWarning("파티클 프리팹을 찾을 수 없습니다.");
            return;
        }
        
        int total = 0;
        
        // Enemy
        foreach (var name in new[] { "Enemy", "EnemyB", "EnemyC" })
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/03.Prefabs/Enemy/{name}.prefab");
            if (!prefab) continue;
            
            var enemy = prefab.GetComponent<Enemy>();
            if (!enemy) continue;
            
            var so = new SerializedObject(enemy);
            so.FindProperty("explosionEffect").objectReferenceValue = explosion;
            so.FindProperty("hitEffect").objectReferenceValue = hit;
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(prefab);
            total++;
        }
        
        // Item
        foreach (var name in new[] { "Health", "Speed", "AttackSpeed" })
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/03.Prefabs/Item/{name}.prefab");
            if (!prefab) continue;
            
            var item = prefab.GetComponent<Item>();
            if (!item) continue;
            
            var so = new SerializedObject(item);
            so.FindProperty("_pickupParticlePrefab").objectReferenceValue = collect;
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(prefab);
            total++;
        }
        
        AssetDatabase.SaveAssets();
        
        if (total > 0)
        {
            Debug.Log($"✓ 파티클 연결 완료! ({total}개 프리팹)");
            EditorPrefs.SetBool("ParticlesConnected", true);
        }
    }
    
    [MenuItem("Tools/Reset Particle Connection")]
    private static void Reset()
    {
        EditorPrefs.DeleteKey("ParticlesConnected");
        Debug.Log("파티클 연결 상태 초기화");
    }
}
