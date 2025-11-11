using System.Collections;
using UnityEngine;

/// <summary>
/// 폭발 효과를 재생하고 자동으로 파괴됩니다.
/// 스프라이트 애니메이션, 스케일 효과, 페이드 아웃 등을 지원합니다.
/// </summary>
public class ExplosionEffect : MonoBehaviour
{
    [Header("폭발 효과 설정")]
    [SerializeField] private float LifeTime = 1f; // 폭발 효과 지속 시간 (초)
    [SerializeField] private bool AutoDestroy = true; // 자동 파괴 여부

    [Header("스케일 애니메이션")]
    [SerializeField] private bool EnableScaleAnimation = true;
    [SerializeField] private AnimationCurve ScaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // 스케일 커브
    [SerializeField] private float MaxScale = 2f; // 최대 스케일

    [Header("페이드 아웃")]
    [SerializeField] private bool EnableFadeOut = true;
    [SerializeField] private float FadeOutDelay = 0.3f; // 페이드 아웃 시작 지연 시간

    [Header("회전 효과")]
    [SerializeField] private bool EnableRotation = false;
    [SerializeField] private float RotationSpeed = 360f; // 초당 회전 각도

    [Header("컴포넌트")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator; // 선택사항: 스프라이트 애니메이션
    [SerializeField] private ParticleSystem _particleSystem; // 선택사항: 파티클 시스템

    private Vector3 _initialScale;
    private Color _initialColor;
    private float _elapsedTime = 0f;

    /// <summary>
    /// 초기화
    /// </summary>
    void Start()
    {
        // 컴포넌트 자동 찾기
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_animator == null)
            _animator = GetComponent<Animator>();

        if (_particleSystem == null)
            _particleSystem = GetComponent<ParticleSystem>();

        // 초기값 저장
        _initialScale = transform.localScale;
        if (_spriteRenderer != null)
            _initialColor = _spriteRenderer.color;

        // 파티클 시스템 재생
        if (_particleSystem != null)
        {
            _particleSystem.Play();
        }

        // 자동 파괴 설정
        if (AutoDestroy)
        {
            Destroy(gameObject, LifeTime);
        }
    }

    /// <summary>
    /// 매 프레임 효과 업데이트
    /// </summary>
    void Update()
    {
        _elapsedTime += Time.deltaTime;
        float normalizedTime = Mathf.Clamp01(_elapsedTime / LifeTime);

        // 스케일 애니메이션
        if (EnableScaleAnimation)
        {
            float scaleMultiplier = ScaleCurve.Evaluate(normalizedTime) * MaxScale;
            transform.localScale = _initialScale * scaleMultiplier;
        }

        // 페이드 아웃
        if (EnableFadeOut && _spriteRenderer != null && _elapsedTime >= FadeOutDelay)
        {
            float fadeTime = _elapsedTime - FadeOutDelay;
            float fadeDuration = LifeTime - FadeOutDelay;
            float alpha = Mathf.Lerp(1f, 0f, fadeTime / fadeDuration);

            Color color = _initialColor;
            color.a = alpha;
            _spriteRenderer.color = color;
        }

        // 회전 효과
        if (EnableRotation)
        {
            transform.Rotate(0f, 0f, RotationSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// 특정 위치에 폭발 효과를 생성합니다. (정적 헬퍼 메서드)
    /// </summary>
    /// <param name="explosionPrefab">폭발 효과 프리팹</param>
    /// <param name="position">생성 위치</param>
    /// <param name="rotation">회전값 (기본: Quaternion.identity)</param>
    /// <returns>생성된 폭발 효과 GameObject</returns>
    public static GameObject CreateExplosion(GameObject explosionPrefab, Vector3 position, Quaternion rotation = default)
    {
        if (explosionPrefab == null)
        {
            Debug.LogWarning("ExplosionEffect: 폭발 프리팹이 null입니다!");
            return null;
        }

        if (rotation == default)
            rotation = Quaternion.identity;

        GameObject explosion = Instantiate(explosionPrefab, position, rotation);
        return explosion;
    }
}