using UnityEngine;

public class FireDamage : MonoBehaviour
{
    public int Damage;
    public uint _ticks = 0;

    void FixedUpdate()
    {
        if ((++_ticks % 4) != 0)
        {
            return;
        }

        var hp = GetComponent<IHealth>();
        hp.TakeDamage(Damage, gameObject);
    }
}

