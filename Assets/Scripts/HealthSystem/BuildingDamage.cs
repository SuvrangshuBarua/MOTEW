using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class BuildingDamage : MonoBehaviour
{

    [Header("Data")]
    IDamageable _damageable;

    void Awake()
    {
        _damageable = GetComponent<IDamageable>();
    }

    public void ApplyDamage(int amount, object source = null)
    {
        if (_damageable != null && amount > 0)
            _damageable.TakeDamage(amount, source ?? "hammer");
    }

    // Update is called once per frame
    void Update()
    {

    }

}
