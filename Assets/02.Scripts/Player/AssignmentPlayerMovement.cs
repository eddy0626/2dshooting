using UnityEngine;

/// <summary>
/// 과제용 플레이어 이동 시스템
/// Q/E: 속도 조절, Shift: 가속, R: 원점 귀환
/// </summary>
public class AssignmentPlayerMovement : MonoBehaviour
{
    [Header("속도 설정")]
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float minSpeed = 2f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float speedChangeAmount = 1f;
    [SerializeField] private float shiftMultiplier = 1.2f;
    
    [Header("이동 영역")]
    [SerializeField] private Vector2 boundaryMin = new Vector2(-8f, -4f);
    [SerializeField] private Vector2 boundaryMax = new Vector2(8f, 4f);
    [SerializeField] private bool enableWrapAround = true;
    
    private float _currentSpeed;
    
    void Start()
    {
        _currentSpeed = baseSpeed;
        Debug.Log("[과제] AssignmentPlayerMovement 시작!");
    }
    
    void Update()
    {
        HandleSpeedControl();
        HandleMovement();
    }
    
    void HandleSpeedControl()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _currentSpeed = Mathf.Min(_currentSpeed + speedChangeAmount, maxSpeed);
            Debug.Log($"[속도] 증가: {_currentSpeed}");
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            _currentSpeed = Mathf.Max(_currentSpeed - speedChangeAmount, minSpeed);
            Debug.Log($"[속도] 감소: {_currentSpeed}");
        }
    }
    
    void HandleMovement()
    {
        Vector3 movement = Vector3.zero;
        
        if (Input.GetKey(KeyCode.R))
        {
            Vector3 direction = Vector3.zero - transform.position;
            if (direction.magnitude > 0.1f)
            {
                movement = direction.normalized;
            }
            else
            {
                transform.position = Vector3.zero;
                return;
            }
        }
        else
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            movement = new Vector3(h, v, 0f).normalized;
        }
        
        float finalSpeed = _currentSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            finalSpeed *= shiftMultiplier;
        }
        
        if (movement != Vector3.zero)
        {
            transform.Translate(movement * finalSpeed * Time.deltaTime, Space.World);
        }
        
        ApplyBoundary();
    }
    
    void ApplyBoundary()
    {
        Vector3 pos = transform.position;
        
        if (enableWrapAround)
        {
            if (pos.x > boundaryMax.x) pos.x = boundaryMin.x;
            else if (pos.x < boundaryMin.x) pos.x = boundaryMax.x;
            
            if (pos.y > boundaryMax.y) pos.y = boundaryMin.y;
            else if (pos.y < boundaryMin.y) pos.y = boundaryMax.y;
        }
        else
        {
            pos.x = Mathf.Clamp(pos.x, boundaryMin.x, boundaryMax.x);
            pos.y = Mathf.Clamp(pos.y, boundaryMin.y, boundaryMax.y);
        }
        
        transform.position = pos;
    }
}