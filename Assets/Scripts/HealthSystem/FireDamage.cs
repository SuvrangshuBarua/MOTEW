using UnityEngine;

public class FireDamage : MonoBehaviour
{
    public int Damage;

    void FixedUpdate()
    {
        var hp = GetComponent<IHealth>();
        hp.TakeDamage(Damage, gameObject);
    }
}

