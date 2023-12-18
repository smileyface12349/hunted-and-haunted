namespace Effects
{
    public class PiercingBullets : ActiveEffect
    {
        public PiercingBullets() : base(EffectType.PiercingBullets, 5f, "Piercing Bullets") {
            IsHaunt = false;
        }
    }
}