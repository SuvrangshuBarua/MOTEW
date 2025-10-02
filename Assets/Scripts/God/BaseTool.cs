using UnityEngine;

public abstract class BaseTool : ScriptableObject
{
    public abstract void Use(RaycastHit hit);
}

