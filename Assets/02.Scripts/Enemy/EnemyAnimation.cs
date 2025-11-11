using UnityEngine;

/// <summary>
/// 에너미의 시각 효과를 담당합니다.
/// 흔들림, 사인파 이동, 페이드 인, 회전 등의 이펙트를 제공합니다.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyAnimation : MonoBehaviour
{
    [Header("좌우 흔들림 효과")]
    [SerializeField] private bool EnableWobble = false;
    [SerializeField] private float WobbleSpeed = 2f; // 흔들림 속도
    [SerializeField] private float WobbleAmplitude = 0.3f; // 흔들림 크기 (유닛)

    [Header("사인파 이동 효과")]
    [SerializeField] private bool EnableSineWave = false;
    [SerializeField] private float SineWaveSpeed = 1f; // 사인파 속도
    [SerializeField] private float SineWaveAmplitude = 1f; // 사인파 진폭

    [Header("스폰 페이드 인 효과")]
    [SerializeField] private bool EnableSpawnFade = true;
    [SerializeField] private float FadeInDuration = 0.5f; // 페이드 인 지속 시간

    [Header("회전 효과")]
    [SerializeField] private bool EnableRotation = false;
    [SerializeField] private float RotationSpeed = 90f; // 초당 회전 각도

    [Header("스케일 펄스")]
    [SerializeField] private bool EnableScalePulse = false;
    [SerializeField] private float PulseSpeed = 2f; // 펄스 속도
    [SerializeField] private float PulseAmplitude = 0.1f; // 펄스 크기

    [Header("발광 효과")]
    [SerializeField] private bool EnableGlow = false;
    [SerializeField] private float GlowSpeed = 3f; // 발광 속도
    [SerializeField] private float GlowMin = 0.9f; // 최소 밝기
    [SerializeField] private float GlowMax = 1.1f; // 최대 밝기

    [Header("스프라이트 애니메이션")]
    [SerializeField] private Animator _animator; // 선택적: 스프라이트 애니메이션 재생용

    private SpriteRenderer _spriteRenderer;
    private Vector3 _startPosition;
    private Vector3 _originalScale;
    private Color _originalColor;
    private float _elapsedTime = 0f;
    private bool _isInitialized = false;
    private bool _fadeInComplete = false;

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

            // 스폰 페이드 인 효과
            if (EnableSpawnFade)
            {
                Color fadeColor = _originalColor;
                fadeColor.a = 0f; // 완전 투명 상태로 시작
                _spriteRenderer.color = fadeColor;
            }
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
        // 시작 위치를 첫 프레임 이후에 저장 (Enemy 이동 스크립트 초기화 대기)
        if (!_isInitialized && Time.frameCount > 1)
        {
            _startPosition = transform.position;
            _isInitialized = true;
        }

        _elapsedTime += Time.deltaTime;

        // 페이드 인 효과 처리
        if (EnableSpawnFade && !_fadeInComplete && _spriteRenderer != null)
        {
            float alpha = Mathf.Clamp01(_elapsedTime / FadeInDuration);
            Color color = _originalColor;
            color.a = Mathf.Lerp(0f, _originalColor.a, alpha);
            _spriteRenderer.color = color;

            if (_elapsedTime >= FadeInDuration)
            {
                _fadeInComplete = true;
                _spriteRenderer.color = _originalColor;
            }
            return; // 페이드 인 중에는 다른 효과 안 함
        }

        if (!_isInitialized) return;

        // 좌우 흔들림 효과
        if (EnableWobble)
        {
            float wobbleOffset = Mathf.Sin(_elapsedTime * WobbleSpeed) * WobbleAmplitude;
            Vector3 currentPosition = transform.position;
            transform.position = new Vector3(
                _startPosition.x + wobbleOffset,
                currentPosition.y,
                currentPosition.z
            );
            _startPosition = new Vector3(_startPosition.x, currentPosition.y, _startPosition.z);
        }

        // 사인파 이동 효과
        if (EnableSineWave)
        {
            float sineOffset = Mathf.Sin(_elapsedTime * SineWaveSpeed) * SineWaveAmplitude;
            Vector3 currentPosition = transform.position;
            transform.position = new Vector3(
                currentPosition.x + sineOffset * Time.deltaTime,
                currentPosition.y,
                currentPosition.z
            );
        }

        // 회전 효과
        if (EnableRotation)
        {
            transform.Rotate(0f, 0f, RotationSpeed * Time.deltaTime);
        }

        // 스케일 펄스
        if (EnableScalePulse)
        {
            float pulse = 1f + Mathf.Sin(_elapsedTime * PulseSpeed) * PulseAmplitude;
            transform.localScale = _originalScale * pulse;
        }

        // 발광 효과
        if (EnableGlow && _spriteRenderer != null && _fadeInComplete)
        {
            float glow = Mathf.Lerp(GlowMin, GlowMax, (Mathf.Sin(_elapsedTime * GlowSpeed) + 1f) / 2f);
            Color color = _originalColor;
            color.r *= glow;
            color.g *= glow;
            color.b *= glow;
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