using UnityEngine;
using System.Runtime.CompilerServices;

public static class PlayerMoveExtensions
{
    /// <summary>
    /// PlayerMove에 Heal이 없다면 확장 메서드로 사용됩니다.
    /// (있다면 원래 인스턴스 메서드가 우선 호출되므로 충돌하지 않습니다.)
    /// 현재 HP/MaxHP 필드 명칭이 다를 경우 이 메서드는 무시됩니다.
    /// </summary>
    public static void Heal(this PlayerMove pm, int amount)
    {
        var t = pm.GetType();
        var currentHPField = t.GetField("CurrentHP");
        var maxHPField = t.GetField("MaxHP");

        if (currentHPField != null && maxHPField != null)
        {
            int cur = (int)currentHPField.GetValue(pm);
            int max = (int)maxHPField.GetValue(pm);
            cur = Mathf.Min(cur + amount, max);
            currentHPField.SetValue(pm, cur);
            Debug.Log($"[Ext] Player Heal +{amount} → {cur}/{max}");
        }
        else
        {
            Debug.LogWarning("[Ext] PlayerMove에 CurrentHP/MaxHP 필드가 없어 Heal 확장을 적용하지 못했습니다.");
        }
    }
}
