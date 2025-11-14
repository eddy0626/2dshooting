using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// 스프라이트 시트를 자동으로 Multiple 모드로 설정하고 슬라이스
/// </summary>
public class AutoSpriteSetup : EditorWindow
{
    [MenuItem("Tools/Auto Sprite Setup 📸")]
    public static void ShowWindow()
    {
        GetWindow<AutoSpriteSetup>("Sprite Setup");
    }

    void OnGUI()
    {
        GUILayout.Label("자동 스프라이트 설정", EditorStyles.boldLabel);
        
        EditorGUILayout.Space(10);
        
        if (GUILayout.Button("🎯 모든 스프라이트 자동 설정!", GUILayout.Height(50)))
        {
            SetupAllSprites();
        }
        
        EditorGUILayout.Space(10);
        
        EditorGUILayout.HelpBox("스프라이트를 Multiple 모드로 설정하고 자동 슬라이스합니다:\n" +
            "• Player.png\n" +
            "• Enemies.png\n" +
            "• Bullets.png\n" +
            "• Items.png\n" +
            "• Explosion.png\n" +
            "• Backgrounds.png", MessageType.Info);
    }

    private void SetupAllSprites()
    {
        Debug.Log("=== 📸 스프라이트 자동 설정 시작 ===");
        
        string spritesPath = "Assets/08.Assets/Vertical 2D Shooting BE4/Sprites";
        
        // 각 스프라이트 시트 설정
        SetupSpriteSheet(spritesPath + "/Player.png", 128, 128);
        SetupSpriteSheet(spritesPath + "/Enemies.png", 128, 128);
        SetupSpriteSheet(spritesPath + "/Bullets.png", 64, 64);
        SetupSpriteSheet(spritesPath + "/Items.png", 64, 64);
        SetupSpriteSheet(spritesPath + "/Explosion.png", 128, 128);
        SetupSpriteSheet(spritesPath + "/Boom.png", 128, 128);
        SetupSpriteSheet(spritesPath + "/Backgrounds.png", 256, 256);
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("=== ✅ 스프라이트 자동 설정 완료! ===");
        EditorUtility.DisplayDialog("완료!", "모든 스프라이트가 Multiple 모드로 설정되었습니다!", "확인");
    }

    private void SetupSpriteSheet(string path, int pixelsPerUnit, int spriteSize)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
        {
            Debug.LogWarning($"❌ {path}를 찾을 수 없습니다.");
            return;
        }

        // Sprite 모드로 변경
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.spritePixelsPerUnit = pixelsPerUnit;
        importer.filterMode = FilterMode.Point; // Pixel Perfect를 위해
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        
        // 읽기/쓰기 활성화
        importer.isReadable = true;
        
        // Import 설정 적용
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();
        
        // Automatic slicing
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (texture != null)
        {
            SliceSprite(importer, texture, spriteSize);
        }
        
        Debug.Log($"✅ {System.IO.Path.GetFileName(path)} 스프라이트 설정 완료!");
    }

    private void SliceSprite(TextureImporter importer, Texture2D texture, int spriteSize)
    {
        // Grid로 자동 슬라이스
        int cols = texture.width / spriteSize;
        int rows = texture.height / spriteSize;
        
        List<SpriteMetaData> spritesheet = new List<SpriteMetaData>();
        int spriteIndex = 0;
        
        for (int row = rows - 1; row >= 0; row--)
        {
            for (int col = 0; col < cols; col++)
            {
                SpriteMetaData smd = new SpriteMetaData();
                smd.pivot = new Vector2(0.5f, 0.5f);
                smd.alignment = (int)SpriteAlignment.Center;
                smd.name = $"{System.IO.Path.GetFileNameWithoutExtension(importer.assetPath)}_{spriteIndex}";
                smd.rect = new Rect(col * spriteSize, row * spriteSize, spriteSize, spriteSize);
                
                spritesheet.Add(smd);
                spriteIndex++;
            }
        }
        
        importer.spritesheet = spritesheet.ToArray();
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();
    }
}
