using UnityEngine;
using UnityEngine.PlayerLoop;

namespace God
{

public abstract class BaseTool : ScriptableObject
{
    public abstract void MouseDown();
    public abstract void MouseUp();
    public abstract void FixedUpdate();
    public abstract void Update();
    public abstract float Range();
    public virtual void Upgrade() {}
    public virtual int UnlockPrice() { return 0; }
    public virtual int UpgradeCost()
    {
        return int.MaxValue;
    }

    public virtual bool TryUpgradeCooldown() { return false; }
    public virtual bool TryUpgradeDamage() { return false; }
    public virtual bool TryUpgradeRange() { return false; }
    public virtual Stat[] GetCooldownStats() { return null; }
    public virtual Stat[] GetDamageStats() { return null; }
    public virtual Stat[] GetRangeStats() { return null; }
}

} // namespace God

