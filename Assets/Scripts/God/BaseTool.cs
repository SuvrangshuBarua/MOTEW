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

    public virtual int UpgradeCost()
    {
        return int.MaxValue;
    }
}

} // namespace God

