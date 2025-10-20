using GrimTools.Runtime.Core;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Upgrade
{
    [Serializable]
    public class UpgradeMilestone
    {
        public int requiredDeath = 0;
        public List<UpgradeData> upgrades;
        public bool isComplete = false;
        
    }
    public class UpgradeManager : MonoSingleton<UpgradeManager>
    {
        public HumonStats baseStats;
        private HumonStats m_currentStats;
        public HumonStats CurrentStats => m_currentStats;
        public List<UpgradeMilestone> milestones; 
        private void Start()
        {
            Initialize();
            GameManager.Instance.OnHumonDeathCountChanged += CheckforMilestone;
        }
        private void Initialize()
        {
            m_currentStats = baseStats.CreateCopy();
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnHumonDeathCountChanged -= CheckforMilestone;
        }

        private void CheckforMilestone(int currentDeathCount)
        {
            foreach (var m in milestones)
            {
                if(m.isComplete) continue;
                if(currentDeathCount >= m.requiredDeath)
                {
                    ApplyMilestoneUpgrades(m);
                }
            }
        }

        private void ApplyMilestoneUpgrades(UpgradeMilestone milestone)
        {
            milestone.isComplete = true;
            milestone.upgrades.ForEach(u => u.ApplyToStats(m_currentStats));
        }
    }
}

