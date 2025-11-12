using UnityEngine;
using TMPro;

/// <summary>
/// 게임의 점수를 관리하고 UI에 표시합니다.
/// 싱글톤 패턴을 사용하여 어디서든 접근 가능합니다.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("점수 설정")]
    [SerializeField] private int ScorePerEnemy = 100; // 적 처치당 획득 점수

    [Header("UI 설정")]
    [SerializeField] private TextMeshProUGUI ScoreText; // TextMeshPro 텍스트 컴포넌트

    private int _currentScore = 0;

    /// <summary>
    /// 싱글톤 인스턴스를 초기화합니다.
    /// </summary>
    void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("ScoreManager: 인스턴스가 성공적으로 초기화되었습니다.");
        }
        else
        {
            Debug.LogWarning("ScoreManager: 이미 인스턴스가 존재합니다. 중복된 오브젝트를 파괴합니다.");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 초기 점수를 UI에 표시합니다.
    /// </summary>
    void Start()
    {
        // ScoreText가 할당되지 않았다면 자동으로 찾기
        if (ScoreText == null)
        {
            // 방법 1: 이름으로 찾기
            GameObject scoreTextObj = GameObject.Find("ScoreText");
            if (scoreTextObj != null)
            {
                ScoreText = scoreTextObj.GetComponent<TextMeshProUGUI>();
                Debug.Log("ScoreManager: ScoreText를 이름으로 찾았습니다.");
            }
            
            // 방법 2: 찾지 못했다면 Scene에서 모든 TextMeshProUGUI 검색
            if (ScoreText == null)
            {
                TextMeshProUGUI[] allTexts = FindObjectsOfType<TextMeshProUGUI>();
                foreach (var text in allTexts)
                {
                    if (text.name.Contains("Score"))
                    {
                        ScoreText = text;
                        Debug.Log($"ScoreManager: '{text.name}'을(를) ScoreText로 사용합니다.");
                        break;
                    }
                }
            }
            
            // 여전히 못 찾았다면 에러
            if (ScoreText == null)
            {
                Debug.LogError("ScoreManager: ScoreText를 찾을 수 없습니다! Canvas에 TextMeshProUGUI 컴포넌트를 추가하세요.");
            }
        }
        
        UpdateScoreUI();
    }

    /// <summary>
    /// 점수를 추가하고 UI를 업데이트합니다.
    /// </summary>
    /// <param name="score">추가할 점수 (기본값: ScorePerEnemy)</param>
    public void AddScore(int score = -1)
    {
        // score가 -1이면 기본 점수 사용
        if (score == -1)
        {
            score = ScorePerEnemy;
        }

        _currentScore += score;
        Debug.Log($"ScoreManager: 점수 추가 +{score}, 현재 점수: {_currentScore}");
        UpdateScoreUI();
    }

    /// <summary>
    /// 현재 점수를 반환합니다.
    /// </summary>
    public int GetScore()
    {
        return _currentScore;
    }

    /// <summary>
    /// 점수를 초기화합니다.
    /// </summary>
    public void ResetScore()
    {
        _currentScore = 0;
        UpdateScoreUI();
        Debug.Log("ScoreManager: 점수가 초기화되었습니다.");
    }

    /// <summary>
    /// UI에 현재 점수를 표시합니다.
    /// </summary>
    private void UpdateScoreUI()
    {
        if (ScoreText != null)
        {
            ScoreText.text = $"Score: {_currentScore:N0}"; // 천 단위 콤마 포함
        }
        else
        {
            Debug.LogWarning("ScoreManager: ScoreText가 할당되지 않았습니다. Inspector에서 할당해주세요.");
        }
    }
}