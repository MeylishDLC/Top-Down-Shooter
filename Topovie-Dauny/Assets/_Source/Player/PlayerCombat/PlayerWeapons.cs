using System;
using System.Collections.Generic;
using System.Linq;
using DialogueSystem;
using UnityEngine;
using Weapons;
using Zenject;

namespace Player.PlayerCombat
{
    public class PlayerWeapons: MonoBehaviour
    {
        public IEnumerable<IShooting> Weapons => _weapons; 
        public int CurrentActiveWeaponIndex { get; private set; }
        
        [SerializeField] private SerializedDictionary<KeyCode, GameObject> weaponsObjects;
        
        private List<IShooting> _weapons;
        private float _fireTimer;
        private DialogueManager _dialogueManager;

        [Inject]
        public void Construct(DialogueManager dialogueManager)
        {
            _dialogueManager = dialogueManager;
        }

        private void Awake()
        {
            _weapons = new List<IShooting>();
            GetIShootingComponent();
        }

        private void Start()
        {
            CurrentActiveWeaponIndex = GetActiveWeaponIndex();
        }

        private void Update()
        {
            if (!_dialogueManager.DialogueIsPlaying)
            {
                HandleShooting();
            }
        }

        private void HandleShooting()
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
            
            CheckSwitchWeapon();
        }
        private void Shoot()
        {
            _weapons[CurrentActiveWeaponIndex].Shoot();
        }
        private void CheckSwitchWeapon()
        {
            var keysArray = weaponsObjects.Keys.ToArray();
            
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
            var weaponsObjectsArray = weaponsObjects.Values.ToArray();
            weaponsObjectsArray[CurrentActiveWeaponIndex].SetActive(false);
            
            CurrentActiveWeaponIndex = weaponIndex;
            weaponsObjectsArray[CurrentActiveWeaponIndex].SetActive(true);
        }
        private void GetIShootingComponent()
        {
            var weaponObjectsValuesArray = weaponsObjects.Values.ToArray();
            
            for (int i = 0; i < weaponsObjects.Count; i++)
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