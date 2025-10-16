using UnityEngine;
using System;

/* 
* HEALTH SYSTEM
* 
* Simple per-object component health system:
* - Tracks current and max health
* - Applies damage and emits events
*/

[DisallowMultipleComponent]
public class Health : MonoBehaviour, IHealth, IDamageable
{
    [SerializeField] private HumonStats humonStats;
    [SerializeField] private BuildingStats buildingStats;

    public int CurrentHealth { get; private set; }
    public int HumonMaxHealth => humonStats != null ? humonStats.BaseHealth : 100;
    public int BuildingMaxHealth => buildingStats != null ? buildingStats.BuildingBaseHealth : 100;
    public bool IsDead => CurrentHealth <= 0;

    public event Action<HealthChangedArgs> OnDamaged;
    // public event Action<HealthChangedArgs> OnHealed;
    public event Action<SpawnedArgs> OnSpawned;
    public event Action<DeathArgs> OnDied;

    private Building.BaseBuilding _building;
    private bool _isBuilding;

    private void Start()
    {
        // Spawn on start 
        Spawn();
    }

    private void Awake()
    {
        _building = GetComponent<Building.BaseBuilding>();
        _isBuilding = buildingStats != null;
    }

    public void Spawn()
    {
        // Set either humon or building health
        int maxHealth;
        if (buildingStats != null)
        {
            CurrentHealth = BuildingMaxHealth;
            maxHealth = BuildingMaxHealth;
            Debug.Log($"[Health] Spawned building {name} with {CurrentHealth}/{BuildingMaxHealth}"); // DEBUG
        }
        else
        {
            CurrentHealth = HumonMaxHealth;
            maxHealth = HumonMaxHealth;
            Debug.Log($"[Health] Spawned {name} with {CurrentHealth}/{HumonMaxHealth}"); // DEBUG
        }

        OnSpawned?.Invoke(new SpawnedArgs(CurrentHealth, maxHealth));
    }

    public void TakeDamage(int amount, object source = null)
    {
        // Only take damage when building is fully constructed
        if (_isBuilding && _building != null && !_building.State.IsConstructed)
        {
            Debug.Log($"[Health] Building {name} is under construction! ");
            return;
        }

        if (IsDead || amount <= 0) return;

        int old = CurrentHealth;
        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);

        int delta = CurrentHealth - old;
        int maxHealth = buildingStats != null ? BuildingMaxHealth : HumonMaxHealth;

        Debug.Log($"[Health] {name} took {-delta} damage (src={source}) -> {CurrentHealth}/{maxHealth}"); // DEBUG

        OnDamaged?.Invoke(new HealthChangedArgs(CurrentHealth, maxHealth, delta, source));

        if (CurrentHealth == 0)
        {
            Die(source);
        }
    }

    // Method for instant kill
    /* public void Kill(object source = null)
    {
        if (IsDead) return;
        CurrentHealth = 0;
        Die(source);
    }*/

    private void Die(object source)
    {
        // Emit death first
        OnDied?.Invoke(new DeathArgs(source));
        // TODO: disable other inputs here on death
    }
}
