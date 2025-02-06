using System;
using Player.PlayerCombat;
using Player.PlayerControl;
using UnityEngine;
using Zenject;

namespace Interactable.HealingStuff
{
    public class HealOrb : MonoBehaviour
    { 
        [field: SerializeField] public int HealAmount { get; private set; } = 10;
        private PlayerHealth _playerHealth;
        
        [Inject]
        public void Construct(PlayerMovement playerMovement)
        {
            _playerHealth = playerMovement.gameObject.GetComponent<PlayerHealth>();
        }
        private void Start()
        {
            PlayAppearAnimation();
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _playerHealth.Heal(HealAmount);
                DestroyOrb();
            }
        }
        private void DestroyOrb()
        {
            //todo fade???
            Destroy(gameObject);
        }
        private void PlayAppearAnimation()
        {
            //todo play spawn animation
        }
    }
}
