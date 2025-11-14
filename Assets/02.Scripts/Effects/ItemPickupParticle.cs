using UnityEngine;

public class ItemPickupParticle : MonoBehaviour
{
    [Header("Particle Settings")]
    [SerializeField] private int _particleCount = 20;
    [SerializeField] private float _particleSpeed = 3f;
    [SerializeField] private float _particleLifetime = 1f;
    [SerializeField] private float _particleSize = 0.1f;
    [SerializeField] private Gradient _particleColorGradient;
    
    private ParticleSystem _particleSystem;
    
    private void Awake()
    {
        CreateParticleSystem();
    }
    
    private void Start()
    {
        if (_particleSystem != null)
        {
            _particleSystem.Play();
        }
        
        // 파티클 시스템이 끝나면 오브젝트 삭제
        Destroy(gameObject, _particleLifetime + 0.5f);
    }
    
    private void CreateParticleSystem()
    {
        // ParticleSystem 컴포넌트 추가
        _particleSystem = gameObject.AddComponent<ParticleSystem>();
        
        // Main 모듈 설정
        var main = _particleSystem.main;
        main.duration = 0.5f;
        main.loop = false;
        main.startLifetime = _particleLifetime;
        main.startSpeed = _particleSpeed;
        main.startSize = _particleSize;
        main.startColor = new ParticleSystem.MinMaxGradient(_particleColorGradient);
        main.gravityModifier = 0.5f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        
        // Emission 모듈 설정
        var emission = _particleSystem.emission;
        emission.enabled = true;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, _particleCount) });
        
        // Shape 모듈 설정
        var shape = _particleSystem.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;
        
        // Color Over Lifetime 모듈 설정
        var colorOverLifetime = _particleSystem.colorOverLifetime;
        colorOverLifetime.enabled = true;
        
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(Color.white, 0.0f), 
                new GradientColorKey(Color.yellow, 0.5f),
                new GradientColorKey(Color.red, 1.0f) 
            },
            new GradientAlphaKey[] { 
                new GradientAlphaKey(1.0f, 0.0f), 
                new GradientAlphaKey(0.0f, 1.0f) 
            }
        );
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);
        
        // Size Over Lifetime 모듈 설정
        var sizeOverLifetime = _particleSystem.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0.0f, 1.0f);
        sizeCurve.AddKey(1.0f, 0.0f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);
        
        // Renderer 설정
        var renderer = _particleSystem.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.material = new Material(Shader.Find("Sprites/Default"));
    }
}
