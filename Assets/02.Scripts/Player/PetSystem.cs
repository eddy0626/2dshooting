using UnityEngine;
using System.Collections.Generic;

public class PetSystem : MonoBehaviour
{
    [Header("펫 설정")]
    [SerializeField] private GameObject _petPrefab;
    [SerializeField] private int _maxPets = 3;
    [SerializeField] private float _orbitRadius = 1.5f;
    [SerializeField] private float _followSpeed = 8f;
    
    [Header("공격 설정")]
    [SerializeField] private GameObject _petBulletPrefab;
    [SerializeField] private float _attackCooldown = 3f;
    [SerializeField] private float _petBulletDamage = 30f;
    
    private List<Pet> _pets = new List<Pet>();
    private float[] _nextAttackTimes;
    
    private void Start()
    {
        _nextAttackTimes = new float[_maxPets];
        
        for (int i = 0; i < _maxPets; i++)
        {
            CreatePet(i);
        }
    }
    
    private void Update()
    {
        UpdatePetPositions();
        UpdatePetAttacks();
    }
    
    private void CreatePet(int index)
    {
        if (_petPrefab == null)
        {
            Debug.LogWarning("Pet Prefab이 설정되지 않았습니다!");
            return;
        }
        
        GameObject petObj = Instantiate(_petPrefab, transform.position, Quaternion.identity);
        petObj.name = $"Pet_{index}";
        
        Pet pet = petObj.GetComponent<Pet>();
        if (pet == null)
        {
            pet = petObj.AddComponent<Pet>();
        }
        
        pet.Initialize(index, _maxPets);
        _pets.Add(pet);
        
        _nextAttackTimes[index] = Time.time + Random.Range(0f, _attackCooldown);
    }
    
    private void UpdatePetPositions()
    {
        for (int i = 0; i < _pets.Count; i++)
        {
            if (_pets[i] == null) continue;
            
            float angle = (360f / _maxPets) * i * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(
                Mathf.Cos(angle + Time.time * 0.5f) * _orbitRadius,
                Mathf.Sin(angle + Time.time * 0.5f) * _orbitRadius,
                0f
            );
            
            Vector3 targetPosition = transform.position + offset;
            
            _pets[i].transform.position = Vector3.Lerp(
                _pets[i].transform.position,
                targetPosition,
                _followSpeed * Time.deltaTime
            );
        }
    }
    
    private void UpdatePetAttacks()
    {
        if (_petBulletPrefab == null) return;
        
        for (int i = 0; i < _pets.Count; i++)
        {
            if (_pets[i] == null) continue;
            
            if (Time.time >= _nextAttackTimes[i])
            {
                PetAttack(_pets[i].transform);
                _nextAttackTimes[i] = Time.time + _attackCooldown + Random.Range(-0.5f, 0.5f);
            }
        }
    }
    
    private void PetAttack(Transform petTransform)
    {
        GameObject bullet = Instantiate(_petBulletPrefab, petTransform.position, Quaternion.identity);
        
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Initialize(_petBulletDamage, Vector3.up, BulletType.Sub);
        }
        else
        {
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.up * 8f;
            }
        }
    }
}

public class Pet : MonoBehaviour
{
    private int _index;
    private int _totalPets;
    
    public void Initialize(int index, int totalPets)
    {
        _index = index;
        _totalPets = totalPets;
    }
}