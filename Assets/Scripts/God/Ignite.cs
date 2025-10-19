using UnityEngine;
using System;

namespace God
{

public class Ignite : BaseTool
{
    private float _cooldown = 1;
    private float _duration = 10;
    private float _radius = 1;
    private int _damage = 1;
    private int _cost = 100;

    private float _timestamp = 0;

    public GameObject Prefab = null;

    public override void MouseDown()
    {
        var now = Time.time;
        if (_timestamp + _cooldown > now)
        {
            // on cooldown...
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(
                Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit,
                Mathf.Infinity, LayerMask.GetMask("Default")))
        {
            return;
        }

        var fire = Instantiate(
                Prefab, hit.point, Quaternion.identity);
        fire.transform.localScale *= _radius;

        var impl = fire.GetComponent<Fire>();
        impl.Damage = _damage;
        impl.Radius = _radius;
        impl.Duration = _duration;

        Destroy(fire, _duration);

        _timestamp = now;
    }

    public override void MouseUp()
    {
    }

    public override void FixedUpdate()
    {
    }

    public override void Update()
    {
    }

    public override void Upgrade()
    {
        _radius *= 2;
        _cost = Math.Max(_cost, _cost * 2);
    }

    public override int UpgradeCost()
    {
        return _cost;
    }
}

} // namespace God

