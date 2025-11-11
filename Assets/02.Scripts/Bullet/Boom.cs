using System.Collections;
using UnityEngine;

/// <summary>
/// 필살기 '붐' - 화면 중앙에 생성되어 3초간 지속되며 닿는 모든 에너미를 즉사시킵니다.
/// 스프라이트 애니메이션, 파티클 효과, 스케일/회전/페이드 효과를 지원합니다.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Boom : MonoBehaviour
{
    [Header("필살기 설정")]
    [SerializeField] private float Duration = 3f; // 필살기 지속 시간 (초)
    [SerializeField] private bool AutoDestroy = true; // 자동 파괴 여부

    [Header("스케일 애니메이션")]
    [SerializeField] private bool EnableScaleAnimation = true;
    [SerializeField] private float StartScale = 0.5f; // 시작 스케일
    [SerializeField] private float MaxScale = 5f; // 최대 스케일
    [SerializeField] private AnimationCurve ScaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // 스케일 커브

    [Header("회전 효과")]
    [SerializeField] private bool EnableRotation = true;
    [SerializeField] private float RotationSpeed = 180f; // 초당 회전 각도

    [Header("페이드 효과")]
    [SerializeField] private bool EnableFadeOut = true;
    [SerializeField] private float FadeOutStartTime = 2f; // 페이드 아웃 시작 시간

    [Header("펄스 효과 (깜빡임)")]
    [SerializeField] private bool EnablePulse = true;
    [SerializeField] private float PulseSpeed = 5f; // 펄스 속도
    [SerializeField] private float PulseMin = 0.7f; // 최소 밝기
    [SerializeField] private float PulseMax = 1.3f; // 최대 밝기

    [Header("컴포넌트")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private ParticleSystem _particleSystem; // 파티클 시스템
    [SerializeField] private Animator _animator; // 스프라이트 애니메이션
    [SerializeField] private Collider2D _collider; // 충돌 범위

    private Vector3 _initialScale;
    private Color _originalColor;
    private float _elapsedTime = 0f;

    /// <summary>
    /// 초기화
    /// </summary>
    void Start()
    {
        // 컴포넌트 자동 찾기
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_particleSystem == null)
            _particleSystem = GetComponent<ParticleSystem>();

        if (_animator == null)
            _animator = GetComponent<Animator>();

        if (_collider == null)
            _collider = GetComponent<Collider2D>();

        // Collider를 Trigger로 설정
        if (_collider != null)
        {
            _collider.isTrigger = true;
        }

        // 초기값 저장
        _initialScale = new Vector3(StartScale, StartScale, 1f);
        transform.localScale = _initialScale;

        if (_spriteRenderer != null)
        {
            _originalColor = _spriteRenderer.color;
        }

        // 파티클 시스템 재생
        if (_particleSystem != null)
        {
            _particleSystem.Play();
        }

        // 자동 파괴 설정
        if (AutoDestroy)
        {
            Destroy(gameObject, Duration);
        }

        Debug.Log("필살기 '붐' 발동!");
    }

    /// <summary>
    /// 매 프레임 효과 업데이트
    /// </summary>
    void Update()
    {
        _elapsedTime += Time.deltaTime;
        float normalizedTime = Mathf.Clamp01(_elapsedTime / Duration);

        // 스케일 애니메이션 (점점 커짐)
        if (EnableScaleAnimation)
        {
            float scaleValue = Mathf.Lerp(StartScale, MaxScale, ScaleCurve.Evaluate(normalizedTime));
            transform.localScale = new Vector3(scaleValue, scaleValue, 1f);
        }

        // 회전 효과
        if (EnableRotation)
        {
            transform.Rotate(0f, 0f, RotationSpeed * Time.deltaTime);
        }

        // 스프라이트 색상 효과 처리
        if (_spriteRenderer != null)
        {
            Color finalColor = _originalColor;

            // 펄스 효과 (깜빡임)
            if (EnablePulse)
            {
                float pulse = Mathf.Lerp(PulseMin, PulseMax, (Mathf.Sin(Time.time * PulseSpeed) + 1f) / 2f);
                finalColor.r *= pulse;
                finalColor.g *= pulse;
                finalColor.b *= pulse;
            }

            // 페이드 아웃 (마지막 구간)
            if (EnableFadeOut && _elapsedTime >= FadeOutStartTime)
            {
                float fadeTime = _elapsedTime - FadeOutStartTime;
                float fadeDuration = Duration - FadeOutStartTime;
                float alpha = Mathf.Lerp(_originalColor.a, 0f, fadeTime / fadeDuration);
                finalColor.a = alpha;
            }

            _spriteRenderer.color = finalColor;
        }
    }

    /// <summary>
    /// 에너미와 충돌 시 즉사시킵니다.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 에너미 태그를 가진 오브젝트와 충돌 시
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && !enemy.IsDying())
            {
                Debug.Log($"필살기가 {other.name}을(를) 처치했습니다!");
                enemy.Die(); // 에너미 즉사
            }
        }
    }

    /// <summary>
    /// 스크립트가 비활성화될 때 색상 복구
    /// </summary>
    void OnDisable()
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _originalColor;
        }
    }
}
