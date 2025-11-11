using System.Collections;
using UnityEngine;

/// <summary>
/// 필살기 '붐' - 화면 중앙에 생성되어 3초간 지속되며 닿는 모든 에너미를 즉사시킵니다.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class UltimateSkill : MonoBehaviour
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

    [Header("컴포넌트")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private ParticleSystem _particleSystem; // 파티클 시스템
    [SerializeField] private Collider2D _collider; // 충돌 범위

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

        if (_particleSystem == null)
            _particleSystem = GetComponent<ParticleSystem>();

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
            _initialColor = _spriteRenderer.color;

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

        // 페이드 아웃 (마지막 1초)
        if (EnableFadeOut && _spriteRenderer != null && _elapsedTime >= FadeOutStartTime)
        {
            float fadeTime = _elapsedTime - FadeOutStartTime;
            float fadeDuration = Duration - FadeOutStartTime;
            float alpha = Mathf.Lerp(_initialColor.a, 0f, fadeTime / fadeDuration);

            Color color = _initialColor;
            color.a = alpha;
            _spriteRenderer.color = color;
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
    /// 특정 위치에 필살기를 생성합니다. (정적 헬퍼 메서드)
    /// </summary>
    /// <param name="ultimatePrefab">필살기 프리팹</param>
    /// <param name="position">생성 위치</param>
    /// <returns>생성된 필살기 GameObject</returns>
    public static GameObject CreateUltimate(GameObject ultimatePrefab, Vector3 position)
    {
        if (ultimatePrefab == null)
        {
            Debug.LogWarning("UltimateSkill: 필살기 프리팹이 null입니다!");
            return null;
        }

        GameObject ultimate = Instantiate(ultimatePrefab, position, Quaternion.identity);
        return ultimate;
    }
}