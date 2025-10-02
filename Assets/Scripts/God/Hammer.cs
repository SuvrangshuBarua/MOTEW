using UnityEngine;

public class Hammer : BaseTool
{
    private float _radius = 6f;
    private int _damage = 50;

    public override void Use(RaycastHit hit)
    {
        Collider[] hits = Physics.OverlapSphere(hit.point,
                _radius, LayerMask.GetMask("NPC"));

        foreach (var collider in hits)
        {
            if (collider.TryGetComponent<Health>(out Health hp))
            {
                hp.TakeDamage(_damage);
            }
        }
    }
}

