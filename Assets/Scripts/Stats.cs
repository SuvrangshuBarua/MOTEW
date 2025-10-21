using UnityEngine;

[CreateAssetMenu(fileName = "Humon stats", menuName = "MOTEW/Humon Stats", order = 51)]
public class HumonStats : ScriptableObject
{
    [SerializeField] private int _baseHealth = 100;
    [SerializeField] private float _baseRange = 1;
    [SerializeField] private float _baseSpeed = 2;

    public int BaseHealth { get => _baseHealth; set => _baseHealth = value; }
    public float BaseRange {get => _baseRange; set => _baseRange = value;}
    public float BaseSpeed {get => _baseSpeed; set => _baseSpeed = value;}

    [Header("Fall Damage")]
    [SerializeField] private float _minFallHeight = 1f;
    [SerializeField] private float _damagePerMeter = 10f;
    [SerializeField] private int _maxFallDamage = 50;

    public float MinFallHeight {get => _minFallHeight; set => _minFallHeight = value;}
    public float DamagePerMeter {get => _damagePerMeter; set => _damagePerMeter = value;}

    public int MaxFallDamage
    {
        get => _maxFallDamage;
        set => _maxFallDamage = value;
    }

    public HumonStats CreateCopy()
    {
        HumonStats copy = CreateInstance<HumonStats>();
        copy.BaseHealth = BaseHealth;
        copy.BaseRange = BaseRange;
        copy.BaseSpeed = BaseSpeed;
        copy.MinFallHeight = MinFallHeight;
        copy.DamagePerMeter = DamagePerMeter;
        copy.MaxFallDamage = MaxFallDamage;
        return copy;
    }
}

