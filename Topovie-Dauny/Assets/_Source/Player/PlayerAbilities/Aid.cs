using Cysharp.Threading.Tasks;
using Player.PlayerCombat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Player.PlayerAbilities
{
    public class Aid: MonoBehaviour, IAbility
    {
        [field:SerializeField] public int CooldownMilliseconds { get; set; }
        [SerializeField] private int healAmount;

        [Header("UI")] 
        [SerializeField] private TMP_Text cooldownText;
        [SerializeField] private Button abilityButton;

        private PlayerHealth _playerHealth;
        private bool _canUse;

        [Inject]
        public void Construct(PlayerMovement.PlayerMovement playerMovement)
        {
            _playerHealth = playerMovement.gameObject.GetComponent<PlayerHealth>();
        }
        
        public void UseAbility()
        {
            if (!_canUse)
            {
                return;
            }
            
            UseAbilityAsync().Forget();
        }

        private async UniTask UseAbilityAsync()
        {
            _canUse = false;
            _playerHealth.Heal(healAmount);
            await UniTask.Delay(CooldownMilliseconds);
            _canUse = true;
        }
    }
}