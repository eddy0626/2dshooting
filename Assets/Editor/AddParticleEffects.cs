using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// 게임에 파티클 효과 자동 추가
/// </summary>
public class AddParticleEffects : EditorWindow
{
    private const string EFFECTS_PATH = "Assets/08.Assets/JMO Assets/Cartoon FX Remaster/CFXR Prefabs";
    private const string PREFABS_PATH = "Assets/03.Prefabs";
    
    [MenuItem("Tools/Add Particle Effects ✨")]
    public static void ShowWindow()
    {
        GetWindow<AddParticleEffects>("Particle Effects");
    }

    void OnGUI()
    {
        GUILayout.Label("파티클 효과 추가", EditorStyles.boldLabel);
        
        EditorGUILayout.Space(10);
        
        if (GUILayout.Button("✨ 모든 파티클 효과 추가!", GUILayout.Height(50)))
        {
            AddAllParticleEffects();
        }
        
        EditorGUILayout.Space(10);
        
        if (GUILayout.Button("💥 적 죽음 이펙트만", GUILayout.Height(30)))
        {
            AddEnemyDeathEffect();
        }
        
        if (GUILayout.Button("⭐ 아이템 수집 이펙트만", GUILayout.Height(30)))
        {
            AddItemCollectEffect();
        }
        
        if (GUILayout.Button("🔥 궁극기 이펙트만", GUILayout.Height(30)))
        {
            AddUltimateEffect();
        }
        
        EditorGUILayout.Space(10);
        
        EditorGUILayout.HelpBox("파티클 효과를 자동으로 추가합니다:\n" +
            "✅ 적 죽음 - 폭발 효과\n" +
            "✅ 아이템 수집 - 반짝이는 효과\n" +
            "✅ 궁극기 - 강력한 공격 효과", MessageType.Info);
    }

    private void AddAllParticleEffects()
    {
        Debug.Log("=== ✨ 파티클 효과 추가 시작 ===");
        
        AddEnemyDeathEffect();
        AddItemCollectEffect();
        AddUltimateEffect();
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("=== ✅ 모든 파티클 효과 추가 완료! ===");
        EditorUtility.DisplayDialog("완료!", "모든 파티클 효과가 추가되었습니다!", "확인");
    }

    private void AddEnemyDeathEffect()
    {
        Debug.Log(">>> 적 죽음 이펙트 추가 시작");
        
        // 1. 폭발 이펙트 복사
        string explosionSource = EFFECTS_PATH + "/Explosions/CFXR Explosion 1.prefab";
        string explosionDest = PREFABS_PATH + "/Effects/EnemyExplosion.prefab";
        
        CreateEffectPrefab(explosionSource, explosionDest, "적 폭발 이펙트");
        
        // 2. 히트 스파크 이펙트 복사
        string hitSource = EFFECTS_PATH + "/Impacts/CFXR Hit A (Red).prefab";
        string hitDest = PREFABS_PATH + "/Effects/EnemyHit.prefab";
        
        CreateEffectPrefab(hitSource, hitDest, "적 히트 이펙트");
        
        // 3. Enemy 스크립트에 연결
        ConnectEnemyEffects();
        
        Debug.Log(">>> 적 죽음 이펙트 추가 완료!");
    }

    private void AddItemCollectEffect()
    {
        Debug.Log(">>> 아이템 수집 이펙트 추가 시작");
        
        // 1. 반짝이는 별 이펙트
        string starSource = EFFECTS_PATH + "/Magic Misc/CFXR4 Falling Stars.prefab";
        string starDest = PREFABS_PATH + "/Effects/ItemCollect.prefab";
        
        CreateEffectPrefab(starSource, starDest, "아이템 수집 이펙트");
        
        // 2. 반짝이는 빛 이펙트
        string glowSource = EFFECTS_PATH + "/Light/CFXR3 LightGlow A (Loop).prefab";
        string glowDest = PREFABS_PATH + "/Effects/ItemGlow.prefab";
        
        CreateEffectPrefab(glowSource, glowDest, "아이템 빛 이펙트");
        
        // 3. Item 스크립트에 연결
        ConnectItemEffects();
        
        Debug.Log(">>> 아이템 수집 이펙트 추가 완료!");
    }

    private void AddUltimateEffect()
    {
        Debug.Log(">>> 궁극기 이펙트 추가 시작");
        
        // 1. 강력한 폭발 이펙트
        string explosionSource = EFFECTS_PATH + "/Explosions/CFXR3 Fire Explosion B.prefab";
        string explosionDest = PREFABS_PATH + "/Effects/UltimateExplosion.prefab";
        
        CreateEffectPrefab(explosionSource, explosionDest, "궁극기 폭발");
        
        // 2. 빛나는 공격 이펙트
        string lightSource = EFFECTS_PATH + "/Light/CFXR3 Hit Light B (Air).prefab";
        string lightDest = PREFABS_PATH + "/Effects/UltimateLight.prefab";
        
        CreateEffectPrefab(lightSource, lightDest, "궁극기 빛");
        
        // 3. 불 효과
        string fireSource = EFFECTS_PATH + "/Fire/CFXR3 Hit Fire B (Air).prefab";
        string fireDest = PREFABS_PATH + "/Effects/UltimateFire.prefab";
        
        CreateEffectPrefab(fireSource, fireDest, "궁극기 불");
        
        // 4. UltimateSkill 프리팹에 연결
        ConnectUltimateEffects();
        
        Debug.Log(">>> 궁극기 이펙트 추가 완료!");
    }

    private void CreateEffectPrefab(string sourcePath, string destPath, string effectName)
    {
        // Effects 폴더 생성
        string effectsFolder = PREFABS_PATH + "/Effects";
        if (!Directory.Exists(effectsFolder))
        {
            Directory.CreateDirectory(effectsFolder);
            AssetDatabase.Refresh();
        }
        
        // 이미 존재하면 건너뛰기
        if (File.Exists(destPath))
        {
            Debug.Log($"⏭️ {effectName}은 이미 존재합니다.");
            return;
        }
        
        // 파일 복사
        if (AssetDatabase.CopyAsset(sourcePath, destPath))
        {
            Debug.Log($"✅ {effectName} 생성 완료!");
        }
        else
        {
            Debug.LogWarning($"❌ {sourcePath}를 복사할 수 없습니다.");
        }
    }

    private void ConnectEnemyEffects()
    {
        string enemyPrefabPath = PREFABS_PATH + "/Enemy.prefab";
        GameObject enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(enemyPrefabPath);
        
        if (enemyPrefab == null)
        {
            Debug.LogWarning("Enemy 프리팹을 찾을 수 없습니다.");
            return;
        }
        
        GameObject explosionEffect = AssetDatabase.LoadAssetAtPath<GameObject>(PREFABS_PATH + "/Effects/EnemyExplosion.prefab");
        GameObject hitEffect = AssetDatabase.LoadAssetAtPath<GameObject>(PREFABS_PATH + "/Effects/EnemyHit.prefab");
        
        GameObject instance = PrefabUtility.LoadPrefabContents(enemyPrefabPath);
        Enemy enemyScript = instance.GetComponent<Enemy>();
        
        if (enemyScript != null)
        {
            SerializedObject so = new SerializedObject(enemyScript);
            
            SerializedProperty explosionProp = so.FindProperty("explosionEffect");
            if (explosionProp != null && explosionEffect != null)
            {
                explosionProp.objectReferenceValue = explosionEffect;
            }
            
            SerializedProperty hitProp = so.FindProperty("hitEffect");
            if (hitProp != null && hitEffect != null)
            {
                hitProp.objectReferenceValue = hitEffect;
            }
            
            so.ApplyModifiedProperties();
            PrefabUtility.SaveAsPrefabAsset(instance, enemyPrefabPath);
            Debug.Log("✅ Enemy에 이펙트 연결 완료!");
        }
        
        PrefabUtility.UnloadPrefabContents(instance);
    }

    private void ConnectItemEffects()
    {
        string[] itemPrefabs = {
            PREFABS_PATH + "/Item_Health.prefab",
            PREFABS_PATH + "/Item_Speed.prefab",
            PREFABS_PATH + "/Item_AttackSpeed.prefab"
        };
        
        GameObject collectEffect = AssetDatabase.LoadAssetAtPath<GameObject>(PREFABS_PATH + "/Effects/ItemCollect.prefab");
        
        foreach (string itemPath in itemPrefabs)
        {
            GameObject itemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(itemPath);
            if (itemPrefab == null) continue;
            
            GameObject instance = PrefabUtility.LoadPrefabContents(itemPath);
            Item itemScript = instance.GetComponent<Item>();
            
            if (itemScript != null)
            {
                SerializedObject so = new SerializedObject(itemScript);
                
                SerializedProperty collectProp = so.FindProperty("_pickupParticlePrefab");
                if (collectProp != null && collectEffect != null)
                {
                    collectProp.objectReferenceValue = collectEffect;
                }
                
                so.ApplyModifiedProperties();
                PrefabUtility.SaveAsPrefabAsset(instance, itemPath);
            }
            
            PrefabUtility.UnloadPrefabContents(instance);
        }
        
        Debug.Log("✅ Item들에 이펙트 연결 완료!");
    }

    private void ConnectUltimateEffects()
    {
        string ultimatePrefabPath = PREFABS_PATH + "/Bullet/UltimateSkill.prefab";
        GameObject ultimatePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ultimatePrefabPath);
        
        if (ultimatePrefab == null)
        {
            Debug.LogWarning("UltimateSkill 프리팹을 찾을 수 없습니다.");
            return;
        }
        
        GameObject explosionEffect = AssetDatabase.LoadAssetAtPath<GameObject>(PREFABS_PATH + "/Effects/UltimateExplosion.prefab");
        GameObject lightEffect = AssetDatabase.LoadAssetAtPath<GameObject>(PREFABS_PATH + "/Effects/UltimateLight.prefab");
        GameObject fireEffect = AssetDatabase.LoadAssetAtPath<GameObject>(PREFABS_PATH + "/Effects/UltimateFire.prefab");
        
        GameObject instance = PrefabUtility.LoadPrefabContents(ultimatePrefabPath);
        
        // 파티클 시스템을 자식으로 추가
        if (explosionEffect != null)
        {
            GameObject explosion = PrefabUtility.InstantiatePrefab(explosionEffect) as GameObject;
            explosion.transform.SetParent(instance.transform);
            explosion.transform.localPosition = Vector3.zero;
            explosion.name = "ExplosionEffect";
        }
        
        if (lightEffect != null)
        {
            GameObject light = PrefabUtility.InstantiatePrefab(lightEffect) as GameObject;
            light.transform.SetParent(instance.transform);
            light.transform.localPosition = Vector3.zero;
            light.name = "LightEffect";
        }
        
        if (fireEffect != null)
        {
            GameObject fire = PrefabUtility.InstantiatePrefab(fireEffect) as GameObject;
            fire.transform.SetParent(instance.transform);
            fire.transform.localPosition = Vector3.zero;
            fire.name = "FireEffect";
        }
        
        PrefabUtility.SaveAsPrefabAsset(instance, ultimatePrefabPath);
        PrefabUtility.UnloadPrefabContents(instance);
        
        Debug.Log("✅ UltimateSkill에 이펙트 연결 완료!");
    }
}
