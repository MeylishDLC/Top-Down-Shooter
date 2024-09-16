using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Weapons.Test_Gun
{
    public class TestGun: MonoBehaviour, IShooting
    {
        public event Action<int> OnBulletsAmountChange;
        public bool IsUnlocked { get; set; }
        [field:SerializeField] public int BulletsAmount { get; set; }
        [field: Range(0.01f, 1f)] [field:SerializeField] public float FireRate { get; set; }
        
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Animator firingPointAnimator;
        [SerializeField] private int reloadTimeMilliseconds = 1000;
        [SerializeField] private TMP_Text reloadingText;
   
        private static readonly int shoot = Animator.StringToHash("shoot");

        private int currentBulletsAmount;
        private bool canShoot = true;

        private void Start()
        {
            currentBulletsAmount = BulletsAmount;
        }

        public void Shoot()
        {
            if (canShoot && currentBulletsAmount > 0)
            {
                HandleShooting();
                currentBulletsAmount--;
                OnBulletsAmountChange?.Invoke(currentBulletsAmount);
            }
            
            if (canShoot && currentBulletsAmount == 0)
            {
                canShoot = false;
                Debug.Log("Reload");
                ReloadAsync(CancellationToken.None).Forget();
            }
        }

        private void HandleShooting()
        {
            firingPointAnimator.SetTrigger(shoot);
            var firingPointTransform = firingPointAnimator.transform;
            Instantiate(bulletPrefab, firingPointTransform.position, firingPointTransform.rotation);
        }

        private async UniTask ReloadAsync(CancellationToken token)
        {
            reloadingText.gameObject.SetActive(true);
            await UniTask.Delay(reloadTimeMilliseconds, cancellationToken: token);
            reloadingText.gameObject.SetActive(false);
            currentBulletsAmount = BulletsAmount;
            canShoot = true;
        }
        
    }
}