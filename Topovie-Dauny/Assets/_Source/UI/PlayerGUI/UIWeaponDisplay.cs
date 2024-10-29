using System.Linq;
using Player.PlayerAbilities;
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
        private WeaponsSetter _weaponsSetter;
        
        [Inject]
        public void Construct(WeaponsSetter weaponsSetter)
        {
            _weaponsSetter = weaponsSetter;
        }
        private void Start()
        {
            SubscribeOnEvents();
            var currentWeapon = _weaponsSetter.Guns.
                ElementAt(_weaponsSetter.CurrentActiveGunIndex);
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
            foreach (var weapon in _weaponsSetter.Guns)
            {
                weapon.OnBulletsAmountChange += RefreshBulletsText;
            }
        }
        private void UnsubscribeOnEvents()
        {
            foreach (var weapon in _weaponsSetter.Guns)
            {
                weapon.OnBulletsAmountChange -= RefreshBulletsText;
            }
        }
    }
}