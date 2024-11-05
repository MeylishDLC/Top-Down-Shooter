using System.Linq;
using System.Threading;
using _Support.Demigiant.DOTween.Modules;
using Core.InputSystem;
using Core.LevelSettings;
using Core.SceneManagement;
using Cysharp.Threading.Tasks;
using Player.PlayerCombat;
using Player.PlayerControl;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace UI.Menus
{
    public class GameOverScreen: MonoBehaviour
    {
        [SerializeField] private Image gameOverScreen;
        [SerializeField] private Button restartButton;
        [SerializeField] private float fadeInTime;
        
        private PlayerHealth _playerHealth;
        private SceneLoader _sceneLoader;
        private InputListener _inputListener;
        private LevelChargesHandler _levelChargesHandler;
        
        [Inject]
        public void Construct(PlayerMovement playerMovement, SceneLoader sceneLoader, InputListener inputListener, 
            LevelChargesHandler levelChargesHandler)
        {
            _playerHealth = playerMovement.gameObject.GetComponent<PlayerHealth>();
            _sceneLoader = sceneLoader;
            _inputListener = inputListener;
            _levelChargesHandler = levelChargesHandler;
        }
        private void Awake()
        {
            _playerHealth.OnDeath += ShowGameOverScreen;
            restartButton.onClick.AddListener(RestartLevel);
            gameOverScreen.gameObject.SetActive(false);
        }
        private void OnDestroy()
        {
            _playerHealth.OnDeath -= ShowGameOverScreen;
        }
        private void ShowGameOverScreen()
        {
            ShowGameOverScreenAsync(CancellationToken.None).Forget();
        }
        private async UniTask ShowGameOverScreenAsync(CancellationToken token)
        {
            _levelChargesHandler.StopSpawning();
            DisableInput();
            gameOverScreen.gameObject.SetActive(true);
            await FadeDeathScreen(0f, 0f, token);
            await FadeDeathScreen(1f, fadeInTime, token);
            _levelChargesHandler.ClearAllSpawns();
            Destroy(_playerHealth.gameObject);
        }
        private void RestartLevel()
        {
            restartButton.interactable = false;
            _sceneLoader.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, CancellationToken.None).Forget();
        }
        private void DisableInput()
        {
            _inputListener.SetInput(false);
        }
        private UniTask FadeDeathScreen(float fadeValue, float duration, CancellationToken token)
        {
            var texts = gameOverScreen.GetComponentsInChildren<TMP_Text>();
            var buttonsImages = gameOverScreen.GetComponentsInChildren<Button>()
                .Select(i => i.GetComponent<Image>());
            
            gameOverScreen.DOFade(fadeValue, duration).ToUniTask(cancellationToken: token);
            foreach (var text in texts)
            {
                text.DOFade(fadeValue, duration).ToUniTask(cancellationToken: token);
            }
            foreach (var image in buttonsImages)
            {
                image.DOFade(fadeValue, duration).ToUniTask(cancellationToken: token);
            }

            return UniTask.CompletedTask;
        }
    }
}