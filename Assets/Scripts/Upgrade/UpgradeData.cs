using UnityEngine;

namespace Upgrade
{
    [CreateAssetMenu(fileName = "New Upgrade", menuName = "MOTEW/Upgrade Data", order = 51)]
    public class UpgradeData : ScriptableObject
    {
        [SerializeField] private int requiredDeathCount = 15;
        public int RequiredDeathCount => requiredDeathCount;
        
        
    }

}
