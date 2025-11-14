using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float _defaultDuration = 0.3f;
    [SerializeField] private float _defaultIntensity = 0.2f;
    
    private Vector3 _originalPosition;
    private float _shakeTimer = 0f;
    private float _shakeDuration = 0f;
    private float _shakeIntensity = 0f;
    private bool _isShaking = false;
    
    private void Start()
    {
        _originalPosition = transform.localPosition;
    }
    
    private void Update()
    {
        if (_isShaking)
        {
            _shakeTimer += Time.deltaTime;
            
            if (_shakeTimer < _shakeDuration)
            {
                Vector3 offset = Random.insideUnitSphere * _shakeIntensity;
                offset.z = 0f;
                transform.localPosition = _originalPosition + offset;
            }
            else
            {
                _isShaking = false;
                transform.localPosition = _originalPosition;
            }
        }
    }
    
    public void Shake() => Shake(_defaultDuration, _defaultIntensity);
    
    public void Shake(float duration, float intensity)
    {
        _originalPosition = transform.localPosition;
        _shakeDuration = duration;
        _shakeIntensity = intensity;
        _shakeTimer = 0f;
        _isShaking = true;
    }
}