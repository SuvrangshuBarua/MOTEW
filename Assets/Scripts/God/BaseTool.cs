using UnityEngine;

namespace God
{

public abstract class BaseTool : ScriptableObject
{
    public abstract void MouseDown();
    public abstract void MouseUp();
    public abstract void FixedUpdate();
}

} // namespace God

