using UnityEngine;
using UnityEditor;
using System.Linq;

/// <summary>
/// 프리팹에 올바른 스프라이트 재연결
/// </summary>
public class ReconnectSpritesToPrefabs : EditorWindow
{
    [MenuItem("Tools/Reconnect Sprites to Prefabs 🔗")]
    public static void ShowWindow()
    {
        GetWindow<ReconnectSpritesToPrefabs>("Reconnect Sprites");
    }

    void OnGUI()
    {
        GUILayout.Label("프리팹 스프라이트 재연결", EditorStyles.boldLabel);
        
        EditorGUILayout.Space(10);
        
        if (GUILayout.Button("🔗 프리팹에 스프라이트 재연결!", GUILayout.Height(50)))
        {
            ReconnectAllSprites();
        }
        
        EditorGUILayout.Space(10);
        
        EditorGUILayout.HelpBox("프리팹들에 올바른 스프라이트를 다시 연결합니다:\n" +
            "• Player 프리팹\n" +
            "• Enemy 프리팹\n" +
            "• Bullet 프리팹\n" +
            "• Item 프리팹", MessageType.Info);
    }

    private void ReconnectAllSprites()
    {
        Debug.Log("=== 🔗 스프라이트 재연결 시작 ===");
        
        string spritePath = "Assets/08.Assets/Vertical 2D Shooting BE4/Sprites";
        string prefabPath = "Assets/03.Prefabs";
        
        // Player
        ReconnectPlayerSprite(spritePath + "/Player.png", prefabPath + "/Player.prefab");
        
        // Enemy
        ReconnectEnemySprite(spritePath + "/Enemies.png", prefabPath + "/Enemy.prefab");
        
        // Bullet
        ReconnectBulletSprite(spritePath + "/Bullets.png", prefabPath + "/Bullet.prefab");
        
        // Items
        ReconnectItemSprite(spritePath + "/Items.png", prefabPath + "/Item_Health.prefab", 0);
        ReconnectItemSprite(spritePath + "/Items.png", prefabPath + "/Item_Speed.prefab", 1);
        ReconnectItemSprite(spritePath + "/Items.png", prefabPath + "/Item_AttackSpeed.prefab", 2);
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("=== ✅ 스프라이트 재연결 완료! ===");
        EditorUtility.DisplayDialog("완료!", "모든 프리팹에 스프라이트가 재연결되었습니다!", "확인");
    }

    private void ReconnectPlayerSprite(string spritePath, string prefabPath)
    {
        Sprite[] sprites = LoadSprites(spritePath);
        if (sprites.Length == 0) return;
        
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogWarning($"❌ {prefabPath} 프리팹을 찾을 수 없습니다.");
            return;
        }
        
        GameObject instance = PrefabUtility.LoadPrefabContents(prefabPath);
        SpriteRenderer sr = instance.GetComponent<SpriteRenderer>();
        
        if (sr != null && sprites.Length > 0)
        {
            sr.sprite = sprites[0]; // 첫 번째 스프라이트 사용
            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            Debug.Log($"✅ Player 스프라이트 재연결 완료!");
        }
        
        PrefabUtility.UnloadPrefabContents(instance);
    }

    private void ReconnectEnemySprite(string spritePath, string prefabPath)
    {
        Sprite[] sprites = LoadSprites(spritePath);
        if (sprites.Length == 0) return;
        
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogWarning($"❌ {prefabPath} 프리팹을 찾을 수 없습니다.");
            return;
        }
        
        GameObject instance = PrefabUtility.LoadPrefabContents(prefabPath);
        SpriteRenderer sr = instance.GetComponent<SpriteRenderer>();
        
        if (sr != null && sprites.Length > 0)
        {
            sr.sprite = sprites[0]; // 첫 번째 적 스프라이트
            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            Debug.Log($"✅ Enemy 스프라이트 재연결 완료!");
        }
        
        PrefabUtility.UnloadPrefabContents(instance);
    }

    private void ReconnectBulletSprite(string spritePath, string prefabPath)
    {
        Sprite[] sprites = LoadSprites(spritePath);
        if (sprites.Length == 0) return;
        
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogWarning($"❌ {prefabPath} 프리팹을 찾을 수 없습니다.");
            return;
        }
        
        GameObject instance = PrefabUtility.LoadPrefabContents(prefabPath);
        SpriteRenderer sr = instance.GetComponent<SpriteRenderer>();
        
        if (sr != null && sprites.Length > 0)
        {
            sr.sprite = sprites[0]; // 첫 번째 총알 스프라이트
            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            Debug.Log($"✅ Bullet 스프라이트 재연결 완료!");
        }
        
        PrefabUtility.UnloadPrefabContents(instance);
    }

    private void ReconnectItemSprite(string spritePath, string prefabPath, int spriteIndex)
    {
        Sprite[] sprites = LoadSprites(spritePath);
        if (sprites.Length <= spriteIndex) return;
        
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogWarning($"❌ {prefabPath} 프리팹을 찾을 수 없습니다.");
            return;
        }
        
        GameObject instance = PrefabUtility.LoadPrefabContents(prefabPath);
        SpriteRenderer sr = instance.GetComponent<SpriteRenderer>();
        
        if (sr != null && sprites.Length > spriteIndex)
        {
            sr.sprite = sprites[spriteIndex];
            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            Debug.Log($"✅ {System.IO.Path.GetFileName(prefabPath)} 스프라이트 재연결 완료!");
        }
        
        PrefabUtility.UnloadPrefabContents(instance);
    }

    private Sprite[] LoadSprites(string path)
    {
        Object[] objects = AssetDatabase.LoadAllAssetsAtPath(path);
        return objects.Where(o => o is Sprite).Cast<Sprite>().ToArray();
    }
}
