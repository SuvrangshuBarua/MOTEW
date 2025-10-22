using UnityEngine;

[CreateAssetMenu(fileName = "Humon stats", menuName = "MOTEW/Humon Stats", order = 51)]
public class HumonStats : ScriptableObject
{
    [SerializeField] private int _baseHealth = 100;
    [SerializeField] private float _baseRange = 1;
    [SerializeField] private float _baseSpeed = 2;

    public int BaseHealth => _baseHealth;
    public float BaseRange => _baseRange;
    public float BaseSpeed => _baseSpeed;

    [Header("Fall Damage")]
    [SerializeField] private float _minFallHeight = 1f;
    [SerializeField] private float _damagePerMeter = 10f;
    [SerializeField] private int _maxFallDamage = 50;

    public float MinFallHeight => _minFallHeight;
    public float DamagePerMeter => _damagePerMeter;
    public int MaxFallDamage => _maxFallDamage;
}

