namespace Effects
{
    public class Invincibility : ActiveEffect
    {
        public Invincibility() : base(EffectType.Invincible, 5f, "Invincible") {
            IsHaunt = false;
        }
    }
}