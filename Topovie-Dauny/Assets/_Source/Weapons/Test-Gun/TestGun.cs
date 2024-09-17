using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Player.PlayerMovement;
using Player.PlayerMovement.GunMovement;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Weapons.Test_Gun
{
    public class TestGun: MonoBehaviour, IShooting
    {
        public event Action<int> OnBulletsAmountChange;
        public bool IsUnlocked { get; set; }
        
        [field:Header("Main Settings")]
        [field:SerializeField] public bool ShootOnHold { get; set; }
        [field:SerializeField] public int BulletsAmount { get; set; }
        [field: Range(0.01f, 1f)] [field:SerializeField] public float FireRate { get; set; }
        [SerializeField] private int reloadTimeMilliseconds = 1000;
        [SerializeField] private GameObject bulletPrefab;
        
        [Header("Components")]
        [SerializeField] private Animator firingPointAnimator;
        [SerializeField] private TMP_Text reloadingText;

        [Header("Kickback")] 
        [SerializeField] private Transform kickbackTransform;
        [SerializeField] private float kickbackDistance;
        [SerializeField] private float kickbackDuration;
   
        private static readonly int shoot = Animator.StringToHash("shoot");

        private int _currentBulletsAmount;
        private bool _canShoot = true;

        private PlayerKickback _playerKickback;
        private void Awake()
        {
            _playerKickback = new PlayerKickback(kickbackDistance, kickbackDuration,transform, kickbackTransform);
            _currentBulletsAmount = BulletsAmount;
        }
        public void Shoot()
        {
            if (_canShoot && _currentBulletsAmount > 0)
            {
                HandleShooting();
                _currentBulletsAmount--;
                OnBulletsAmountChange?.Invoke(_currentBulletsAmount);
            }
            
            if (_canShoot && _currentBulletsAmount == 0)
            {
                _canShoot = false;
                ReloadAsync(CancellationToken.None).Forget();
            }
        }

        private void HandleShooting()
        {
            firingPointAnimator.SetTrigger(shoot);
            var firingPointTransform = firingPointAnimator.transform;
            Instantiate(bulletPrefab, firingPointTransform.position, firingPointTransform.rotation);
            _playerKickback.ApplyKickback(CancellationToken.None).Forget();
        }

        private async UniTask ReloadAsync(CancellationToken token)
        {
            reloadingText.gameObject.SetActive(true);
            await UniTask.Delay(reloadTimeMilliseconds, cancellationToken: token);
            reloadingText.gameObject.SetActive(false);
            _currentBulletsAmount = BulletsAmount;
            _canShoot = true;
        }
        
    }
}