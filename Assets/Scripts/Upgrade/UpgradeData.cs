using UnityEngine;

namespace Upgrade
{
    [CreateAssetMenu(fileName = "New Upgrade", menuName = "MOTEW/Upgrade Data", order = 51)]
    public class UpgradeData : ScriptableObject
    {
        [Header("Upgraded Stats")]
        public int extraHealth;
        public float extraSpeed;
        public float extraRange;
        public float reducedDamagePerMeter;

        public void ApplyToStats(HumonStats stats)
        {
            stats.BaseHealth += extraHealth;
            stats.BaseSpeed += extraSpeed;
            stats.BaseRange += extraRange;
            stats.DamagePerMeter -= reducedDamagePerMeter;
        }
    }

}
