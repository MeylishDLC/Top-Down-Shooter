using System;
using System.Threading;
using Bullets.BulletPools;
using Cysharp.Threading.Tasks;
using FMODUnity;
using Player.PlayerControl.GunMovement;
using SoundSystem;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

namespace Weapons.Guns
{
    public class BasicGun: Gun
    {
        [Header("Sound")]
        [SerializeField] private EventReference shootSound;
        [SerializeField] private float soundDelay;
        
        [Header("Components")] 
        [SerializeField] private Animator firingPointAnimator;
        [SerializeField] private Image reloadingImage;

        [Header("Kickback And Dispersion")] 
        [SerializeField] private Transform kickbackTransform;
        [SerializeField] private float kickbackDistance;
        [SerializeField] private float kickbackDuration;
        [SerializeField] private float dispersionAngle;
        
        private AudioManager _audioManager;
        private PlayerKickback _playerKickback;
        private BulletPool _bulletPool;
        
        private float _nextSoundTime;
        private float _remainingTime;
        private UniTask _soundTask = UniTask.CompletedTask;
        private static readonly int shoot = Animator.StringToHash("shoot");

        [Inject]
        public void Construct(AudioManager audioManager)
        {
            _audioManager = audioManager;
        }
        public override void Initialize(BulletPool bulletPool)
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
            PlaySound();
        }

        private void PlaySound()
        {
            if (Time.time >= _nextSoundTime)
            {
                _audioManager.PlayOneShot(shootSound);
                _nextSoundTime = Time.time + soundDelay;
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
            _audioManager.PlayOneShot(_audioManager.FMODEvents.ReloadSound);
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