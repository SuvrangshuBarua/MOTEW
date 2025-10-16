/* 
* HEALTH INTERFACE
*
* Generic interface for health management:
* - Exposes health-related properties and events
*/

public interface IHealth
{
    int CurrentHealth { get; }
    int HumonMaxHealth { get; }

    int BuildingMaxHealth { get; }
    bool IsDead { get; }

    event System.Action<HealthChangedArgs> OnDamaged;
    // event System.Action<HealthChangedArgs> OnHealed;
    event System.Action<SpawnedArgs> OnSpawned;
    event System.Action<DeathArgs> OnDied;

    void Spawn();
    void TakeDamage(int amount, object source = null);
    // void Heal(int amount, object source = null);
    // void Kill(object source = null);
}

