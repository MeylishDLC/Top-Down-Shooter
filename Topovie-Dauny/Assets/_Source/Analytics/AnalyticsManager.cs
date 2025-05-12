using System;
using _Support.GameAnalytics.Plugins.Scripts;
using Player.PlayerAbilities;
using UnityEngine;

namespace Analytics
{
    public class AnalyticsManager : MonoBehaviour
    {
        private int _generalDeathsCount;
        private int _deathsOnBossCount;
        private int _playerReachedEnding;
        private const string GeneralDeathsKey = "GeneralDeathsCount";
        private const string DeathsOnBossKey = "DeathsOnBossCount";
        private const string ReachedEndingKey = "ReachedEnding";
        private void Awake()
        {
            _generalDeathsCount = InitializeDataFromPlayerPrefs(GeneralDeathsKey);
            _deathsOnBossCount = InitializeDataFromPlayerPrefs(DeathsOnBossKey);
            _playerReachedEnding = InitializeDataFromPlayerPrefs(ReachedEndingKey);
            
            GameAnalytics.Initialize();
        }

        public void OnLevelComplete(int levelNumber)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Levels passed_" + levelNumber);
        }
        public void OnDeath()
        {
            _generalDeathsCount++;
            SaveProgressToPlayerPrefs(GeneralDeathsKey, _generalDeathsCount);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "General deaths_" + _generalDeathsCount);
        }
        public void OnDeathOnBoss()
        {
            OnDeath();
            
            _deathsOnBossCount++;
            SaveProgressToPlayerPrefs(DeathsOnBossKey, _deathsOnBossCount);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Deaths on boss_" + _deathsOnBossCount);
        }
        public void OnAbilityEquipped(Ability equippedAbility)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Ability equipped_" + equippedAbility.name);
        }
        public void OnEndingReached()
        {
            if (_playerReachedEnding == 1)
            {
                return;
            }
            SaveProgressToPlayerPrefs(ReachedEndingKey, 1);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Player reached ending");
        }
        private int InitializeDataFromPlayerPrefs(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetInt(key);
            }
            return 0;
        }
        private void SaveProgressToPlayerPrefs(string key, int newValue)
        {
            PlayerPrefs.SetInt(key, newValue);
        }
    }
}
