using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Item Settings")]
    [SerializeField] private ItemType _itemType = ItemType.Health;
    [SerializeField] private float _value = 1f;
    
    [Header("Movement Settings")]
    [SerializeField] private float _attractDelay = 2f;
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _bezierHeight = 2f; // 베지어 곡선 높이
    [SerializeField] private AnimationCurve _moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Visual Effects")]
    [SerializeField] private float _rotationSpeed = 180f;
    [SerializeField] private float _bobSpeed = 2f;
    [SerializeField] private float _bobHeight = 0.2f;
    [SerializeField] private GameObject _pickupParticlePrefab;
    
    private Transform _playerTransform;
    private bool _isAttracting = false;
    private float _spawnTime;
    private Vector3 _startPosition;
    private Vector3 _initialPosition;
    private float _moveProgress = 0f;
    
    private void Start()
    {
        _spawnTime = Time.time;
        _startPosition = transform.position;
        _initialPosition = transform.position;
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            _playerTransform = player.transform;
    }
    
    private void Update()
    {
        // 끌어당기기 시작
        if (!_isAttracting && Time.time - _spawnTime >= _attractDelay)
        {
            _isAttracting = true;
            _startPosition = transform.position;
            _moveProgress = 0f;
        }
        
        if (_isAttracting && _playerTransform != null)
        {
            MoveToBezierCurve();
        }
        else
        {
            // 대기 중 둥둥 떠다니는 효과
            IdleFloat();
        }
        
        // 회전 애니메이션
        transform.Rotate(Vector3.forward, _rotationSpeed * Time.deltaTime);
    }
    
    private void MoveToBezierCurve()
    {
        _moveProgress += Time.deltaTime * _moveSpeed;
        
        if (_moveProgress >= 1f)
        {
            ApplyEffect();
            return;
        }
        
        // 베지어 곡선 계산
        Vector3 targetPosition = _playerTransform.position;
        float curveValue = _moveCurve.Evaluate(_moveProgress);
        
        // 중간 제어점 계산 (위쪽으로 아치형)
        Vector3 midPoint = (_startPosition + targetPosition) / 2f;
        midPoint.y += _bezierHeight;
        
        // 3차 베지어 곡선
        Vector3 p0 = _startPosition;
        Vector3 p1 = midPoint;
        Vector3 p2 = targetPosition;
        
        // 2차 베지어 곡선 공식: B(t) = (1-t)²P0 + 2(1-t)tP1 + t²P2
        float t = curveValue;
        float oneMinusT = 1f - t;
        
        transform.position = (oneMinusT * oneMinusT * p0) + 
                           (2f * oneMinusT * t * p1) + 
                           (t * t * p2);
    }
    
    private void IdleFloat()
    {
        // 둥둥 떠다니는 효과
        float bobOffset = Mathf.Sin(Time.time * _bobSpeed) * _bobHeight;
        transform.position = _initialPosition + Vector3.up * bobOffset;
    }
    
    private void ApplyEffect()
    {
        GameObject player = _playerTransform?.gameObject;
        if (player == null) return;
        
        // 파티클 효과 생성
        if (_pickupParticlePrefab != null)
        {
            Instantiate(_pickupParticlePrefab, transform.position, Quaternion.identity);
        }
        
        switch (_itemType)
        {
            case ItemType.Health:
                player.GetComponent<PlayerHealth>()?.Heal((int)_value);
                break;
            case ItemType.Speed:
                player.GetComponent<PlayerController>()?.SetSpeed(
                    player.GetComponent<PlayerController>().GetCurrentSpeed() + _value);
                break;
            case ItemType.AttackSpeed:
                player.GetComponent<BulletSystem>()?.IncreaseFireRate(_value);
                break;
        }
        
        Debug.Log($"아이템 획득: {_itemType} (+{_value})");
        Destroy(gameObject);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            ApplyEffect();
    }
}

public enum ItemType { Health, Speed, AttackSpeed }
