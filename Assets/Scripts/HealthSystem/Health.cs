using UnityEngine;
using System;
using Microlight.MicroBar;
using Unity.Mathematics;

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
    [SerializeField] MicroBar healthBar;
    private Camera _cam;

    public int CurrentHealth { get; private set; }
    public int MaxHealth => stats != null ? stats.BaseHealth : 100;
    public bool IsDead => CurrentHealth <= 0;

    public event Action<HealthChangedArgs> OnDamaged;
    // public event Action<HealthChangedArgs> OnHealed;
    public event Action<SpawnedArgs> OnSpawned;
    public event Action<DeathArgs> OnDied;

    private void Start()
    {
        _cam = Camera.main;

        // Spawn on start 
        Spawn();
    }

    private void LateUpdate()
    {
        // Make health bar Y and Z axis 0 so that it always faces front
        if (healthBar != null)
        {
            // Keep only X rotation, reset Y and Z to 0 in world space
            healthBar.transform.rotation = Quaternion.Euler(healthBar.transform.eulerAngles.x, 0, 0);
        }
    }

    public void Spawn()
    {
        CurrentHealth = MaxHealth;
        
        // Add null check
        if (healthBar != null)
        {
            healthBar.Initialize(MaxHealth);
        }
        
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
        
        // Add null check before updating the health bar
        if (healthBar != null)
        {
            healthBar.UpdateBar(CurrentHealth);
        }

        OnDamaged?.Invoke(new HealthChangedArgs(CurrentHealth, MaxHealth, delta, source));

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
        // Add null check before destroying
        if (healthBar != null)
        {
            Destroy(healthBar.gameObject);
            healthBar = null;
        }
        
        // Emit death first
        OnDied?.Invoke(new DeathArgs(source));
        // TODO: disable other inputs here on death
    }
}
