using UnityEngine;

namespace Core.LevelSettings
{
    public class LevelSave
    {
        public int LevelsPassed { get; private set; }

        public LevelSave()
        {
            if (PlayerPrefs.HasKey("LevelsPassed"))
            {
                LevelsPassed = PlayerPrefs.GetInt("LevelsPassed");
                Debug.Log("LevelsPassed: " + LevelsPassed);
            }
            else
            {
                LevelsPassed = 0;
                PlayerPrefs.SetInt("LevelsPassed", LevelsPassed);
            }
        }
        public void SaveProgress(int passedLevelNumber)
        {
            PlayerPrefs.SetInt("LevelsPassed", passedLevelNumber + 1);
            Debug.Log($"Levels passed : {GetLevelsPassed()}");
        }
        public int GetLevelsPassed()
        {
            return PlayerPrefs.GetInt("LevelsPassed");
        }
    }
}