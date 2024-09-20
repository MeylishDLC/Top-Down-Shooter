using System.Threading;
using Bullets;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Weapons.AbilityWeapons;
using Zenject;

namespace Player.PlayerAbilities
{
    public class Knife: Ability
    {
        [SerializeField] private KnifeObject knifePrefab;

        private Transform _playerTransform;

        [Inject]
        public void Construct(PlayerMovement.PlayerMovement playerMovement)
        {
            _playerTransform = playerMovement.transform;
            AbilityType = AbilityTypes.HoldButton;
        }
        public override void UseAbility()
        {
            if (CanUse)
            {
                UseAbilityAsync(CancellationToken.None).Forget();
            }
        }

        private async UniTask UseAbilityAsync(CancellationToken token)
        {
            CanUse = false;

            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            var direction = (mousePosition - _playerTransform.position).normalized;
            
            var knife = Instantiate(knifePrefab, _playerTransform.position, Quaternion.identity);

            knife.transform.right = direction;

            knife.ShootInDirection(direction);

            await UniTask.Delay(CooldownMilliseconds, cancellationToken: token);
            CanUse = true;
        }
    }
}