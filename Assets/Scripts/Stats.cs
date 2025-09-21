using UnityEngine;

[CreateAssetMenu(fileName = "Stats", menuName = "MOTEW/Stats", order = 51)]
public class Stats : ScriptableObject
{
    [SerializeField] private int _baseHealth = 100;
    [SerializeField] private float _baseRange = 1;
    [SerializeField] private float _baseSpeed = 2;
    
    public int BaseHealth => _baseHealth;
    public float BaseRange => _baseRange;
    public float BaseSpeed => _baseSpeed;
}
