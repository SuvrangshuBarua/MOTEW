using UnityEngine;
using System;

[DisallowMultipleComponent]
public class Health : MonoBehaviour, IHealth, IDamageable
{
    [SerializeField] private Stats stats;

    public int CurrentHealth { get; private set; }
    public int MaxHealth => stats != null ? stats.BaseHealth : 100;
    public bool IsDead => CurrentHealth <= 0;

    public event Action<HealthChangedArgs> OnDamaged;
    // public event Action<HealthChangedArgs> OnHealed;
    public event Action<SpawnedArgs> OnSpawned;
    public event Action<DeathArgs> OnDied;

    private void Start()
    {
        // Spawn on start 
        // TODO: not sure how this will be handled
        Spawn();
    }

    public void Spawn()
    {
        CurrentHealth = MaxHealth;
        OnSpawned?.Invoke(new SpawnedArgs(CurrentHealth, MaxHealth));
    }

    public void TakeDamage(int amount, object source = null)
    {
        if (IsDead || amount <= 0) return;

        int old = CurrentHealth;
        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        OnDamaged?.Invoke(new HealthChangedArgs(CurrentHealth, MaxHealth, CurrentHealth - old, source));

        if (CurrentHealth == 0)
        {
            Die(source);
        }
    }

    public void Kill(object source = null)
    {
        if (IsDead) return;
        CurrentHealth = 0;
        Die(source);
    }

    private void Die(object source)
    {
        // Emit death first
        OnDied?.Invoke(new DeathArgs(source));
        // TODO: disable other inputs here on death
    }
}
