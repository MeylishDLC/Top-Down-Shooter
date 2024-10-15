using System.Threading;
using Bullets;
using Cysharp.Threading.Tasks;
using Player.PlayerControl;
using UnityEngine;
using Weapons.AbilityWeapons;
using Zenject;

namespace Player.PlayerAbilities
{
    [CreateAssetMenu(fileName = "Knife", menuName = "Abilities/Knife")]
    public class Knife: Ability
    {
        [SerializeField] private KnifeObject knifePrefab;

        private Transform _playerTransform;

        public override void Construct(PlayerMovement playerMovement)
        {
            _playerTransform = playerMovement.transform;
            AbilityType = AbilityTypes.HoldButton;
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

            knife.transform.right = direction;

            knife.ShootInDirection(direction);

            await UniTask.Delay(CooldownMilliseconds, cancellationToken: token);
        }
    }
}