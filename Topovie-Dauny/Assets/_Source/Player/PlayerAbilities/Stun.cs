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
    [CreateAssetMenu(fileName = "Stun", menuName = "Combat/Abilities/Stun")]
    public class Stun: Ability
    {
        public override event Action OnAbilitySuccessfullyUsed;

        [SerializeField] private StunZone stunZonePrefab;
        private Transform _playerTransform;
        private AudioManager _audioManager;
        public override void Construct(PlayerMovement playerMovement, ProjectContext projectContext)
        {
            _audioManager = projectContext.Container.Resolve<AudioManager>();
            _playerTransform = playerMovement.transform;
        }
        public override void UseAbility()
        {
            UseAbilityAsync(CancellationToken.None).Forget();
        }
        private async UniTask UseAbilityAsync(CancellationToken token)
        {
            Instantiate(stunZonePrefab, _playerTransform.position, Quaternion.identity);
            _audioManager.PlayOneShot(_audioManager.FMODEvents.StunSound);
            OnAbilitySuccessfullyUsed?.Invoke();
            await UniTask.Delay(CooldownMilliseconds, cancellationToken: token);
        }
    }
}