namespace Effects
{
    public class ReducedEnemySpeed : ActiveEffect
    {
        public ReducedEnemySpeed() : base(EffectType.ReducedEnemySpeed, 5f, "Half Enemy Speed") {
            IsHaunt = false;
        }
    }
}