using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Collider))]
public class HouseDamage : MonoBehaviour
{

    // Script should handle damage for houses when they are hit by God or with a hammer
    // if damage is taken, second "build type" should be applied. Maybe animation too?
    // Note: houses should have stats, maybe add a house
    // Call this script in BaseBuilding.cs

    [Header("Data")]
    [SerializeField] private Stats stats;
    [SerializeField] private LayerMask groundMask = ~0; // for Terrain

    public System.Action OnLand;
    Rigidbody _rigidbody;
    UnityEngine.AI.NavMeshAgent _agent;
    IDamageable _damageable;

    public Stats StatsAsset { get => stats; set => stats = value; }

    float _lastLandingTime;
    float _maxYHeight; // track apex
    const float _LandingCooldown = 0.15f;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _damageable = GetComponent<IDamageable>();
    }

    private void OnCollisionEnter(Collision collision)
    {

        if ((groundMask.value & (1 << collision.gameObject.layer)) == 0) return;
        if (Time.time - _lastLandingTime < _LandingCooldown) return;
        _lastLandingTime = Time.time;

        // Get lowest contact point Y as ground level
        float groundY = collision.GetContact(0).point.y;

        for (int i = 1; i < collision.contactCount; i++)
            groundY = Mathf.Min(groundY, collision.GetContact(i).point.y);

        float fallHeight = Mathf.Max(0f, _maxYHeight - groundY);

        // Calculate house damage
        int damageAmount = 0;
        if (stats && fallHeight >= stats.MinFallHeight)
        {
            float excessHeight = fallHeight - stats.MinFallHeight;
            damageAmount = Mathf.Clamp(Mathf.RoundToInt(excessHeight * stats.DamagePerMeter), 1, stats.MaxFallDamage);
        }

        // Apply calculated damage
        if (damageAmount > 0 && _damageable != null)
            _damageable.TakeDamage(damageAmount, source: "fall");

        StartCoroutine(CompleteFall());
    }

    System.Collections.IEnumerator CompleteFall()
    {
        yield return new WaitForFixedUpdate();

        _rigidbody.angularVelocity = Vector3.zero;

        if (_agent)
        {
            if (UnityEngine.AI.NavMesh.SamplePosition(transform.position, out var hit, 2f, UnityEngine.AI.NavMesh.AllAreas))
                _agent.Warp(hit.position);
            _agent.enabled = true;
        }

        OnLand?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {

    }

}
