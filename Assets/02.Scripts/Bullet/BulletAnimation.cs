using UnityEngine;

/// <summary>
/// 총알의 시각 효과를 담당합니다.
/// 회전, 스케일 펄스, 트레일 등의 이펙트를 제공합니다.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class BulletAnimation : MonoBehaviour
{
    [Header("회전 효과")]
    [SerializeField] private bool EnableRotation = true;
    [SerializeField] private float RotationSpeed = 360f; // 초당 회전 각도 (360 = 1회전/초)

    [Header("스케일 펄스 효과")]
    [SerializeField] private bool EnableScalePulse = false;
    [SerializeField] private float PulseSpeed = 3f; // 펄스 속도
    [SerializeField] private float PulseAmplitude = 0.15f; // 펄스 크기 (0.15 = ±15%)

    [Header("깜빡임 효과")]
    [SerializeField] private bool EnableBlink = false;
    [SerializeField] private float BlinkSpeed = 5f; // 깜빡임 속도
    [SerializeField] private float BlinkMin = 0.7f; // 최소 밝기
    [SerializeField] private float BlinkMax = 1.2f; // 최대 밝기

    [Header("트레일 효과")]
    [SerializeField] private TrailRenderer _trailRenderer; // 선택사항: 트레일 효과

    [Header("스프라이트 애니메이션")]
    [SerializeField] private Animator _animator; // 선택적: 스프라이트 애니메이션 재생용

    private SpriteRenderer _spriteRenderer;
    private Vector3 _originalScale;
    private Color _originalColor;

    /// <summary>
    /// 초기화
    /// </summary>
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalScale = transform.localScale;

        if (_spriteRenderer != null)
        {
            _originalColor = _spriteRenderer.color;
        }

        // 트레일 렌더러 자동 찾기
        if (_trailRenderer == null)
        {
            _trailRenderer = GetComponent<TrailRenderer>();
        }

        // Animator 자동 찾기
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
    }

    /// <summary>
    /// 매 프레임 애니메이션 효과 적용
    /// </summary>
    void Update()
    {
        // 회전 효과
        if (EnableRotation)
        {
            transform.Rotate(0f, 0f, RotationSpeed * Time.deltaTime);
        }

        // 스케일 펄스 효과
        if (EnableScalePulse)
        {
            float pulse = 1f + Mathf.Sin(Time.time * PulseSpeed) * PulseAmplitude;
            transform.localScale = _originalScale * pulse;
        }

        // 깜빡임 효과 (밝기 변화)
        if (EnableBlink && _spriteRenderer != null)
        {
            float brightness = Mathf.Lerp(BlinkMin, BlinkMax, (Mathf.Sin(Time.time * BlinkSpeed) + 1f) / 2f);
            Color color = _originalColor;
            color.r *= brightness;
            color.g *= brightness;
            color.b *= brightness;
            _spriteRenderer.color = color;
        }
    }

    /// <summary>
    /// 스크립트가 비활성화될 때 색상을 원래대로 복구
    /// </summary>
    void OnDisable()
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _originalColor;
        }
    }
}