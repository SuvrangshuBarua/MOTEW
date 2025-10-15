using UnityEngine;

[CreateAssetMenu(fileName = "Building Stats", menuName = "MOTEW/Building Stats", order = 51)]
public class BuildingStats : ScriptableObject
{
    [SerializeField] private int _buildingBaseHealth = 100;

    public int BuildingBaseHealth => _buildingBaseHealth;
}