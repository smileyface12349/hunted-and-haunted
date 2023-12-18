namespace Effects
{
    public class ExtraDamage: ActiveEffect
    {
        public ExtraDamage() : base(EffectType.ExtraDamage, 5f, "Double Damage") {
            IsHaunt = false;
        }
    }
}