using System;
using GameAnalyticsSDK;
using Player.PlayerAbilities;
using UnityEngine;

namespace Analytics
{
    public class AnalyticsManager : MonoBehaviour
    {
        private void Awake()
        {
            GameAnalytics.Initialize();
        }

        public void OnLevelComplete(int levelNumber)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Level: " + levelNumber);
            Debug.Log("Level progression sent to analytics");
        }
        public void OnDeath()
        {
            
        }
        public void OnDeathOnBoss()
        {
            
        }
        public void OnAbilityEquipped(Ability equippedAbility)
        {
            
        }

        public void OnEndingReached()
        {
            
        }
        
    }
}
