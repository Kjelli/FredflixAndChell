using FredflixAndChell.Shared.GameObjects.Players;

namespace FredflixAndChell.Shared.Components.StatusEffects
{
    public class DisarmEffect : StatusEffect
    {
        public DisarmEffect(float durationSeconds) : base(durationSeconds) { }

        protected override void Effect(Player player)
        {
            // nope
        }

        protected override void OnEffectAdded(Player player)
        {
            player.Disarmed = true;
        }

        protected override void OnEffectRemoved(Player player)
        {
            player.Disarmed = false;
        }
    }
}
