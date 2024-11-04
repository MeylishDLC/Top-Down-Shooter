using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.InputSystem;
using Cysharp.Threading.Tasks;
using Player.PlayerControl.GunMovement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

namespace Player.PlayerCombat
{
    public class WeaponsSetter
    {
        public int CurrentActiveGunIndex { get; private set; }
        public IEnumerable<Gun> Guns => _guns.Values;

        private readonly Image _gunUIImage;
        private readonly Color _keyImageEnabledColor;
        private readonly Color _keyImageDisabledColor;
        private readonly GunRotation _gunRotation;
        private readonly SerializedDictionary<int, Gun> _guns;
        
        private float _fireTimer;
        private CancellationTokenSource _stopShootingOnHoldCts = new();
        public void SubscribeOnInputEvents(InputListener inputListener)
        {
            inputListener.OnFirePressed += HandleShooting;
            inputListener.OnFireReleased += StopShooting;
            inputListener.OnSwitchWeaponPressed += SwitchWeapon;
        }
        public void UnsubscribeOnInputEvents(InputListener inputListener)
        {
            inputListener.OnFirePressed -= HandleShooting;
            inputListener.OnFireReleased -= StopShooting;
            inputListener.OnSwitchWeaponPressed -= SwitchWeapon;
        }
        public WeaponsSetter(WeaponsSetterConfig config)
        {
            _guns = config.Guns;
            _gunUIImage = config.GunUIImage;
            _keyImageDisabledColor = config.GunKeyColorDisabled;
            _keyImageEnabledColor = config.GunKeyColorEnabled;
            _gunRotation = config.GunRotation;
            
            CurrentActiveGunIndex = GetActiveWeaponIndex();
            var currentGun = _guns.Values.ElementAt(CurrentActiveGunIndex);
            
            _gunRotation.CurrentGun = currentGun.GetComponent<SpriteRenderer>();
            _gunUIImage.sprite = currentGun.GunIconSprite;
            currentGun.GunKeyImage.color = _keyImageEnabledColor;
        }
        private void HandleShooting()
        {
            if (_guns.Values.ElementAt(CurrentActiveGunIndex).ShootOnHold)
            {
                ShootByHold(_stopShootingOnHoldCts.Token).Forget();
            }
            else
            {
                ShootByTap();
            }
        }
        private void ShootByTap()
        {
            Shoot();
        }
        private async UniTask ShootByHold(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (_fireTimer <= 0)
                    {
                        Shoot();
                        _fireTimer = _guns.Values.ElementAt(CurrentActiveGunIndex).FireRate;
                    }
                    else
                    {
                        _fireTimer -= Time.deltaTime;
                    }
                    await UniTask.Yield(PlayerLoopTiming.Update);
                }
            }
            catch
            {
                //ignored
            }
        }
        private void StopShooting()
        {
            CancelRecreateCts();
        }
        private void Shoot()
        {
            _guns.Values.ElementAt(CurrentActiveGunIndex).Shoot();
        }
        private void SwitchWeapon(int weaponNum)
        {
            if (!_guns.ContainsKey(weaponNum))
            {
                throw new Exception($"The gun with the key {weaponNum} is not registered.");
            }
            
            var weaponIndex = weaponNum - 1;
            if (!_guns.Values.ElementAt(weaponIndex).IsUnlocked)
            {
                Debug.Log("The weapon is not unlocked yet");
                return;
            }
            
            var gunArray = _guns.Values.ToArray();
            var currentGun = gunArray[CurrentActiveGunIndex];
            
            currentGun.gameObject.SetActive(false);
            if (currentGun.IsReloading)
            {
                InterruptReload(currentGun);
            }

            currentGun.GunKeyImage.color = _keyImageDisabledColor;
            
            CurrentActiveGunIndex = weaponIndex;
            
            gunArray[CurrentActiveGunIndex].gameObject.SetActive(true);
            ChangeVisuals(gunArray[CurrentActiveGunIndex]);
        }
        private void ChangeVisuals(Gun gun)
        {
            _gunRotation.CurrentGun = gun.GetComponent<SpriteRenderer>();
            
            gun.OnBulletsAmountChange
                .Invoke(gun.CurrentBulletsAmount);
            _gunUIImage.sprite = gun.GunIconSprite;
            gun.GunKeyImage.color = _keyImageEnabledColor;
        }
        private int GetActiveWeaponIndex()
        {
            var gunObjectsValuesArray = _guns.Values.ToArray();
            for (int i = 0; i < gunObjectsValuesArray.Length; i++)
            {
                if (gunObjectsValuesArray[i].gameObject.activeSelf)
                {
                    return i;
                }
            }
            throw new Exception("No weapon was active");
        }
        private void InterruptReload(Gun currentGun)
        {
            currentGun.CancelReloadCts.Cancel();
            currentGun.CancelReloadCts.Dispose();
            currentGun.CancelReloadCts = new CancellationTokenSource();
                
            currentGun.StopReload();
        }
        private void CancelRecreateCts()
        {
            if (_stopShootingOnHoldCts != null)
            {
                _stopShootingOnHoldCts.Cancel();
                _stopShootingOnHoldCts.Dispose();
            }
            _stopShootingOnHoldCts = new();
        }
    }
}