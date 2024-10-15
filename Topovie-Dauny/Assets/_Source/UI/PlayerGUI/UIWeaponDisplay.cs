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
        
        [Inject]
        public void Construct(PlayerMovement playerMovement)
        {
            _playerEquipment = playerMovement.gameObject.GetComponent<PlayerEquipment>();
        }
        private void Start()
        {
            SubscribeOnEvents();
            var currentWeapon = _playerEquipment.PlayerWeaponsSetter.Guns.
                ElementAt(_playerEquipment.PlayerWeaponsSetter.CurrentActiveGunIndex);
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
            foreach (var weapon in _playerEquipment.PlayerWeaponsSetter.Guns)
            {
                weapon.OnBulletsAmountChange += RefreshBulletsText;
            }
        }
        private void UnsubscribeOnEvents()
        {
            foreach (var weapon in _playerEquipment.PlayerWeaponsSetter.Guns)
            {
                weapon.OnBulletsAmountChange -= RefreshBulletsText;
            }
        }
    }
}