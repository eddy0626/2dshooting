using UnityEngine;
using UnityEditor;
using System.Linq;

/// <summary>
/// Vertical 2D Shooting BE4 스프라이트를 올바르게 재설정
/// </summary>
public class FixSpritesSetup : EditorWindow
{
    [MenuItem("Tools/Fix Sprites 🔧")]
    public static void ShowWindow()
    {
        GetWindow<FixSpritesSetup>("Fix Sprites");
    }

    void OnGUI()
    {
        GUILayout.Label("스프라이트 재설정", EditorStyles.boldLabel);
        
        EditorGUILayout.Space(10);
        
        if (GUILayout.Button("🔧 스프라이트 완전 재설정!", GUILayout.Height(50)))
        {
            FixAllSprites();
        }
        
        EditorGUILayout.Space(10);
        
        EditorGUILayout.HelpBox("Vertical 2D Shooting BE4 스프라이트를 다시 설정합니다:\n" +
            "• Player.png → Multiple, Slice\n" +
            "• Enemies.png → Multiple, Slice\n" +
            "• Bullets.png → Multiple, Slice\n" +
            "• Items.png → Multiple, Slice\n" +
            "• Backgrounds.png → Multiple, Slice", MessageType.Info);
    }

    private void FixAllSprites()
    {
        Debug.Log("=== 🔧 스프라이트 재설정 시작 ===");
        
        string basePath = "Assets/08.Assets/Vertical 2D Shooting BE4/Sprites";
        
        // 각 스프라이트 시트 재설정
        ConfigureSprite(basePath + "/Player.png", SpriteImportMode.Multiple, 100, new Vector2Int(32, 32));
        ConfigureSprite(basePath + "/Enemies.png", SpriteImportMode.Multiple, 100, new Vector2Int(32, 32));
        ConfigureSprite(basePath + "/Bullets.png", SpriteImportMode.Multiple, 100, new Vector2Int(16, 16));
        ConfigureSprite(basePath + "/Items.png", SpriteImportMode.Multiple, 100, new Vector2Int(16, 16));
        ConfigureSprite(basePath + "/Explosion.png", SpriteImportMode.Multiple, 100, new Vector2Int(32, 32));
        ConfigureSprite(basePath + "/Boom.png", SpriteImportMode.Multiple, 100, new Vector2Int(32, 32));
        ConfigureSprite(basePath + "/Backgrounds.png", SpriteImportMode.Multiple, 100, new Vector2Int(64, 64));
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("=== ✅ 스프라이트 재설정 완료! ===");
        EditorUtility.DisplayDialog("완료!", "모든 스프라이트가 재설정되었습니다!\n\n이제 프리팹을 다시 설정해주세요.", "확인");
    }

    private void ConfigureSprite(string path, SpriteImportMode mode, float pixelsPerUnit, Vector2Int spriteSize)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
        {
            Debug.LogWarning($"❌ {path}를 찾을 수 없습니다.");
            return;
        }

        // 기본 설정
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = mode;
        importer.spritePixelsPerUnit = pixelsPerUnit;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.isReadable = true;
        importer.mipmapEnabled = false;
        importer.alphaSource = TextureImporterAlphaSource.FromInput;
        importer.alphaIsTransparency = true;
        
        // 먼저 저장
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();
        
        // Multiple 모드일 경우 자동 슬라이싱
        if (mode == SpriteImportMode.Multiple)
        {
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (texture != null)
            {
                AutoSliceSprite(importer, texture, spriteSize.x, spriteSize.y);
            }
        }
        
        Debug.Log($"✅ {System.IO.Path.GetFileName(path)} 설정 완료!");
    }

    private void AutoSliceSprite(TextureImporter importer, Texture2D texture, int width, int height)
    {
        int cols = texture.width / width;
        int rows = texture.height / height;
        
        var spritesheet = new System.Collections.Generic.List<SpriteMetaData>();
        int index = 0;
        
        // 위에서 아래로, 왼쪽에서 오른쪽으로
        for (int row = rows - 1; row >= 0; row--)
        {
            for (int col = 0; col < cols; col++)
            {
                SpriteMetaData meta = new SpriteMetaData();
                meta.pivot = new Vector2(0.5f, 0.5f);
                meta.alignment = (int)SpriteAlignment.Center;
                meta.name = $"{System.IO.Path.GetFileNameWithoutExtension(importer.assetPath)}_{index}";
                meta.rect = new Rect(col * width, row * height, width, height);
                
                spritesheet.Add(meta);
                index++;
            }
        }
        
        importer.spritesheet = spritesheet.ToArray();
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();
    }
}
