using System.Threading;
using Cysharp.Threading.Tasks;
using Player.PlayerControl;
using UnityEngine;
using Weapons.AbilityWeapons;

namespace Player.PlayerAbilities
{    
    [CreateAssetMenu(fileName = "Stun", menuName = "Combat/Abilities/Stun")]
    public class Stun: Ability
    {
        [SerializeField] private StunZone stunZonePrefab;

        private Transform _playerTransform;

        public override void Construct(PlayerMovement playerMovement)
        {
            _playerTransform = playerMovement.transform;
        }
        public override void UseAbility()
        {
            UseAbilityAsync(CancellationToken.None).Forget();
        }
        private async UniTask UseAbilityAsync(CancellationToken token)
        {
            Instantiate(stunZonePrefab, _playerTransform.position, Quaternion.identity);
            OnAbilitySuccessfullyUsed?.Invoke();
            await UniTask.Delay(CooldownMilliseconds, cancellationToken: token);
        }
    }
}