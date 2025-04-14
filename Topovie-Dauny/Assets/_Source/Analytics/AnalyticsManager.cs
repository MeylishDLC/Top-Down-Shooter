using System;
using GameAnalyticsSDK;
using Player.PlayerAbilities;
using UnityEngine;

namespace Analytics
{
    public class AnalyticsManager : MonoBehaviour
    {
        private int _generalDeathsCount;
        private int _deathsOnBossCount;
        private const string GeneralDeathsKey = "GeneralDeathsCount";
        private const string DeathsOnBossKey = "DeathsOnBossCount";
        private void Awake()
        {
            _generalDeathsCount = InitializeDataFromPlayerPrefs(GeneralDeathsKey);
            _deathsOnBossCount = InitializeDataFromPlayerPrefs(DeathsOnBossKey);
            
            GameAnalytics.Initialize();
        }

        public void OnLevelComplete(int levelNumber)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Levels passed_" + levelNumber);
            Debug.Log("Level progression sent to analytics");
        }
        public void OnDeath()
        {
            _generalDeathsCount++;
            SaveProgress(GeneralDeathsKey, _generalDeathsCount);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "General deaths_" + _generalDeathsCount);
            Debug.Log("General deaths count sent to analytics");
        }
        public void OnDeathOnBoss()
        {
            OnDeath();
            
            _deathsOnBossCount++;
            SaveProgress(DeathsOnBossKey, _deathsOnBossCount);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Deaths on boss_" + _deathsOnBossCount);
            Debug.Log("Deaths on boss count sent to analytics");
        }
        public void OnAbilityEquipped(Ability equippedAbility)
        {
            
        }

        public void OnEndingReached()
        {
            
        }
        private int InitializeDataFromPlayerPrefs(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetInt(key);
            }
            return 0;
        }
        private void SaveProgress(string key, int newValue)
        {
            PlayerPrefs.SetInt(key, newValue);
        }
    }
}
