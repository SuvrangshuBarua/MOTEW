using UnityEngine;
using System;
using UnityEngine.EventSystems;

namespace God
{

public class Hammer : BaseTool
{
    public GameObject Prefab;

    private Stat[] _cooldowns = new Stat[]
    {
        new Stat(6f, 0),
        new Stat(4f, 100),
        new Stat(2f, 100),
        new Stat(1f, 200),
        new Stat(0.5f, 400),
        new Stat(0.25f, 400),
    };
    private int _cooldown = 0;

    private Stat[] _ranges = new Stat[]
    {
        new Stat(6f, 0),
        new Stat(7f, 50),
        new Stat(8f, 100),
        new Stat(9f, 100),
        new Stat(10f, 100),
        new Stat(15f, 800),
    };
    private int _range = 0;

    private Stat[] _damages = new Stat[]
    {
        new Stat(10f, 0),
        new Stat(20f, 100),
        new Stat(30f, 200),
        new Stat(40f, 400),
        new Stat(80f, 400),
        new Stat(160f, 800),
    };
    private int _damage = 0;

    // time at last usage
    private float _timestamp = -6f;

    public static string Name = "hammer";
    public static int Price = 10;

    public override float Range()
    {
        return _ranges[_range].Value;
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

        var vfx = Instantiate(Prefab, hit.point,
                Quaternion.identity);
        Destroy(vfx, 0.5f);

        Collider[] hits = Physics.OverlapSphere(hit.point,
                _ranges[_range].Value,
                LayerMask.GetMask("NPC", "Building"));

        foreach (var collider in hits)
        {
            if (collider.TryGetComponent<Health>(out Health hp))
            {
                hp.TakeDamage((int) _damages[_damage].Value,
                        source: "hammer");
            }
            else
            {
                Health parentHp = collider
                        .GetComponentInParent<Health>();
                if (parentHp != null)
                {
                    parentHp.TakeDamage(
                            (int) _damages[_damage].Value,
                            source: "hammer");
                }
            }
        }

        _timestamp = now;
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
}

} // namespace God

