using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Collider))]
public class FallDamage : MonoBehaviour
{

    [Header("Data")]
    [SerializeField] private HumonStats humonStats;
    [SerializeField] private LayerMask groundMask = ~0; // for Terrain

    public System.Action OnLand;
    Rigidbody _rigidbody;
    UnityEngine.AI.NavMeshAgent _agent;
    IDamageable _damageable;

    public HumonStats HumonStatsAsset { get => humonStats; set => humonStats = value; }

    bool _isAirborne;
    float _lastLandingTime;
    float _maxYHeight; // track apex
    const float _LandingCooldown = 0.15f;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _damageable = GetComponent<IDamageable>();
    }

    public void BeginAirborne()
    {
        _isAirborne = true;
        _maxYHeight = transform.position.y; // start tracking from current Y

        if (_agent && _agent.enabled) _agent.enabled = false;

        //_rigidbody.isKinematic = false; 
        //_rigidbody.useGravity = true;
        _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        //_rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_isAirborne) return;

        if ((groundMask.value & (1 << collision.gameObject.layer)) == 0) return;
        if (Time.time - _lastLandingTime < _LandingCooldown) return;
        _lastLandingTime = Time.time;

        // Get lowest contact point Y as ground level
        float groundY = collision.GetContact(0).point.y;

        for (int i = 1; i < collision.contactCount; i++)
            groundY = Mathf.Min(groundY, collision.GetContact(i).point.y);

        float fallHeight = Mathf.Max(0f, _maxYHeight - groundY);

        // Calculate fall damage
        // damage = round( max(0, (fallHeight - minFallHeight) * damagePerMeter ) )
        int damageAmount = 0;
        if (humonStats && fallHeight >= humonStats.MinFallHeight)
        {
            float excessHeight = fallHeight - humonStats.MinFallHeight;
            damageAmount = Mathf.Clamp(Mathf.RoundToInt(excessHeight * humonStats.DamagePerMeter), 1, humonStats.MaxFallDamage);
        }

        // Apply calculated damage
        if (damageAmount > 0 && _damageable != null)
            _damageable.TakeDamage(damageAmount, source: "fall");

        StartCoroutine(CompleteFall());
    }

    System.Collections.IEnumerator CompleteFall()
    {
        yield return new WaitForFixedUpdate();

        //_rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        //_rigidbody.isKinematic = true;
        //_rigidbody.useGravity = false;

        if (_agent)
        {
            if (UnityEngine.AI.NavMesh.SamplePosition(transform.position, out var hit, 2f, UnityEngine.AI.NavMesh.AllAreas))
                _agent.Warp(hit.position);
            _agent.enabled = true;
        }

        _isAirborne = false;
        OnLand?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isAirborne)
            _maxYHeight = Mathf.Max(_maxYHeight, transform.position.y); // track apex
    }
}
