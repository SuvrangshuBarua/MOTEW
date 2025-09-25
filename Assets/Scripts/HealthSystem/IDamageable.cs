using UnityEngine;

/* 
* Generic interface for damageable objects
* Should be kept simple, should not handle computation of damage
* Exposes TakeDamage method
*/

public interface IDamageable
{
    void TakeDamage(int amount, object source = null);
}
