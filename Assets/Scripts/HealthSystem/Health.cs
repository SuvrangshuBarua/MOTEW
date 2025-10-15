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
    [SerializeField] private Stats stats;
    private HealthBar healthBar;

    public int CurrentHealth { get; private set; }
    public int MaxHealth => stats != null ? stats.BaseHealth : 100;
    public bool IsDead => CurrentHealth <= 0;

    public event Action<HealthChangedArgs> OnDamaged;
    // public event Action<HealthChangedArgs> OnHealed;
    public event Action<SpawnedArgs> OnSpawned;
    public event Action<DeathArgs> OnDied;

    private void Start()
    {
        healthBar = GetComponent<HealthBar>();
        // Spawn on start
        Spawn();
    }

    public void Spawn()
    {
        CurrentHealth = MaxHealth;
        // Initialize health bar if available
        healthBar.Initialize(MaxHealth);
        OnSpawned?.Invoke(new SpawnedArgs(CurrentHealth, MaxHealth));
        Debug.Log($"[Health] Spawned {name} with {CurrentHealth}/{MaxHealth}"); // DEBUG
    }

    public void TakeDamage(int amount, object source = null)
    {
        if (IsDead || amount <= 0) return;

        int old = CurrentHealth;
        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);

        int delta = CurrentHealth - old;
        Debug.Log($"[Health] {name} took {-delta} damage (src={source}) -> {CurrentHealth}/{MaxHealth}"); // DEBUG

        OnDamaged?.Invoke(new HealthChangedArgs(CurrentHealth, MaxHealth, delta, source));

        // Update health bar if available
        healthBar.UpdateCurrentHealth(CurrentHealth);

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
        healthBar.Destroy();
        // Emit death first
        OnDied?.Invoke(new DeathArgs(source));
        // TODO: disable other inputs here on death
    }
}
