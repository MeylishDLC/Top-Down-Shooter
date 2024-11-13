using System;
using System.Threading;
using Bullets;
using Bullets.BulletPools;
using Cysharp.Threading.Tasks;
using Player.PlayerControl.GunMovement;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;
using Random = UnityEngine.Random;

namespace Weapons.Test_Gun
{
    public class BasicGun: Gun
    {
        [field:Header("Main Settings")]
        [SerializeField] private GameObject bulletPrefab;
        
        [Header("Components")]
        [SerializeField] private Animator firingPointAnimator;
        [SerializeField] private TMP_Text reloadingText;

        [Header("Kickback And Dispersion")] 
        [SerializeField] private Transform kickbackTransform;
        [SerializeField] private float kickbackDistance;
        [SerializeField] private float kickbackDuration;
        [SerializeField] private float dispersionAngle;
   
        private static readonly int shoot = Animator.StringToHash("shoot");
        
        private PlayerKickback _playerKickback;
        private BulletPool _bulletPool;
        public void Initialize(BulletPool bulletPool)
        {
            _playerKickback = new PlayerKickback(kickbackDistance, kickbackDuration,transform, kickbackTransform);
            CurrentBulletsAmount = BulletsAmount;
            _bulletPool = bulletPool;
        }
        public override void Shoot()
        {
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
            reloadingText.gameObject.SetActive(false);
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

            if (_bulletPool.TryGetFromPool(out var bullet))
            {
                bullet.transform.position = firingPointTransform.position;
                bullet.transform.rotation = firingPointTransform.rotation;
                bullet.transform.right = GetBulletDirectionWithDispersion();
                _playerKickback.ApplyKickback(CancellationToken.None).Forget();
            } 
        }
        private Vector3 GetBulletDirectionWithDispersion()
        {
            var randomAngle = Random.Range(-dispersionAngle, dispersionAngle);

            var dispersionRotation = Quaternion.Euler(0, 0, randomAngle);
            var bulletDirection = dispersionRotation * transform.right;
            return bulletDirection;
        }
        private async UniTask ReloadAsync(CancellationToken token)
        {
            reloadingText.gameObject.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(ReloadTime), cancellationToken: token);
            reloadingText.gameObject.SetActive(false);

            CurrentBulletsAmount = BulletsAmount;
            OnBulletsAmountChange?.Invoke(CurrentBulletsAmount);
            IsReloading = false;
        }
    }
}