using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Player.PlayerMovement.GunMovement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

namespace Player.PlayerCombat
{
    public class PlayerWeapons
    {
        public int CurrentActiveGunIndex { get; private set; }
        public IEnumerable<Gun> Guns => _guns.Values;

        private Image _gunUIImage;
        private GunRotation _gunRotation;
        private SerializedDictionary<KeyCode, Gun> _guns;
        private float _fireTimer;
        public PlayerWeapons(SerializedDictionary<KeyCode, Gun> guns, GunRotation gunRotation, Image gunUIImage)
        {
            _guns = guns;
            _gunUIImage = gunUIImage;
            CurrentActiveGunIndex = GetActiveWeaponIndex();
            
            _gunRotation = gunRotation;
            _gunRotation.CurrentGun = _guns.Values.ElementAt(CurrentActiveGunIndex).GetComponent<SpriteRenderer>();
            _gunUIImage.sprite = _guns.Values.ElementAt(CurrentActiveGunIndex).GunIconSprite;
        }
      
        public void HandleShooting()
        {
            if (_guns.Values.ElementAt(CurrentActiveGunIndex).ShootOnHold)
            {
                if (Input.GetMouseButton(0) && _fireTimer <= 0)
                {
                    Shoot();
                    _fireTimer = _guns.Values.ElementAt(CurrentActiveGunIndex).FireRate;
                }
                else
                {
                    _fireTimer -= Time.deltaTime;
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0) && _fireTimer <= 0)
                {
                    Shoot();
                    _fireTimer = _guns.Values.ElementAt(CurrentActiveGunIndex).FireRate;
                }
                else
                {
                    _fireTimer -= Time.deltaTime;
                }
            }

            CheckSwitchWeapon();
        }
        private void Shoot()
        {
            _guns.Values.ElementAt(CurrentActiveGunIndex).Shoot();
        }

        private void CheckSwitchWeapon()
        {
            var keysArray = _guns.Keys.ToArray();

            for (int i = 0; i < _guns.Count; i++)
            {
                if (Input.GetKeyDown(keysArray[i]) && _guns.Values.ElementAt(i).IsUnlocked)
                {
                    SwitchWeapon(i);
                }
            }
        }

        private void SwitchWeapon(int weaponIndex)
        {
            var gunArray = _guns.Values.ToArray();
            var currentGun = gunArray[CurrentActiveGunIndex];
            
            currentGun.gameObject.SetActive(false);
            if (currentGun.IsReloading)
            {
                currentGun.CancelReloadCts.Cancel();
                currentGun.CancelReloadCts.Dispose();
                currentGun.CancelReloadCts = new CancellationTokenSource();
                
                currentGun.StopReload();
            }
            
            CurrentActiveGunIndex = weaponIndex;
            
            _gunRotation.CurrentGun = _guns.Values.ElementAt(CurrentActiveGunIndex).GetComponent<SpriteRenderer>();
            gunArray[CurrentActiveGunIndex].gameObject.SetActive(true);
            
            gunArray[CurrentActiveGunIndex].OnBulletsAmountChange
                .Invoke(gunArray[CurrentActiveGunIndex].CurrentBulletsAmount);
            _gunUIImage.sprite = gunArray[CurrentActiveGunIndex].GunIconSprite;
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
    }
}