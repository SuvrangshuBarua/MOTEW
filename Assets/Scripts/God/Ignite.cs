using UnityEngine;
using System;
using UnityEngine.EventSystems;

namespace God
{

public class Stat
{
    public float Value;
    public int Cost;

    public Stat(float value, int cost)
    {
        Value = value;
        Cost = cost;
    }
}

public class Ignite : BaseTool
{
    private Stat[] _cooldowns = new Stat[]
    {
        new Stat(30f, 0),
        new Stat(20f, 100),
        new Stat(10f, 200),
        new Stat(5f, 400),
        new Stat(1f, 400),
        new Stat(0.5f, 800),
    };
    private int _cooldown = 0;

    private Stat[] _ranges = new Stat[]
    {
        new Stat(1f, 0),
        new Stat(2f, 50),
        new Stat(3f, 100),
        new Stat(4f, 200),
        new Stat(5f, 200),
        new Stat(6f, 400),
    };
    private int _range = 0;

    private Stat[] _damages = new Stat[]
    {
        new Stat(1f, 0),
        new Stat(1.5f, 50),
        new Stat(2f, 100),
        new Stat(3f, 200),
        new Stat(4f, 200),
        new Stat(5f, 200),
    };
    private int _damage = 0;

    // time at last usage
    private float _timestamp = -30f;

    public GameObject Prefab = null;

    public static string Name = "ignite";
    public static int Price = 30;

    public override float Range()
    {
        return _ranges[_range].Value;
    }

    public override bool TryUpgradeCooldown()
    {
        if (_cooldown == _cooldowns.Length - 1)
        {
            return false;
        }

        var cost = _cooldowns[_cooldown + 1].Cost;

        if (!GameManager.Instance.TryDeductCash(cost))
        {
            return false;
        }

        ++_cooldown;
        return true;
    }

    public override Stat[] GetCooldownStats()
    {
        if (_cooldown == _cooldowns.Length - 1)
        {
            return new Stat[]{ _cooldowns[_cooldown] };
        }

        return new Stat[]{
            _cooldowns[_cooldown],
            _cooldowns[_cooldown + 1]
        };
    }

    public override Stat[] GetDamageStats()
    {
        if (_damage == _damages.Length - 1)
        {
            return new Stat[]{ _damages[_damage] };
        }

        return new Stat[]{
            _damages[_damage],
            _damages[_damage + 1]
        };
    }

    public override Stat[] GetRangeStats()
    {
        if (_range == _ranges.Length - 1)
        {
            return new Stat[]{ _ranges[_range] };
        }

        return new Stat[]{
            _ranges[_range],
            _ranges[_range + 1]
        };
    }

    public override bool TryUpgradeDamage()
    {
        if (_damage == _damages.Length - 1)
        {
            return false;
        }

        var cost = _damages[_damage + 1].Cost;
        if (!GameManager.Instance.TryDeductCash(cost))
        {
            return false;
        }

        ++_damage;
        return true;
    }

    public override bool TryUpgradeRange()
    {
        if (_range == _ranges.Length - 1)
        {
            return false;
        }

        var cost = _ranges[_range + 1].Cost;
        if (!GameManager.Instance.TryDeductCash(cost))
        {
            return false;
        }

        ++_range;
        return true;
    }

    public override void MouseDown()
    {
        var now = Time.time;
        if (_timestamp + _cooldowns[_cooldown].Value > now)
        {
            // on cooldown...
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(
                Input.mousePosition);
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // Pointer is over a UI element, so skip raycasting into the world
            return;
        }
        if (!Physics.Raycast(ray, out RaycastHit hit,
                Mathf.Infinity, LayerMask.GetMask("Default")))
        {
            return;
        }

        var fire = Instantiate(
                Prefab, hit.point, Quaternion.identity);
        fire.transform.localScale *= _ranges[_range].Value;

        var impl = fire.GetComponent<Fire>();
        impl.Damage = (int) _damages[_damage].Value;
        impl.Radius = _ranges[_range].Value;
        impl.Duration = 10f;

        Destroy(fire, _cooldowns[_cooldown].Value);

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
}

} // namespace God

