using UnityEngine;

/// <summary>
/// 게임 시작 시 자동으로 BulletSystem의 프리팹을 연결
/// </summary>
public class AutoConnectBulletPrefabs : MonoBehaviour
{
    private void Awake()
    {
        // Player 찾기
        GameObject player = GameObject.Find("Player");
        if (player == null)
        {
            Debug.LogWarning("Player를 찾을 수 없습니다!");
            return;
        }
        
        // BulletSystem 가져오기
        BulletSystem bulletSystem = player.GetComponent<BulletSystem>();
        if (bulletSystem == null)
        {
            Debug.LogWarning("Player에 BulletSystem이 없습니다!");
            return;
        }
        
        // Resources 폴더에서 프리팹 로드
        GameObject mainBullet = Resources.Load<GameObject>("Bullet/MainBullet");
        GameObject subBullet = Resources.Load<GameObject>("Bullet/SubBullet");
        
        if (mainBullet != null && subBullet != null)
        {
            // Reflection을 사용하여 private 필드에 접근
            var mainField = typeof(BulletSystem).GetField("_mainBulletPrefab", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var subField = typeof(BulletSystem).GetField("_subBulletPrefab", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (mainField != null && subField != null)
            {
                mainField.SetValue(bulletSystem, mainBullet);
                subField.SetValue(bulletSystem, subBullet);
                Debug.Log("✅ Bullet 프리팹 자동 연결 완료!");
            }
        }
    }
}
