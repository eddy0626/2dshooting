using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _highScoreText;
    
    [Header("애니메이션 설정")]
    [SerializeField] private float _scoreAnimationDuration = 0.3f;
    [SerializeField] private float _scoreAnimationScale = 1.2f;
    
    private int _currentScore = 0;
    private int _highScore = 0;
    private const string HIGH_SCORE_KEY = "HighScore";
    
    private float _animationTimer = 0f;
    private bool _isAnimating = false;
    private Vector3 _originalScale;
    
    private void Start()
    {
        _highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
        
        if (_scoreText != null)
        {
            _originalScale = _scoreText.transform.localScale;
            UpdateScoreUI();
        }
        
        if (_highScoreText != null)
        {
            UpdateHighScoreUI();
        }
    }
    
    private void Update()
    {
        if (_isAnimating && _scoreText != null)
        {
            _animationTimer += Time.deltaTime;
            float progress = _animationTimer / _scoreAnimationDuration;
            
            if (progress >= 1f)
            {
                _isAnimating = false;
                _scoreText.transform.localScale = _originalScale;
            }
            else
            {
                float scale = progress < 0.5f 
                    ? Mathf.Lerp(1f, _scoreAnimationScale, progress * 2f)
                    : Mathf.Lerp(_scoreAnimationScale, 1f, (progress - 0.5f) * 2f);
                
                _scoreText.transform.localScale = _originalScale * scale;
            }
        }
    }
    
    public void AddScore(int amount)
    {
        _currentScore += amount;
        UpdateScoreUI();
        PlayScoreAnimation();
        
        if (_currentScore > _highScore)
        {
            _highScore = _currentScore;
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, _highScore);
            PlayerPrefs.Save();
            UpdateHighScoreUI();
        }
    }
    
    public void ResetScore()
    {
        _currentScore = 0;
        UpdateScoreUI();
    }
    
    private void UpdateScoreUI()
    {
        if (_scoreText != null)
        {
            _scoreText.text = $"Score: {_currentScore}";
        }
    }
    
    private void UpdateHighScoreUI()
    {
        if (_highScoreText != null)
        {
            _highScoreText.text = $"Best: {_highScore}";
        }
    }
    
    private void PlayScoreAnimation()
    {
        if (_scoreText == null) return;
        
        _isAnimating = true;
        _animationTimer = 0f;
    }
    
    public int GetCurrentScore() => _currentScore;
    public int GetHighScore() => _highScore;
}