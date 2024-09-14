namespace Weapons
{
    public interface IShooting
    {
        public float FireRate { get; set; }
        public bool IsUnlocked { get; set; }
        public void Shoot();
    }
}