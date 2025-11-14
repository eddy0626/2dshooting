using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float _baseSpeed = 5f;
    [SerializeField] private float _minSpeed = 2f;
    [SerializeField] private float _maxSpeed = 10f;
    [SerializeField] private float _shiftSpeedMultiplier = 1.2f;
    [SerializeField] private float _returnToOriginSpeed = 8f;
    
    [Header("이동 영역 제한")]
    [SerializeField] private float _minX = -8f;
    [SerializeField] private float _maxX = 8f;
    [SerializeField] private float _minY = -4f;
    [SerializeField] private float _maxY = 4f;
    
    private float _currentSpeed;
    private Vector3 _origin = Vector3.zero;
    
    private void Start()
    {
        _currentSpeed = _baseSpeed;
        _origin = Vector3.zero;
    }
    
    private void Update()
    {
        HandleSpeedControl();
        
        if (Input.GetKey(KeyCode.R))
        {
            ReturnToOrigin();
        }
        else
        {
            HandleMovement();
        }
    }
    
    private void HandleSpeedControl()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _currentSpeed = Mathf.Min(_currentSpeed + 1f, _maxSpeed);
            Debug.Log($"Speed Up: {_currentSpeed}");
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            _currentSpeed = Mathf.Max(_currentSpeed - 1f, _minSpeed);
            Debug.Log($"Speed Down: {_currentSpeed}");
        }
    }
    
    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        Vector3 direction = new Vector3(horizontal, vertical, 0f).normalized;
        
        if (direction.magnitude > 0)
        {
            float speed = _currentSpeed;
            
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                speed *= _shiftSpeedMultiplier;
            }
            
            transform.Translate(direction * speed * Time.deltaTime);
            ClampAndWrapPosition();
        }
    }
    
    private void ReturnToOrigin()
    {
        Vector3 direction = (_origin - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, _origin);
        
        if (distance > 0.1f)
        {
            transform.Translate(direction * _returnToOriginSpeed * Time.deltaTime, Space.World);
        }
        else
        {
            transform.position = _origin;
        }
    }
    
    private void ClampAndWrapPosition()
    {
        Vector3 pos = transform.position;
        
        if (pos.x < _minX) pos.x = _maxX;
        else if (pos.x > _maxX) pos.x = _minX;
        
        if (pos.y < _minY) pos.y = _maxY;
        else if (pos.y > _maxY) pos.y = _minY;
        
        transform.position = pos;
    }
    
    public float GetCurrentSpeed() => _currentSpeed;
    public void SetSpeed(float speed) => _currentSpeed = Mathf.Clamp(speed, _minSpeed, _maxSpeed);
}