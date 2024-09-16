using System;

namespace Weapons
{
    public interface IShooting
    {
        public event Action<int> OnBulletsAmountChange;
        public int BulletsAmount { get; set; }
        public float FireRate { get; set; }
        public bool IsUnlocked { get; set; }
        public void Shoot();
    }
}