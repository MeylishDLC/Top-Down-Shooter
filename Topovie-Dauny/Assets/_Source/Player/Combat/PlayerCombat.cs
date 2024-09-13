using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Player.Combat
{
    public class PlayerCombat
    {
        public event EventHandler<OnShootEventArgs> OnShoot;
        public class OnShootEventArgs : EventArgs
        {
            public Vector3 GunEndPointPosition;
            public Vector3 shootPosition;
        }
        
        private PlayerController _playerController;
        public PlayerCombat(PlayerController playerController)
        {
            _playerController = playerController;
        }
        public void HandleAiming()
        {
            var mousePos = GetMouseWorldPosition();
            var aimDirection = (mousePos - _playerController.transform.position).normalized;
            var angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            _playerController.AimTransform.eulerAngles = new Vector3(0, 0, angle);
        }

        public void HandleShooting()
        {
            var mousePos = GetMouseWorldPosition();
            // OnShoot?.Invoke(this, new OnShootEventArgs 
            //     {GunEndPointPosition = _playerController.AimTransform.position, shootPosition = mousePos});
            var bullet = Object.Instantiate(_playerController.BulletPrefab, _playerController.AimTransform.position, Quaternion.identity);

            var bulletRb = bullet.GetComponent<Rigidbody>();
            
            bulletRb.AddForce(mousePos * bullet.Speed, ForceMode.Impulse);
        }
        
        private Vector3 GetMouseWorldPosition()
        {
            var mousePosition = Input.mousePosition;
            var worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            worldPosition.z = 0;
            return worldPosition;
        }
    }
}