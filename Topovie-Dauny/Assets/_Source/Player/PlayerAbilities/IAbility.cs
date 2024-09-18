namespace Player.PlayerAbilities
{
    public interface IAbility
    {
        public int CooldownMilliseconds { get; set; }
        public void UseAbility();
    }
}