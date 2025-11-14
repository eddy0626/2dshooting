using UnityEngine;

public class BulletSystem : MonoBehaviour
{
    [SerializeField] private GameObject _mainBulletPrefab;
    [SerializeField] private GameObject _subBulletPrefab;
    [SerializeField] private float _fireRate = 0.6f;
    [SerializeField] private float _mainBulletDamage = 60f;
    [SerializeField] private float _subBulletDamage = 40f;
    [SerializeField] private bool _isAutoMode = true;
    
    private float _nextFireTime = 0f;
    private Transform _centerFirePoint;
    private Transform _leftFirePoint;
    private Transform _rightFirePoint;
    
    private void Start()
    {
        CreateFirePoints();
    }
    
    private void CreateFirePoints()
    {
        _centerFirePoint = new GameObject("CenterFirePoint").transform;
        _centerFirePoint.SetParent(transform);
        _centerFirePoint.localPosition = new Vector3(0f, 0.5f, 0f);
        
        _leftFirePoint = new GameObject("LeftFirePoint").transform;
        _leftFirePoint.SetParent(transform);
        _leftFirePoint.localPosition = new Vector3(-0.3f, 0.3f, 0f);
        
        _rightFirePoint = new GameObject("RightFirePoint").transform;
        _rightFirePoint.SetParent(transform);
        _rightFirePoint.localPosition = new Vector3(0.3f, 0.3f, 0f);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _isAutoMode = true;
            Debug.Log("Fire Mode: Auto");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _isAutoMode = false;
            Debug.Log("Fire Mode: Manual");
        }
        
        if (Time.time < _nextFireTime) return;
        
        bool shouldFire = _isAutoMode || Input.GetKey(KeyCode.Space);
        
        if (shouldFire)
        {
            Fire();
            _nextFireTime = Time.time + _fireRate;
        }
    }
    
    private void Fire()
    {
        if (_mainBulletPrefab == null)
        {
            Debug.LogWarning("Main Bullet Prefab not set!");
            return;
        }
        
        FireMainBullet(_centerFirePoint.position + Vector3.left * 0.15f);
        FireMainBullet(_centerFirePoint.position + Vector3.right * 0.15f);
        
        if (_subBulletPrefab != null)
        {
            FireSubBullet(_leftFirePoint.position, Vector3.up + Vector3.left * 0.2f);
            FireSubBullet(_rightFirePoint.position, Vector3.up + Vector3.right * 0.2f);
        }
    }
    
    private void FireMainBullet(Vector3 position)
    {
        GameObject bullet = Instantiate(_mainBulletPrefab, position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
            bulletScript.Initialize(_mainBulletDamage, Vector3.up, BulletType.Main);
    }
    
    private void FireSubBullet(Vector3 position, Vector3 direction)
    {
        GameObject bullet = Instantiate(_subBulletPrefab, position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
            bulletScript.Initialize(_subBulletDamage, direction.normalized, BulletType.Sub);
    }
    
    public void IncreaseFireRate(float amount)
    {
        _fireRate = Mathf.Max(0.1f, _fireRate - amount);
        Debug.Log($"Fire Rate: {_fireRate}");
    }
}

public enum BulletType { Main, Sub, SCurve, Spiral }