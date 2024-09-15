using System;
using Player.PlayerCombat;
using Player.PlayerMovement;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace UI
{
    public class UIHealthDisplay: MonoBehaviour
    {
        [SerializeField] private TMP_Text playerHealthText;
        
        [Header("Health floating")]
        [SerializeField] private GameObject damageTextPrefab;
        [SerializeField] private Canvas gameCanvas;

        private PlayerHealth _playerHealth;

        [Inject]
        public void Construct(PlayerMovement playerMovement)
        {
            _playerHealth = playerMovement.gameObject.GetComponent<PlayerHealth>();
        }
        private void Awake()
        {
            _playerHealth.OnDamageTaken += InstantiateDamageText;
        }
        private void OnDestroy()
        {
            _playerHealth.OnDamageTaken -= InstantiateDamageText;
        }

        private void Update()
        {
            playerHealthText.text = _playerHealth.CurrentHealth.ToString();
        }

        private void InstantiateDamageText(int damage)
        {
            var playerPos = _playerHealth.transform.position;
            
            var spawnPosition = new Vector3 (playerPos.x, playerPos.y, 0);

            var tmpText = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

            tmpText.text = $"-{damage.ToString()}";
        }
      
    }
}