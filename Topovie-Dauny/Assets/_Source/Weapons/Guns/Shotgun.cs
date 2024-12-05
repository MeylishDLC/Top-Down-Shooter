using System;
using System.Threading;
using Bullets.BulletPools;
using Cysharp.Threading.Tasks;
using Player.PlayerControl.GunMovement;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Weapons.Guns
{
    public class Shotgun: Gun
    {
        [field:Header("Main Settings")]
        [SerializeField] private Transform firingPoint;
        [SerializeField] private int bulletsPerShot = 5;
        [SerializeField] private float spreadAngle = 45f;
        
        [Header("Components")]
        [SerializeField] private Animator firingPointAnimator;
        [SerializeField] private Image reloadingImage;

        [Header("Kickback And Dispersion")] 
        [SerializeField] private Transform kickbackTransform;
        [SerializeField] private float kickbackDistance;
        [SerializeField] private float kickbackDuration;
   
        private static readonly int shoot = Animator.StringToHash("shoot");
        
        private PlayerKickback _playerKickback;
        private BulletPool _bulletPool;
        private float _remainingTime;
        private bool _canShoot = true;
        
        public override void Initialize(BulletPool bulletPool)
        {
            _playerKickback = new PlayerKickback(kickbackDistance, kickbackDuration,transform, kickbackTransform);
            CurrentBulletsAmount = BulletsAmount;
            _bulletPool = bulletPool;
        }
        public override void Shoot()
        {
            if (!_canShoot)
            {
                return;
            }
            
            if (!IsReloading && CurrentBulletsAmount > 0)
            {
                HandleShooting();
                CurrentBulletsAmount--;
                OnBulletsAmountChange?.Invoke(CurrentBulletsAmount);
            }
            
            if (!IsReloading && CurrentBulletsAmount == 0)
            {
                IsReloading = true;
                Reload();
            }
        }
        public override void StopReload()
        {
            HideReloadingImage();
            CurrentBulletsAmount = 0;
            IsReloading = false;   
        }
        protected override void Reload()
        {
            ReloadAsync(CancelReloadCts.Token).Forget();
        }
        private void HandleShooting()
        {
            firingPointAnimator.SetTrigger(shoot);
            var firingPointTransform = firingPointAnimator.transform;

            for (var i = 0; i < bulletsPerShot; i++)
            {
                if (_bulletPool.TryGetFromPool(out var bullet))
                {
                    bullet.transform.position = firingPointTransform.position;

                    var direction = GetBulletDirectionWithSpread(firingPointTransform.right);
                    bullet.transform.right = direction;
                }
            }
            _playerKickback.ApplyKickback(CancellationToken.None).Forget();
            WaitDelayBetweenShots(CancellationToken.None).Forget();
        }
        private Vector2 GetBulletDirectionWithSpread(Vector2 forward)
        {
            var halfSpread = spreadAngle / 2f;
            var randomAngle = Random.Range(-halfSpread, halfSpread);

            return Quaternion.Euler(0, 0, randomAngle) * forward;
        }

        private async UniTask WaitDelayBetweenShots(CancellationToken token)
        {
            _canShoot = false;
            await UniTask.Delay(TimeSpan.FromSeconds(FireRate), cancellationToken: token);
            _canShoot = true;
        }
        private async UniTask ReloadAsync(CancellationToken token)
        {
            ShowReloadingImage();
            await UpdateReloadingImageAsync(token);
            if (!token.IsCancellationRequested)
            {
                HideReloadingImage();

                CurrentBulletsAmount = BulletsAmount;
                OnBulletsAmountChange?.Invoke(CurrentBulletsAmount);
                IsReloading = false;
            }
        }
        private void ShowReloadingImage()
        {
            reloadingImage.gameObject.SetActive(true);
            reloadingImage.fillAmount = 1;
            _remainingTime = ReloadTime;
        }
        private void HideReloadingImage()
        {
            reloadingImage.gameObject.SetActive(false);
            reloadingImage.fillAmount = 0;
            _remainingTime = 0;
        }
        private async UniTask UpdateReloadingImageAsync(CancellationToken token)
        {
            while (_remainingTime > 0 && !token.IsCancellationRequested)
            {
                _remainingTime -= Time.deltaTime;
                reloadingImage.fillAmount = Mathf.Clamp01(_remainingTime / ReloadTime);
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }
        
        private void OnDrawGizmos()
        {
            if (firingPoint == null) return;

            Gizmos.color = Color.red;

            var forward = firingPoint.right;
            var leftBoundary = Quaternion.Euler(0, 0, -spreadAngle / 2) * forward;
            var rightBoundary = Quaternion.Euler(0, 0, spreadAngle / 2) * forward;

            Gizmos.DrawRay(firingPoint.position, leftBoundary * 2f);
            Gizmos.DrawRay(firingPoint.position, rightBoundary * 2f);

            Gizmos.color = Color.green;
            Gizmos.DrawRay(firingPoint.position, forward * 2f);
        }
    }
}