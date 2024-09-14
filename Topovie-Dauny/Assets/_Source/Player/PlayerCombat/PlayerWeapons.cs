using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Weapons;

namespace Player.PlayerCombat
{
    public class PlayerWeapons: MonoBehaviour
    { 
        [SerializeField] private SerializedDictionary<KeyCode, GameObject> weaponsObjects;

        private List<IShooting> weapons;
        private int currentActiveWeaponIndex;
        private float fireTimer;

        private void Start()
        {
            weapons = new();
            GetIShootingComponent();
            currentActiveWeaponIndex = GetActiveWeaponIndex();
        }

        private void Update()
        {
            if (Input.GetMouseButton(0) && fireTimer <= 0)
            {
                Shoot();
                fireTimer = weapons[currentActiveWeaponIndex].FireRate;
            }
            else
            {
                fireTimer -= Time.deltaTime;
            }
            
            CheckSwitchWeapon();
        }
        private void Shoot()
        {
            weapons[currentActiveWeaponIndex].Shoot();
        }
        private void CheckSwitchWeapon()
        {
            var keysArray = weaponsObjects.Keys.ToArray();
            
            for (int i = 0; i < weapons.Count; i++)
            {
                if (Input.GetKeyDown(keysArray[i]) && weapons[i].IsUnlocked)
                {
                    SwitchWeapon(i);
                }
            }
        }

        private void SwitchWeapon(int weaponIndex)
        {
            var weaponsObjectsArray = weaponsObjects.Values.ToArray();
            weaponsObjectsArray[currentActiveWeaponIndex].SetActive(false);
            
            currentActiveWeaponIndex = weaponIndex;
            weaponsObjectsArray[currentActiveWeaponIndex].SetActive(true);
        }
        private void GetIShootingComponent()
        {
            var weaponObjectsValuesArray = weaponsObjects.Values.ToArray();
            
            for (int i = 0; i < weaponsObjects.Count; i++)
            {
                if (weaponObjectsValuesArray[i].TryGetComponent(out IShooting weapon))
                {
                    weapons.Add(weapon);
                }
                else
                {
                    throw new Exception("Couldn't get the IShooting component from the gun.");
                }
            }
        }

        private int GetActiveWeaponIndex()
        {
            var weaponObjectsValuesArray = weaponsObjects.Values.ToArray();
            
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