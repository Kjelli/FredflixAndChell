using FredflixAndChell.Shared.GameObjects.Players;

namespace FredflixAndChell.Shared.Components.StatusEffects
{
    public class RegenEffect : StatusEffect
    {
        protected override void Effect(Player player)
        {
            if (player.PlayerState == PlayerState.Normal && player.Health < player.MaxHealth)
            {
                player.Health += 0.001f * player.MaxHealth;
            }
        }

        protected override void OnEffectAdded(Player player)
        {
            // nope
        }

        protected override void OnEffectRemoved(Player player)
        {
            // nope
        }
    }
}
