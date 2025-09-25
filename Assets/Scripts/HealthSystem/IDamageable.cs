/* 
* DAMAGEABLE INTERFACE
*
* Generic interface for damageable objects:
* - Exposes TakeDamage method
*/

public interface IDamageable
{
    void TakeDamage(int amount, object source = null);
}
