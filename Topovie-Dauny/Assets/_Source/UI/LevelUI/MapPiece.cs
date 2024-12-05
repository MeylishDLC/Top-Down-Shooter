using Core.LevelSettings;
using Core.SceneManagement;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace UI.LevelUI
{
    public class MapPiece: MonoBehaviour
    {
        [SerializeField] private int levelNumber;
        [SerializeField] private ConfirmationScreen confirmationScreen;
        
        private Button _button;
        private Image _image;
        private LevelSave _levelSave;
        private SceneLoader _sceneLoader;
        private int _levelBuildIndex;
        
        [Inject]
        public void Construct(LevelSave levelSave, SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
            _levelSave = levelSave;
            _levelBuildIndex = levelNumber + 2;
        }
        private void Awake()
        {
            if (levelNumber < 0)
            {
                Debug.LogError("LevelNumber must be greater than 0");
            }
            _button = GetComponentInChildren<Button>();
            _button.onClick.AddListener(ShowConfirmationScreen);
            confirmationScreen.OnConfirmed += LoadLevel;
            
            gameObject.SetActive(IsLevelUnlocked());
        }
        private bool IsLevelUnlocked()
        {
            var nextLevelNumber = _levelSave.GetLevelsPassed();

            if (levelNumber <= nextLevelNumber)
            {
                return true;
            }
            return false;
        }
        private void ShowConfirmationScreen()
        {
            confirmationScreen.OpenConfirmationScreen();
        }
        private void LoadLevel()
        {
            confirmationScreen.gameObject.SetActive(false);
            if (SceneManager.sceneCountInBuildSettings <= _levelBuildIndex)
            {
                return;
            }
            _sceneLoader.LoadSceneAsync(_levelBuildIndex, false).Forget();
        }
    }
}