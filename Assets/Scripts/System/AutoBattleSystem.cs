using UnityEngine;

public class AutoBattleSystem : MonoBehaviour
{
    [SerializeField] private bool _isAutoBattleEnabled = false;
    [SerializeField] private float _detectionRange = 10f;
    [SerializeField] private float _keepDistance = 5f;
    [SerializeField] private float _moveSpeed = 4f;
    [SerializeField] private float _minX = -8f;
    [SerializeField] private float _maxX = 8f;
    [SerializeField] private float _minY = -4f;
    [SerializeField] private float _maxY = 4f;
    
    private Transform _targetEnemy;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _isAutoBattleEnabled = !_isAutoBattleEnabled;
            Debug.Log($"Auto Battle: {(_isAutoBattleEnabled ? "ON" : "OFF")}");
        }
        
        if (_isAutoBattleEnabled)
            AutoBattle();
    }
    
    private void AutoBattle()
    {
        FindTarget();
        
        if (_targetEnemy != null)
            SmartMove();
        else
            MoveToCenter();
    }
    
    private void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            _targetEnemy = null;
            return;
        }
        
        float closestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;
        
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance && distance <= _detectionRange)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }
        
        _targetEnemy = closestEnemy?.transform;
    }
    
    private void SmartMove()
    {
        if (_targetEnemy == null) return;
        
        float distanceToTarget = Vector2.Distance(transform.position, _targetEnemy.position);
        Vector3 direction = (_targetEnemy.position - transform.position).normalized;
        
        if (distanceToTarget < _keepDistance * 0.8f)
            MoveInDirection(-direction);
        else if (distanceToTarget > _keepDistance * 1.2f)
            MoveInDirection(direction);
        else
        {
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.forward);
            float dodgeDirection = Mathf.Sin(Time.time * 2f);
            MoveInDirection(perpendicular * dodgeDirection);
        }
    }
    
    private void MoveInDirection(Vector3 direction)
    {
        Vector3 newPosition = transform.position + direction * _moveSpeed * Time.deltaTime;
        newPosition.x = Mathf.Clamp(newPosition.x, _minX, _maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, _minY, _maxY);
        transform.position = newPosition;
    }
    
    private void MoveToCenter()
    {
        Vector3 center = new Vector3(0f, -2f, 0f);
        Vector3 direction = (center - transform.position).normalized;
        
        if (Vector2.Distance(transform.position, center) > 0.5f)
            MoveInDirection(direction);
    }
}