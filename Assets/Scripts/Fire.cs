using UnityEngine;

public class Fire : MonoBehaviour
{
    public int Damage;
    public float Radius;

    void Start()
    {
        // check static objects
    }

    void FixedUpdate()
    {
        // check dynamic objects
        Collider[] hits = Physics.OverlapSphere(
                transform.position, Radius,
                LayerMask.GetMask("NPC"));

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Humon>(out var humon))
            {
                humon.SetOnFire(Damage);
            }
        }
    }
}

