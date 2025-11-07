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
        // 트리거에 진입한 게임 오브젝트를 파괴합니다.
        // Destroy(other.gameObject)는 충돌한 대상 오브젝트를 파괴하며,
        // Destroy(gameObject)는 이 스크립트가 붙어있는 DestrozZone 자체를 파괴합니다.
        // 여기서는 충돌한 대상 오브젝트를 파괴하는 것이 목표이므로 other.gameObject를 사용합니다.
        Debug.Log($"DestrozZone: {other.gameObject.name}이(가) 파괴되었습니다.");
        Destroy(other.gameObject);
    }
}
