using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using FMODUnity;
using Player.PlayerControl;
using SoundSystem;
using UnityEngine;
using Weapons.AbilityWeapons;
using Zenject;

namespace Player.PlayerAbilities
{
    [CreateAssetMenu(fileName = "Knife", menuName = "Combat/Abilities/Knife")]
    public class Knife: Ability
    {
        public override event Action OnAbilitySuccessfullyUsed;
        
        [Header("Specific Settings")] 
        [SerializeField] private KnifeObject knifePrefab;
        
        private Transform _playerTransform;
        private AudioManager _audioManager;
        public override void Construct(PlayerMovement playerMovement, ProjectContext projectContext)
        {
            _playerTransform = playerMovement.transform;
            _audioManager = projectContext.Container.Resolve<AudioManager>();
        }
        public override void UseAbility()
        {
            UseAbilityAsync(CancellationToken.None).Forget();
        }

        private async UniTask UseAbilityAsync(CancellationToken token)
        { 
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            var direction = (mousePosition - _playerTransform.position).normalized;
            
            var knife = Instantiate(knifePrefab, _playerTransform.position, Quaternion.identity);

            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            knife.transform.rotation = Quaternion.Euler(0, 0, angle);

            knife.ShootInDirection(direction);
            _audioManager.PlayOneShot(_audioManager.FMODEvents.KnifeSound);
            OnAbilitySuccessfullyUsed?.Invoke();

            await UniTask.Delay(CooldownMilliseconds, cancellationToken: token);
        }
    }
}