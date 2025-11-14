using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// 그래픽과 애니메이션을 자동으로 설정하는 완전 자동화 스크립트
/// </summary>
public class CompleteGraphicsSetup : EditorWindow
{
    private const string SPRITES_PATH = "Assets/08.Assets/Vertical 2D Shooting BE4/Sprites";
    private const string ANIMATIONS_PATH = "Assets/08.Assets/Animations";
    private const string PREFABS_PATH = "Assets/03.Prefabs";
    private const string EFFECTS_PATH = "Assets/08.Assets/JMO Assets/Cartoon FX Remaster/CFXR Prefabs";
    
    [MenuItem("Tools/Complete Graphics Setup 🎨")]
    public static void ShowWindow()
    {
        GetWindow<CompleteGraphicsSetup>("Graphics Setup");
    }

    void OnGUI()
    {
        GUILayout.Label("자동 그래픽 설정", EditorStyles.boldLabel);
        
        EditorGUILayout.Space(10);
        
        if (GUILayout.Button("🚀 모든 그래픽 자동 설정 시작!", GUILayout.Height(50)))
        {
            SetupAllGraphics();
        }
        
        EditorGUILayout.Space(10);
        
        if (GUILayout.Button("1️⃣ 스프라이트만 설정", GUILayout.Height(30)))
        {
            SetupSprites();
        }
        
        if (GUILayout.Button("2️⃣ 애니메이션만 연결", GUILayout.Height(30)))
        {
            SetupAnimations();
        }
        
        if (GUILayout.Button("3️⃣ 이펙트만 추가", GUILayout.Height(30)))
        {
            SetupEffects();
        }
        
        EditorGUILayout.Space(10);
        
        EditorGUILayout.HelpBox("'모든 그래픽 자동 설정 시작!' 버튼을 누르면 자동으로:\n" +
            "✅ Player 스프라이트 설정\n" +
            "✅ Enemy 스프라이트 설정\n" +
            "✅ Bullet 스프라이트 설정\n" +
            "✅ Item 스프라이트 설정\n" +
            "✅ 모든 애니메이션 연결\n" +
            "✅ 폭발/히트 이펙트 추가", MessageType.Info);
    }

    private void SetupAllGraphics()
    {
        Debug.Log("=== 🎨 그래픽 자동 설정 시작 ===");
        
        SetupSprites();
        SetupAnimations();
        SetupEffects();
        
        Debug.Log("=== ✅ 그래픽 자동 설정 완료! ===");
        EditorUtility.DisplayDialog("완료!", "모든 그래픽과 애니메이션 설정이 완료되었습니다!", "확인");
    }

    private void SetupSprites()
    {
        Debug.Log(">>> 스프라이트 설정 시작");
        
        // Player 설정
        SetupPlayerSprites();
        
        // Enemy 설정
        SetupEnemySprites();
        
        // Bullet 설정
        SetupBulletSprites();
        
        // Item 설정
        SetupItemSprites();
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log(">>> 스프라이트 설정 완료!");
    }

    private void SetupPlayerSprites()
    {
        string playerSpritePath = SPRITES_PATH + "/Player.png";
        Sprite[] playerSprites = AssetDatabase.LoadAllAssetsAtPath(playerSpritePath)
            .OfType<Sprite>().ToArray();
        
        if (playerSprites.Length == 0)
        {
            Debug.LogWarning("Player 스프라이트를 찾을 수 없습니다. 스프라이트 시트를 Multiple로 설정해주세요.");
            return;
        }
        
        // Player 프리팹 찾기 또는 생성
        string prefabPath = PREFABS_PATH + "/Player.prefab";
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (playerPrefab == null)
        {
            // 프리팹 생성
            GameObject playerObj = new GameObject("Player");
            SpriteRenderer sr = playerObj.AddComponent<SpriteRenderer>();
            sr.sprite = playerSprites[0];
            
            // 태그와 레이어 설정
            playerObj.tag = "Player";
            playerObj.layer = LayerMask.NameToLayer("Player");
            
            // Collider 추가
            BoxCollider2D col = playerObj.AddComponent<BoxCollider2D>();
            
            // Rigidbody 추가
            Rigidbody2D rb = playerObj.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            
            // 프리팹 저장
            PrefabUtility.SaveAsPrefabAsset(playerObj, prefabPath);
            DestroyImmediate(playerObj);
            
            Debug.Log("✅ Player 프리팹 생성 완료!");
        }
        else
        {
            // 기존 프리팹 업데이트
            GameObject instance = PrefabUtility.LoadPrefabContents(prefabPath);
            SpriteRenderer sr = instance.GetComponent<SpriteRenderer>();
            if (sr == null) sr = instance.AddComponent<SpriteRenderer>();
            sr.sprite = playerSprites[0];
            
            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            PrefabUtility.UnloadPrefabContents(instance);
            
            Debug.Log("✅ Player 프리팹 업데이트 완료!");
        }
    }

    private void SetupEnemySprites()
    {
        string enemySpritePath = SPRITES_PATH + "/Enemies.png";
        Sprite[] enemySprites = AssetDatabase.LoadAllAssetsAtPath(enemySpritePath)
            .OfType<Sprite>().ToArray();
        
        if (enemySprites.Length == 0)
        {
            Debug.LogWarning("Enemy 스프라이트를 찾을 수 없습니다. 스프라이트 시트를 Multiple로 설정해주세요.");
            return;
        }
        
        // Enemy 프리팹 찾기 또는 생성
        string prefabPath = PREFABS_PATH + "/Enemy.prefab";
        GameObject enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (enemyPrefab == null)
        {
            GameObject enemyObj = new GameObject("Enemy");
            SpriteRenderer sr = enemyObj.AddComponent<SpriteRenderer>();
            sr.sprite = enemySprites[0];
            
            enemyObj.tag = "Enemy";
            enemyObj.layer = LayerMask.NameToLayer("Enemy");
            
            BoxCollider2D col = enemyObj.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            
            Rigidbody2D rb = enemyObj.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            
            PrefabUtility.SaveAsPrefabAsset(enemyObj, prefabPath);
            DestroyImmediate(enemyObj);
            
            Debug.Log("✅ Enemy 프리팹 생성 완료!");
        }
        else
        {
            GameObject instance = PrefabUtility.LoadPrefabContents(prefabPath);
            SpriteRenderer sr = instance.GetComponent<SpriteRenderer>();
            if (sr == null) sr = instance.AddComponent<SpriteRenderer>();
            sr.sprite = enemySprites[0];
            
            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            PrefabUtility.UnloadPrefabContents(instance);
            
            Debug.Log("✅ Enemy 프리팹 업데이트 완료!");
        }
    }

    private void SetupBulletSprites()
    {
        string bulletSpritePath = SPRITES_PATH + "/Bullets.png";
        Sprite[] bulletSprites = AssetDatabase.LoadAllAssetsAtPath(bulletSpritePath)
            .OfType<Sprite>().ToArray();
        
        if (bulletSprites.Length == 0)
        {
            Debug.LogWarning("Bullet 스프라이트를 찾을 수 없습니다. 스프라이트 시트를 Multiple로 설정해주세요.");
            return;
        }
        
        // Bullet 프리팹 찾기 또는 생성
        string prefabPath = PREFABS_PATH + "/Bullet.prefab";
        GameObject bulletPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (bulletPrefab == null)
        {
            GameObject bulletObj = new GameObject("Bullet");
            SpriteRenderer sr = bulletObj.AddComponent<SpriteRenderer>();
            sr.sprite = bulletSprites[0];
            sr.sortingOrder = 5;
            
            bulletObj.tag = "Bullet";
            bulletObj.layer = LayerMask.NameToLayer("Bullet");
            
            CircleCollider2D col = bulletObj.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.15f;
            
            Rigidbody2D rb = bulletObj.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            
            PrefabUtility.SaveAsPrefabAsset(bulletObj, prefabPath);
            DestroyImmediate(bulletObj);
            
            Debug.Log("✅ Bullet 프리팹 생성 완료!");
        }
        else
        {
            GameObject instance = PrefabUtility.LoadPrefabContents(prefabPath);
            SpriteRenderer sr = instance.GetComponent<SpriteRenderer>();
            if (sr == null) sr = instance.AddComponent<SpriteRenderer>();
            sr.sprite = bulletSprites[0];
            sr.sortingOrder = 5;
            
            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            PrefabUtility.UnloadPrefabContents(instance);
            
            Debug.Log("✅ Bullet 프리팹 업데이트 완료!");
        }
    }

    private void SetupItemSprites()
    {
        string itemSpritePath = SPRITES_PATH + "/Items.png";
        Sprite[] itemSprites = AssetDatabase.LoadAllAssetsAtPath(itemSpritePath)
            .OfType<Sprite>().ToArray();
        
        if (itemSprites.Length == 0)
        {
            Debug.LogWarning("Item 스프라이트를 찾을 수 없습니다. 스프라이트 시트를 Multiple로 설정해주세요.");
            return;
        }
        
        // Item 프리팹 찾기 또는 생성
        string prefabPath = PREFABS_PATH + "/Item.prefab";
        GameObject itemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (itemPrefab == null)
        {
            GameObject itemObj = new GameObject("Item");
            SpriteRenderer sr = itemObj.AddComponent<SpriteRenderer>();
            sr.sprite = itemSprites[0];
            sr.sortingOrder = 3;
            
            itemObj.tag = "Item";
            itemObj.layer = LayerMask.NameToLayer("Item");
            
            CircleCollider2D col = itemObj.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.3f;
            
            Rigidbody2D rb = itemObj.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            
            PrefabUtility.SaveAsPrefabAsset(itemObj, prefabPath);
            DestroyImmediate(itemObj);
            
            Debug.Log("✅ Item 프리팹 생성 완료!");
        }
        else
        {
            GameObject instance = PrefabUtility.LoadPrefabContents(prefabPath);
            SpriteRenderer sr = instance.GetComponent<SpriteRenderer>();
            if (sr == null) sr = instance.AddComponent<SpriteRenderer>();
            sr.sprite = itemSprites[0];
            sr.sortingOrder = 3;
            
            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            PrefabUtility.UnloadPrefabContents(instance);
            
            Debug.Log("✅ Item 프리팹 업데이트 완료!");
        }
    }

    private void SetupAnimations()
    {
        Debug.Log(">>> 애니메이션 연결 시작");
        
        // Bullet 애니메이션 연결
        ConnectBulletAnimation();
        
        // Enemy 애니메이션 연결
        ConnectEnemyAnimation();
        
        // Item 애니메이션 연결
        ConnectItemAnimation();
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log(">>> 애니메이션 연결 완료!");
    }

    private void ConnectBulletAnimation()
    {
        string prefabPath = PREFABS_PATH + "/Bullet.prefab";
        string animatorPath = ANIMATIONS_PATH + "/BulletAnimator.controller";
        
        RuntimeAnimatorController animator = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(animatorPath);
        if (animator == null)
        {
            Debug.LogWarning("BulletAnimator.controller를 찾을 수 없습니다.");
            return;
        }
        
        GameObject bulletPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (bulletPrefab != null)
        {
            GameObject instance = PrefabUtility.LoadPrefabContents(prefabPath);
            Animator anim = instance.GetComponent<Animator>();
            if (anim == null) anim = instance.AddComponent<Animator>();
            anim.runtimeAnimatorController = animator;
            
            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            PrefabUtility.UnloadPrefabContents(instance);
            
            Debug.Log("✅ Bullet 애니메이션 연결 완료!");
        }
    }

    private void ConnectEnemyAnimation()
    {
        string prefabPath = PREFABS_PATH + "/Enemy.prefab";
        string animatorPath = ANIMATIONS_PATH + "/EnemyAnimator.controller";
        
        RuntimeAnimatorController animator = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(animatorPath);
        if (animator == null)
        {
            Debug.LogWarning("EnemyAnimator.controller를 찾을 수 없습니다.");
            return;
        }
        
        GameObject enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (enemyPrefab != null)
        {
            GameObject instance = PrefabUtility.LoadPrefabContents(prefabPath);
            Animator anim = instance.GetComponent<Animator>();
            if (anim == null) anim = instance.AddComponent<Animator>();
            anim.runtimeAnimatorController = animator;
            
            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            PrefabUtility.UnloadPrefabContents(instance);
            
            Debug.Log("✅ Enemy 애니메이션 연결 완료!");
        }
    }

    private void ConnectItemAnimation()
    {
        string prefabPath = PREFABS_PATH + "/Item.prefab";
        string animatorPath = ANIMATIONS_PATH + "/ItemAnimator.controller";
        
        RuntimeAnimatorController animator = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(animatorPath);
        if (animator == null)
        {
            Debug.LogWarning("ItemAnimator.controller를 찾을 수 없습니다.");
            return;
        }
        
        GameObject itemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (itemPrefab != null)
        {
            GameObject instance = PrefabUtility.LoadPrefabContents(prefabPath);
            Animator anim = instance.GetComponent<Animator>();
            if (anim == null) anim = instance.AddComponent<Animator>();
            anim.runtimeAnimatorController = animator;
            
            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            PrefabUtility.UnloadPrefabContents(instance);
            
            Debug.Log("✅ Item 애니메이션 연결 완료!");
        }
    }

    private void SetupEffects()
    {
        Debug.Log(">>> 이펙트 추가 시작");
        
        // 폭발 이펙트 복사
        CopyEffectPrefab("Explosions/CFXR Explosion 1.prefab", "ExplosionEffect");
        
        // 히트 이펙트 복사
        CopyEffectPrefab("Impacts/CFXR Hit A (Red).prefab", "HitEffect");
        
        // 물 스플래시 이펙트 (아이템 수집용)
        CopyEffectPrefab("Liquids/CFXR Water Splash (Smaller).prefab", "CollectEffect");
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log(">>> 이펙트 추가 완료!");
    }

    private void CopyEffectPrefab(string sourcePath, string newName)
    {
        string fullSourcePath = EFFECTS_PATH + "/" + sourcePath;
        string destPath = PREFABS_PATH + "/" + newName + ".prefab";
        
        if (AssetDatabase.LoadAssetAtPath<GameObject>(destPath) != null)
        {
            Debug.Log($"⏭️ {newName}은 이미 존재합니다.");
            return;
        }
        
        if (AssetDatabase.CopyAsset(fullSourcePath, destPath))
        {
            Debug.Log($"✅ {newName} 이펙트 추가 완료!");
        }
        else
        {
            Debug.LogWarning($"❌ {sourcePath}를 복사할 수 없습니다.");
        }
    }
}


