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
    public class ChooseLevelButton: MonoBehaviour
    {
        [SerializeField] private int levelNumber;
        [SerializeField] private int levelBuildIndex;
        [SerializeField] private Sprite lockedSprite;
        [SerializeField] private Sprite unlockedSprite;
        [SerializeField] private ConfirmationScreen confirmationScreen;
        
        private Image _image;
        private Button _button;
        private TMP_Text _numberText;
        private LevelSave _levelSave;
        private SceneLoader _sceneLoader;
        
        [Inject]
        public void Construct(LevelSave levelSave, SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
            _levelSave = levelSave;
        }
        private void Awake()
        {
            if (levelNumber < 0)
            {
                Debug.LogError("LevelNumber must be greater than 0");
            }
            
            _image = GetComponent<Image>();
            _button = GetComponent<Button>();
            _numberText = GetComponentInChildren<TMP_Text>();
            
            UpdateButton();
            _button.onClick.AddListener(ShowConfirmationScreen);
            confirmationScreen.OnConfirmed += LoadLevel;
        }
        private void OnDestroy()
        {
            confirmationScreen.OnConfirmed -= LoadLevel;
        }
        private void UpdateButton()
        {
            var nextLevelNumber = _levelSave.GetLevelsPassed();

            if (levelNumber <= nextLevelNumber)
            {
                SetUnlocked();
            }
            else
            {
                SetLocked();
            }
        }
        private void SetLocked()
        {
            _button.interactable = false;
            _image.sprite = lockedSprite;
            _numberText.gameObject.SetActive(false);
        }
        private void SetUnlocked()
        {
            _button.interactable = true;
            _image.sprite = unlockedSprite;
            _numberText.gameObject.SetActive(true);
        }
        private void ShowConfirmationScreen()
        {
            confirmationScreen.OpenConfirmationScreen();
        }
        private void LoadLevel()
        {
            confirmationScreen.gameObject.SetActive(false);
            if (SceneManager.sceneCountInBuildSettings <= levelBuildIndex)
            {
                return;
            }
            _sceneLoader.LoadSceneAsync(levelBuildIndex, false).Forget();
        }
    }
}