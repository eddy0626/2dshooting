using UnityEngine;

/// <summary>
/// 적이 피격당했을 때 효과
/// </summary>
public class EnemyHitEffect : MonoBehaviour
{
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private Color hitColor = Color.red;
    
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;
    
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null)
        {
            _originalColor = _spriteRenderer.color;
        }
    }
    
    /// <summary>
    /// 피격 효과 재생
    /// </summary>
    public void PlayHitEffect()
    {
        if (_spriteRenderer != null)
        {
            StartCoroutine(FlashEffect());
        }
    }
    
    System.Collections.IEnumerator FlashEffect()
    {
        _spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        _spriteRenderer.color = _originalColor;
    }
}