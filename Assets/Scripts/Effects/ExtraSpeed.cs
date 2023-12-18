namespace Effects
{
    public class ExtraSpeed : ActiveEffect
    {
        public ExtraSpeed() : base(EffectType.ExtraSpeed, 5f, "Double Speed") {
            IsHaunt = false;
        }
    }
}