using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Weapons;

namespace Player.PlayerCombat
{
    public class PlayerWeapons
    {
        public int CurrentActiveWeaponIndex { get; private set; }
        public IEnumerable<IShooting> Weapons => _weapons;

        private SerializedDictionary<KeyCode, GameObject> _weaponObjects;
        private List<IShooting> _weapons;
        private float _fireTimer;
        private bool _canShoot = true;
        
        public PlayerWeapons(SerializedDictionary<KeyCode, GameObject> weaponObjects)
        {
            _weaponObjects = weaponObjects;
            _weapons = new List<IShooting>();
            GetIShootingComponent();
            CurrentActiveWeaponIndex = GetActiveWeaponIndex();
        }
        public void SetCanShoot(bool canShoot)
        {
            _canShoot = canShoot;
        }
        public void HandleShooting()
        {
            if (!_canShoot)
            {
                return;
            }
            
            if (_weapons[CurrentActiveWeaponIndex].ShootOnHold)
            {
                if (Input.GetMouseButton(0) && _fireTimer <= 0)
                {
                    Shoot();
                    _fireTimer = _weapons[CurrentActiveWeaponIndex].FireRate;
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
                    _fireTimer = _weapons[CurrentActiveWeaponIndex].FireRate;
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
            _weapons[CurrentActiveWeaponIndex].Shoot();
        }

        private void CheckSwitchWeapon()
        {
            var keysArray = _weaponObjects.Keys.ToArray();

            for (int i = 0; i < _weapons.Count; i++)
            {
                if (Input.GetKeyDown(keysArray[i]) && _weapons[i].IsUnlocked)
                {
                    SwitchWeapon(i);
                }
            }
        }

        private void SwitchWeapon(int weaponIndex)
        {
            var weaponsObjectsArray = _weaponObjects.Values.ToArray();
            weaponsObjectsArray[CurrentActiveWeaponIndex].SetActive(false);

            CurrentActiveWeaponIndex = weaponIndex;
            weaponsObjectsArray[CurrentActiveWeaponIndex].SetActive(true);
        }

        private void GetIShootingComponent()
        {
            var weaponObjectsValuesArray = _weaponObjects.Values.ToArray();

            for (int i = 0; i < _weaponObjects.Count; i++)
            {
                if (weaponObjectsValuesArray[i].TryGetComponent(out IShooting weapon))
                {
                    _weapons.Add(weapon);
                }
                else
                {
                    throw new Exception("Couldn't get the IShooting component from the gun.");
                }
            }
        }

        private int GetActiveWeaponIndex()
        {
            var weaponObjectsValuesArray = _weaponObjects.Values.ToArray();

            for (int i = 0; i < weaponObjectsValuesArray.Length; i++)
            {
                if (weaponObjectsValuesArray[i].activeSelf)
                {
                    return i;
                }
            }

            throw new Exception("No weapon was active");
        }
    }
}