using System;
using GameAnalyticsSDK;
using Player.PlayerAbilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Analytics
{
    public class AnalyticsManager : MonoBehaviour
    {
        private int _generalDeathsCount;
        private int _deathsOnBossCount;
        private int _healOrbsCollectedOnLevel;
        private int _playerReachedEnding;
        private const string GeneralDeathsKey = "GeneralDeathsCount";
        private const string DeathsOnBossKey = "DeathsOnBossCount";
        private const string ReachedEndingKey = "ReachedEnding";
        private void Awake()
        {
            _generalDeathsCount = InitializeDataFromPlayerPrefs(GeneralDeathsKey);
            _deathsOnBossCount = InitializeDataFromPlayerPrefs(DeathsOnBossKey);
            _playerReachedEnding = InitializeDataFromPlayerPrefs(ReachedEndingKey);
            
            if (!GameAnalytics.Initialized)
            {
                GameAnalytics.Initialize();
            }
            
            //for the task
            OnError("test error");
        }
        public void OnLevelComplete(int levelNumber)
        {
            _healOrbsCollectedOnLevel = 0;
            
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, $"level_{levelNumber}");        }
        public void OnLevelFailed(int levelNumber)
        {
            _generalDeathsCount++;
            SaveProgressToPlayerPrefs(GeneralDeathsKey, _generalDeathsCount);
            GameAnalytics.NewDesignEvent($"deaths:general:{_generalDeathsCount}");            
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, $"level_{levelNumber}");
        }
        public void OnDeathOnBoss(int levelNumber)
        {
            OnLevelFailed(levelNumber);
            
            _deathsOnBossCount++;
            SaveProgressToPlayerPrefs(DeathsOnBossKey, _deathsOnBossCount);
            GameAnalytics.NewDesignEvent($"deaths:boss:{_generalDeathsCount}");
        }
        public void OnAbilityEquipped(Ability equippedAbility)
        {
            GameAnalytics.NewDesignEvent($"ability:equipped:{equippedAbility.name}");
        }
        public void OnEndingReached()
        {
            if (_playerReachedEnding == 1)
            {
                return;
            }
            SaveProgressToPlayerPrefs(ReachedEndingKey, 1);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete,"ending_reached");
        }
        public void OnHealOrbCollected()
        {
            _healOrbsCollectedOnLevel++;
            //there are actually no resources in the game so ill be counting heal orbs which being dropped from mobs 
            //for the sake of the task xd
            GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "heal", _healOrbsCollectedOnLevel, "drop", "heal_orb");
        }
        public void OnError(string errorMessage)
        {
            GameAnalytics.NewErrorEvent(GAErrorSeverity.Error, errorMessage);
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
        private void OnDestroy()
        {
            GameAnalytics.EndSession();
        }
    }
}
