using UnityEngine;

public class ItemVisualEffect : MonoBehaviour
{
    [Header("Glow Effect")]
    [SerializeField] private bool _enableGlow = true;
    [SerializeField] private float _glowPulseSpeed = 2f;
    [SerializeField] private float _glowMinScale = 0.9f;
    [SerializeField] private float _glowMaxScale = 1.2f;
    
    [Header("Sparkle Particles")]
    [SerializeField] private bool _enableSparkles = true;
    [SerializeField] private float _sparkleInterval = 0.3f;
    [SerializeField] private int _sparkleCount = 3;
    
    private SpriteRenderer _spriteRenderer;
    private ParticleSystem _sparkleParticleSystem;
    private float _nextSparkleTime;
    
    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (_enableSparkles)
        {
            CreateSparkleParticleSystem();
        }
    }
    
    private void Update()
    {
        if (_enableGlow)
        {
            UpdateGlowEffect();
        }
        
        if (_enableSparkles && Time.time >= _nextSparkleTime)
        {
            EmitSparkles();
            _nextSparkleTime = Time.time + _sparkleInterval;
        }
    }
    
    private void UpdateGlowEffect()
    {
        // 펄스 효과 (크기 변화)
        float scale = Mathf.Lerp(_glowMinScale, _glowMaxScale, 
            (Mathf.Sin(Time.time * _glowPulseSpeed) + 1f) / 2f);
        transform.localScale = Vector3.one * scale;
    }
    
    private void CreateSparkleParticleSystem()
    {
        GameObject sparkleObj = new GameObject("SparkleParticles");
        sparkleObj.transform.SetParent(transform);
        sparkleObj.transform.localPosition = Vector3.zero;
        
        _sparkleParticleSystem = sparkleObj.AddComponent<ParticleSystem>();
        
        var main = _sparkleParticleSystem.main;
        main.duration = 1f;
        main.loop = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.3f, 0.8f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.5f, 1.5f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.15f);
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(1f, 1f, 0.5f, 1f), 
            Color.white
        );
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        
        var emission = _sparkleParticleSystem.emission;
        emission.enabled = false; // 수동으로 방출
        
        var shape = _sparkleParticleSystem.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.3f;
        shape.radiusThickness = 1f;
        
        var colorOverLifetime = _sparkleParticleSystem.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(Color.yellow, 0.0f), 
                new GradientColorKey(Color.white, 1.0f) 
            },
            new GradientAlphaKey[] { 
                new GradientAlphaKey(1.0f, 0.0f), 
                new GradientAlphaKey(0.0f, 1.0f) 
            }
        );
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);
        
        var sizeOverLifetime = _sparkleParticleSystem.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve curve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, curve);
        
        var renderer = _sparkleParticleSystem.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.material = new Material(Shader.Find("Sprites/Default"));
    }
    
    private void EmitSparkles()
    {
        if (_sparkleParticleSystem != null)
        {
            _sparkleParticleSystem.Emit(_sparkleCount);
        }
    }
}
