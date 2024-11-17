using System;
using System.Threading;
using Bullets;
using Bullets.BulletPools;
using Cysharp.Threading.Tasks;
using Player.PlayerControl.GunMovement;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
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
        [SerializeField] private Image reloadingImage;

        [Header("Kickback And Dispersion")] 
        [SerializeField] private Transform kickbackTransform;
        [SerializeField] private float kickbackDistance;
        [SerializeField] private float kickbackDuration;
        [SerializeField] private float dispersionAngle;
   
        private static readonly int shoot = Animator.StringToHash("shoot");
        
        private PlayerKickback _playerKickback;
        private BulletPool _bulletPool;
        private float _remainingTime;
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
    }
}