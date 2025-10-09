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
    public virtual void Upgrade() {}
}

} // namespace God

