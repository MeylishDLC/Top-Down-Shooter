using Player.PlayerCombat;
using Player.PlayerControl;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace UI.Menus
{
    public class GameOverScreen: MonoBehaviour
    {
        [SerializeField] private GameObject gameOverScreen;
        [SerializeField] private Button restartButton;
        
        private PlayerHealth _playerHealth;

        [Inject]
        public void Construct(PlayerMovement playerMovement)
        {
            _playerHealth = playerMovement.gameObject.GetComponent<PlayerHealth>();
        }
        private void Awake()
        {
            _playerHealth.OnDeath += ShowGameOverScreen;
            restartButton.onClick.AddListener(RestartLevel);
            gameOverScreen.SetActive(false);
        }
        private void OnDestroy()
        {
            _playerHealth.OnDeath -= ShowGameOverScreen;
        }
        private void ShowGameOverScreen()
        {
            gameOverScreen.SetActive(true);
        }

        private void RestartLevel()
        {
            restartButton.interactable = false;
            SceneManager.LoadScene("SampleScene");
        }
    }
}