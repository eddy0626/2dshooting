using UnityEngine;

/// <summary>
/// 아이템의 시각 효과를 담당합니다.
/// 회전, 스케일 펄스, 깜빡임, 부유 효과 등을 제공합니다.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class ItemAnimation : MonoBehaviour
{
    [Header("회전 효과")]
    [SerializeField] private bool EnableRotation = true;
    [SerializeField] private float RotationSpeed = 180f; // 초당 회전 각도

    [Header("스케일 펄스 효과")]
    [SerializeField] private bool EnableScalePulse = true;
    [SerializeField] private float PulseSpeed = 3f; // 펄스 속도
    [SerializeField] private float PulseAmplitude = 0.15f; // 펄스 크기 (±15%)

    [Header("부유 효과 (위아래 움직임)")]
    [SerializeField] private bool EnableFloating = true;
    [SerializeField] private float FloatSpeed = 2f; // 부유 속도
    [SerializeField] private float FloatAmplitude = 0.3f; // 부유 높이

    [Header("깜빡임 효과")]
    [SerializeField] private bool EnableBlink = true;
    [SerializeField] private float BlinkSpeed = 2f; // 깜빡임 속도
    [SerializeField] private float MinAlpha = 0.5f; // 최소 투명도 (0 = 완전 투명, 1 = 완전 불투명)
    [SerializeField] private float MaxAlpha = 1f; // 최대 투명도

    [Header("발광 효과")]
    [SerializeField] private bool EnableGlow = true;
    [SerializeField] private float GlowSpeed = 3f; // 발광 속도
    [SerializeField] private float GlowMin = 0.8f; // 최소 밝기
    [SerializeField] private float GlowMax = 1.3f; // 최대 밝기

    [Header("스프라이트 애니메이션")]
    [SerializeField] private Animator _animator; // 선택적: 스프라이트 애니메이션 재생용

    private SpriteRenderer _spriteRenderer;
    private Vector3 _originalScale;
    private Vector3 _startPosition;
    private Color _originalColor;
    private bool _isInitialized = false;

    /// <summary>
    /// 초기화
    /// </summary>
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalScale = transform.localScale;
        _originalColor = _spriteRenderer != null ? _spriteRenderer.color : Color.white;

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
        // 시작 위치 저장 (첫 프레임 이후)
        if (!_isInitialized && Time.frameCount > 1)
        {
            _startPosition = transform.position;
            _isInitialized = true;
        }

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

        // 부유 효과 (위아래로 움직임)
        if (EnableFloating && _isInitialized)
        {
            float yOffset = Mathf.Sin(Time.time * FloatSpeed) * FloatAmplitude;
            Vector3 newPosition = _startPosition;
            newPosition.y += yOffset;
            transform.position = newPosition;
        }

        // 스프라이트 색상 효과
        if (_spriteRenderer != null)
        {
            Color finalColor = _originalColor;

            // 발광 효과
            if (EnableGlow)
            {
                float glow = Mathf.Lerp(GlowMin, GlowMax, (Mathf.Sin(Time.time * GlowSpeed) + 1f) / 2f);
                finalColor.r *= glow;
                finalColor.g *= glow;
                finalColor.b *= glow;
            }

            // 깜빡임 효과 (투명도 변화)
            if (EnableBlink)
            {
                float alpha = Mathf.Lerp(MinAlpha, MaxAlpha, (Mathf.Sin(Time.time * BlinkSpeed) + 1f) / 2f);
                finalColor.a = alpha;
            }

            _spriteRenderer.color = finalColor;
        }
    }

    /// <summary>
    /// 스크립트가 비활성화될 때 색상과 위치를 원래대로 복구
    /// </summary>
    void OnDisable()
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _originalColor;
        }

        if (_isInitialized)
        {
            transform.position = _startPosition;
        }
    }
}