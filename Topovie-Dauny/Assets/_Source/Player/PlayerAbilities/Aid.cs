using Cysharp.Threading.Tasks;
using Player.PlayerCombat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Player.PlayerAbilities
{
    public class Aid: Ability
    {
        [Header("Specific Settings")]
        [SerializeField] private int healAmount;

        private PlayerHealth _playerHealth;
        private bool _canUse = true;

        [Inject]
        public void Construct(PlayerMovement.PlayerMovement playerMovement)
        {
            _playerHealth = playerMovement.gameObject.GetComponent<PlayerHealth>();
        }
        
        public override void UseAbility()
        {
            if (!_canUse)
            {
                return;
            }
            
            UseAbilityAsync().Forget();
        }

        private async UniTask UseAbilityAsync()
        {
            Debug.Log("Ability used");
            _canUse = false;
            _playerHealth.Heal(healAmount);
            await UniTask.Delay(CooldownMilliseconds);
            _canUse = true;
        }
    }
}