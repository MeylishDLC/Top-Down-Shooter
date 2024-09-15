using Bullets;
using Player.PlayerMovement;
using UnityEngine;
using Weapons;
using Zenject;

namespace Enemies.Combat
{
    public class Shooter: MonoBehaviour
    {
        [SerializeField] private Bullet bulletPrefab;

        private PlayerMovement _playerMovement;
        
        [Inject]
        public void Construct(PlayerMovement playerMovement)
        {
            _playerMovement = playerMovement;
        }
        
        public void Attack()
        {
            var targetDirection = _playerMovement.transform.position - transform.position;

            var newBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            newBullet.transform.right = targetDirection;
        }
    }
}