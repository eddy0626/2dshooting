using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ParticleConnectorHelper : MonoBehaviour
{
#if UNITY_EDITOR
    [ContextMenu("파티클 자동 연결 실행")]
    public void ConnectAllParticles()
    {
        Debug.Log("=== 파티클 자동 연결 시작 ===");
        
        // 파티클 로드
        GameObject explosion = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/03.Prefabs/Effects/ExplosionEffect.prefab");
        GameObject hit = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/03.Prefabs/Effects/HitEffect.prefab");
        GameObject collect = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/03.Prefabs/Effects/CollectEffect.prefab");
        
        if (!explosion || !hit || !collect)
        {
            Debug.LogError("❌ 파티클 프리팹을 찾을 수 없습니다!");
            return;
        }
        
        Debug.Log("✅ 파티클 프리팹 로드 완료");
        int count = 0;
        
        // Enemy 연결
        string[] enemies = { "Enemy", "EnemyB", "EnemyC" };
        foreach (string name in enemies)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/03.Prefabs/Enemy/{name}.prefab");
            if (prefab == null) continue;
            
            Enemy enemy = prefab.GetComponent<Enemy>();
            if (enemy == null) continue;
            
            SerializedObject so = new SerializedObject(enemy);
            so.FindProperty("explosionEffect").objectReferenceValue = explosion;
            so.FindProperty("hitEffect").objectReferenceValue = hit;
            so.ApplyModifiedProperties();
            
            EditorUtility.SetDirty(prefab);
            count++;
            Debug.Log($"  ✅ {name} 연결 완료");
        }
        
        // Item 연결
        string[] items = { "Health", "Speed", "AttackSpeed" };
        foreach (string name in items)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/03.Prefabs/Item/{name}.prefab");
            if (prefab == null) continue;
            
            Item item = prefab.GetComponent<Item>();
            if (item == null) continue;
            
            SerializedObject so = new SerializedObject(item);
            so.FindProperty("_pickupParticlePrefab").objectReferenceValue = collect;
            so.ApplyModifiedProperties();
            
            EditorUtility.SetDirty(prefab);
            count++;
            Debug.Log($"  ✅ {name} 연결 완료");
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"\n🎉 파티클 연결 완료! 총 {count}개 프리팹");
        EditorUtility.DisplayDialog("완료!", 
            $"파티클이 자동으로 연결되었습니다!\n\n총 {count}개 프리팹\n\n이제 플레이하면 파티클이 나옵니다!", "확인");
    }
#endif
}
