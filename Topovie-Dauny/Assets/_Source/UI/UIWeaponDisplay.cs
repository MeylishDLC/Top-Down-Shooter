using System;
using System.Collections.Generic;
using System.Linq;
using Player.PlayerCombat;
using Player.PlayerMovement;
using TMPro;
using UnityEngine;
using Weapons;
using Zenject;

namespace UI
{
    public class UIWeaponDisplay: MonoBehaviour
    {
        [SerializeField] private TMP_Text bulletAmountText;

        private PlayerWeapons _playerWeapons;
        
        [Inject]
        public void Construct(PlayerMovement playerMovement)
        {
            _playerWeapons = playerMovement.gameObject.GetComponent<PlayerWeapons>();
        }
        private void Start()
        {
            SubscribeOnEvents();
            var currentWeapon = _playerWeapons.Weapons.ElementAt(_playerWeapons.CurrentActiveWeaponIndex);
            RefreshBulletsText(currentWeapon.BulletsAmount);
        }

        private void OnDestroy()
        {
            UnsubscribeOnEvents();
        }

        private void RefreshBulletsText(int bulletsAmount)
        {
            bulletAmountText.text = bulletsAmount.ToString();
        }
        private void SubscribeOnEvents()
        {
            foreach (var weapon in _playerWeapons.Weapons)
            {
                weapon.OnBulletsAmountChange += RefreshBulletsText;
            }
        }
        private void UnsubscribeOnEvents()
        {
            foreach (var weapon in _playerWeapons.Weapons)
            {
                weapon.OnBulletsAmountChange -= RefreshBulletsText;
            }
        }
    }
}