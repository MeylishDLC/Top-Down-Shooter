using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Weapons
{
    public abstract class Gun: MonoBehaviour
    {
        public Action<int> OnBulletsAmountChange;
        public CancellationTokenSource CancelReloadCts = new();
        public bool IsReloading { get; protected set; }
        public int CurrentBulletsAmount { get; protected set; }
        [field:SerializeField] public int BulletsAmount { get; protected set; }
        [field:SerializeField] public float FireRate { get; protected set; }
        [field:SerializeField] public float ReloadTime { get; protected set; }
        
        [field:SerializeField] public bool IsUnlocked { get; protected set; }
        [field:SerializeField] public bool ShootOnHold { get; protected set; }
        
        [field:Header("UI")]
        [field:SerializeField] public Sprite GunIconSprite { get; protected set; }
        [field:SerializeField] public Image GunKeyImage { get; protected set; }
        public abstract void Shoot();
        public abstract void StopReload();
        protected abstract void Reload();
    }
}