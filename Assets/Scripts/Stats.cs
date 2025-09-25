using UnityEngine;

[CreateAssetMenu(fileName = "Stats", menuName = "MOTEW/Stats", order = 51)]
public class Stats : ScriptableObject
{
    [Header("Base Stats")]
    [SerializeField] private int _baseHealth = 100;
    [SerializeField] private float _baseRange = 1;
    [SerializeField] private float _baseSpeed = 2;

    public int BaseHealth => _baseHealth;
    public float BaseRange => _baseRange;
    public float BaseSpeed => _baseSpeed;

    [Header("Fall Damage")]
    [SerializeField] private float _minImpactSpeed = 7.5f;
    [SerializeField] private float _damagePerSpeed = 2.0f;
    [SerializeField] private int _maxFallDamage = 50;

    public float MinImpactSpeed => _minImpactSpeed;
    public float DamagePerSpeed => _damagePerSpeed;
    public int MaxFallDamage => _maxFallDamage;
}
