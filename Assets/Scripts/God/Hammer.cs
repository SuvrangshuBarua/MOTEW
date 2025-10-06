using UnityEngine;

namespace God
{

public class Hammer : BaseTool
{
    public GameObject Prefab;

    private float _radius = 6f;
    private int _damage = 50;

    public override void MouseDown()
    {
        Ray ray = Camera.main.ScreenPointToRay(
                Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit))
        {
            return;
        }

        var vfx = Instantiate(Prefab, hit.point, Quaternion.identity);
        Destroy(vfx, 0.5f);

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

    public override void MouseUp()
    {
    }

    public override void FixedUpdate()
    {
    }
}

} // namespace God

