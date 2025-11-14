using UnityEngine;

/// <summary>
/// 오브젝트를 회전시키는 간단한 스크립트
/// </summary>
public class SimpleRotate : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 360f;
    
    private void Update()
    {
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }
    
    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = speed;
    }
}
