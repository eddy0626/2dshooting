using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 3;
    private int _currentHealth;
    private float _invincibilityTimer = 0f;
    private float _invincibilityDuration = 1f;
    
    private void Start()
    {
        _currentHealth = _maxHealth;
    }
    
    private void Update()
    {
        if (_invincibilityTimer > 0)
            _invincibilityTimer -= Time.deltaTime;
    }
    
    public void TakeDamage(int damage)
    {
        if (_invincibilityTimer > 0) return;
        
        _currentHealth -= damage;
        _invincibilityTimer = _invincibilityDuration;
        
        Debug.Log($"Player Hit! Health: {_currentHealth}/{_maxHealth}");
        
        if (_currentHealth <= 0)
        {
            Debug.Log("Player Died!");
            gameObject.SetActive(false);
        }
    }
    
    public void Heal(int amount)
    {
        _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
        Debug.Log($"Player Healed! Health: {_currentHealth}/{_maxHealth}");
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
            TakeDamage(1);
    }
}