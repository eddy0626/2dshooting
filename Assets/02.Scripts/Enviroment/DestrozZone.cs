using UnityEngine;

/// <summary>
/// 이 스크립트가 적용된 오브젝트와 충돌하는 모든 게임 오브젝트를 파괴하는 역할을 합니다.
/// 이 기능을 사용하려면 오브젝트에 Collider2D 컴포넌트와 Rigidbody2D 컴포넌트가 필요하며,
/// Collider2D의 'Is Trigger' 옵션이 활성화되어 있어야 합니다.
/// </summary>
public class DestrozZone : MonoBehaviour
{
    // Start와 Update 메서드는 이 스크립트의 기능 구현에 필요하지 않으므로 제거합니다.

    /// <summary>
    /// 다른 2D Collider가 이 트리거(Is Trigger가 활성화된 Collider2D)에 진입했을 때 호출됩니다.
    /// 이 메서드는 Collider2D가 Is Trigger로 설정되어 있고, 다른 오브젝트에 Rigidbody2D가 있을 때 작동합니다.
    /// </summary>
    /// <param name="other">이 트리거에 진입한 다른 Collider2D 컴포넌트</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 적이 맵을 이탈한 경우, Enemy.Die()를 호출하여 아이템 드랍이 되도록 합니다.
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            Debug.Log($"DestrozZone: Enemy {other.gameObject.name}이(가) 맵을 이탈하여 Die() 호출됨.");
            enemy.Die();
            return;
        }

        // 총알, 아이템 등 다른 오브젝트는 그냥 파괴합니다.
        Debug.Log($"DestrozZone: {other.gameObject.name}이(가) 파괴되었습니다.");
        Destroy(other.gameObject);
    }
}
