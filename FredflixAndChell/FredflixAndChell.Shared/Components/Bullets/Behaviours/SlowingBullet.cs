using FredflixAndChell.Shared.Components.StatusEffects;
using FredflixAndChell.Shared.GameObjects.Bullets;
using FredflixAndChell.Shared.GameObjects.Players;

namespace FredflixAndChell.Shared.Components.Bullets.Behaviours
{
    public class SlowingBullet : BulletBehaviour
    {
        public SlowingBullet(Bullet bullet) : base(bullet)
        {
        }
        public override void OnImpact(Player player)
        {
            base.OnImpact(player);
            if (player == _bullet.Owner) return;

            var existingSlowEffect = player.getComponent<SlowEffect>();
            if (existingSlowEffect != null)
            {
                existingSlowEffect.Reapply();
            }
            else
            {
                player.addComponent(new SlowEffect(2f));
            }

            var existingDisarmEffect = player.getComponent<DisarmEffect>();
            if (existingDisarmEffect != null)
            {
                existingDisarmEffect.Reapply();
            }
            else
            {
                player.addComponent(new DisarmEffect(2f));
            }
        }
    }
}
