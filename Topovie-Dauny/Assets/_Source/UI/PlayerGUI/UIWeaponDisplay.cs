using System.Linq;
using Player.PlayerCombat;
using Player.PlayerControl;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI.PlayerGUI
{
    public class UIWeaponDisplay: MonoBehaviour
    {
        [SerializeField] private TMP_Text bulletAmountText;

        private PlayerEquipment _playerEquipment;
        private PlayerWeaponsSetter _playerWeaponsSetter;
        
        [Inject]
        public void Construct(PlayerWeaponsSetter playerWeaponsSetter)
        {
            _playerWeaponsSetter = playerWeaponsSetter;
        }
        private void Start()
        {
            SubscribeOnEvents();
            var currentWeapon = _playerWeaponsSetter.Guns.
                ElementAt(_playerWeaponsSetter.CurrentActiveGunIndex);
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
            foreach (var weapon in _playerWeaponsSetter.Guns)
            {
                weapon.OnBulletsAmountChange += RefreshBulletsText;
            }
        }
        private void UnsubscribeOnEvents()
        {
            foreach (var weapon in _playerWeaponsSetter.Guns)
            {
                weapon.OnBulletsAmountChange -= RefreshBulletsText;
            }
        }
    }
}