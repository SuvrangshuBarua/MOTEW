using UnityEngine;

public enum UpgradeStatType
{
    Health = 0,
    Damage,
    Speed,
}

public enum ModifierType
{
    Add = 0,
    Multiply,
    Percentage
}

[System.Serializable]
public class StatModifier
{
    public UpgradeStatType statType;
    public ModifierType modifierType;
    public float value;
    
    
}
