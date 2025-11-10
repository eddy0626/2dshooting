using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerHitCounter : MonoBehaviour
{
    [Header("설정")]
    public string enemyTag = "Enemy";
    public int deathOnHits = 3;

    private int _hits = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(enemyTag))
        {
            _hits++;
            Debug.Log($"[HitCounter] Player touched enemy: {_hits}/{deathOnHits}");
            if (_hits >= deathOnHits)
            {
                // 플레이어 사망 처리: PlayerMove에 Die가 있으면 호출, 아니면 오브젝트 제거
                var pm = GetComponent<PlayerMove>();
                var dieMethod = pm ? pm.GetType().GetMethod("Die", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic) : null;

                if (dieMethod != null)
                    dieMethod.Invoke(pm, null);
                else
                    Destroy(gameObject);
            }
        }
    }
}
