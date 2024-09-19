using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Weapons.AbilityWeapons;
using Zenject;

namespace Player.PlayerAbilities
{
    public class Stun: Ability
    {
        [SerializeField] private StunZone stunZonePrefab;

        private Transform _playerTransform;

        [Inject]
        public void Construct(PlayerMovement.PlayerMovement _playerMovement)
        {
            _playerTransform = _playerMovement.transform;
        }
        public override void UseAbility()
        {
            UseAbilityAsync(CancellationToken.None).Forget();
        }

        private async UniTask UseAbilityAsync(CancellationToken token)
        {
            CanUse = false;
            Instantiate(stunZonePrefab, _playerTransform.position, Quaternion.identity);
            await UniTask.Delay(CooldownMilliseconds, cancellationToken: token);
            CanUse = true;
        }
    }
}