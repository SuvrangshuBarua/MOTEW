using UnityEngine;

[CreateAssetMenu(fileName = "House Stats", menuName = "MOTEW/House Stats", order = 51)]
public class HouseStats : ScriptableObject
{
    [SerializeField] private int _houseBaseHealth = 100;

    public int HouseBaseHealth => _houseBaseHealth;
}

