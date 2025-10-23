using UnityEngine;
using System;
using UnityEngine.EventSystems;

namespace God
{

public class Hammer : BaseTool
{
    public GameObject Prefab;
    private float _radius = 6f;
    private int _damage = 10;

    private int _cost = 50;

    public static string Name = "hammer";
    public static int Price = 10;

    public override float Range()
    {
        return _radius;
    }

    public override void MouseDown()
    {
        Ray ray = Camera.main.ScreenPointToRay(
                Input.mousePosition);

        if (EventSystem.current.IsPointerOverGameObject())
        {
            // Pointer is over a UI element, so skip raycasting into the world
            return;
        }
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Default")))
        {
            return;
        }

        var vfx = Instantiate(Prefab, hit.point, Quaternion.identity);
        Destroy(vfx, 0.5f);

        Collider[] hits = Physics.OverlapSphere(hit.point,
                _radius, LayerMask.GetMask("NPC", "Building"));

        foreach (var collider in hits)
        {
            if (collider.TryGetComponent<Health>(out Health hp))
            {
                hp.TakeDamage(_damage, source: "hammer");
            }
            else
            {
                Health parentHp = collider.GetComponentInParent<Health>();
                if (parentHp != null)
                {
                    parentHp.TakeDamage(_damage, source: "hammer");
                    Debug.Log($"[Hammer] Hit building {parentHp.name} via {collider.name}");
                }
            }
        }
    }

    public override void MouseUp()
    {
    }

    public override void Update()
    {
    }

    public override void FixedUpdate()
    {
    }

    public override void Upgrade()
    {
        _damage *= 2;
        _cost = Math.Max(_cost, _cost * 2);
    }

    public override int UpgradeCost()
    {
        return _cost;
    }
}

} // namespace God
