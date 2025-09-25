using UnityEngine;

public class Humon : MonoBehaviour

{
    [SerializeField] private Stats stats;

    private Health _health;

    private void Awake()
    {
        _health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        if (_health != null)
            _health.OnDied += HandleDeath;
    }

    private void OnDisable()
    {
        if (_health != null)
            _health.OnDied -= HandleDeath;
    }


    void Start()
    {

    }

    void HandleDeath(DeathArgs args)
    {
        Debug.Log($"[Humon] {name} died (source: {args.Source})"); // DEBUG
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Handle fall damage
        if (_health == null || _health.IsDead || stats == null) return;

        float impactSpeed = collision.relativeVelocity.magnitude;

        if (impactSpeed > stats.MinImpactSpeed)
        {
            float over = impactSpeed - stats.MinImpactSpeed;
            int dmg = Mathf.Clamp(Mathf.RoundToInt(over * stats.DamagePerSpeed), 1, stats.MaxFallDamage);

            Debug.Log($"[Humon] Impact {impactSpeed:F2} -> fall damage {dmg}"); // DEBUG
            _health.TakeDamage(dmg, source: "fall");
        }
    }

    void Update()
    {

        // HandleDeath(Source);
        // stuff here to handle death damage
        // Humon damage should occur when: 
        // 
        // - humon is dropped at a high height (-10 damage)
        // This should be triggered when humon is picked up
        // - if repeated, humon dies 

        // Use OnStartDrag? 
    }
}

