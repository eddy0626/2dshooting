using UnityEngine;

public class UltimateSkill : MonoBehaviour
{
    [SerializeField] private float _duration = 3f;
    [SerializeField] private float _radius = 10f;
    [SerializeField] private float _cooldown = 10f;
    
    private float _nextUseTime = 0f;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
            ActivateUltimate();
    }
    
    private void ActivateUltimate()
    {
        if (Time.time < _nextUseTime)
        {
            Debug.Log($"Ultimate on cooldown! {_nextUseTime - Time.time:F1}s remaining");
            return;
        }
        
        Debug.Log("Ultimate Activated!");
        _nextUseTime = Time.time + _cooldown;
        
        GameObject ultimate = new GameObject("UltimateEffect");
        ultimate.transform.position = Vector3.zero;
        
        UltimateEffect effect = ultimate.AddComponent<UltimateEffect>();
        effect.Initialize(_duration, _radius);
    }
}

public class UltimateEffect : MonoBehaviour
{
    private float _duration;
    private float _radius;
    
    public void Initialize(float duration, float radius)
    {
        _duration = duration;
        _radius = radius;
        
        CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = _radius;
        
        transform.localScale = Vector3.one * _radius * 2f;
        
        Destroy(gameObject, _duration);
    }
    
    private void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _radius);
        
        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag("Enemy"))
            {
                Enemy enemy = col.GetComponent<Enemy>();
                if (enemy != null)
                    enemy.TakeDamage(9999f);
            }
        }
    }
}