using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FredflixAndChell.Shared.Components.StatusEffects;
using FredflixAndChell.Shared.GameObjects.Bullets;
using FredflixAndChell.Shared.GameObjects.Players;

namespace FredflixAndChell.Shared.Components.Bullets.Behaviours
{
    public class FireBehaviour : BulletBehaviour
    {
        public FireBehaviour(Bullet bullet) : base(bullet)
        {
            
        }

        public override void OnImpact(Player player)
        {
            base.OnImpact(player);
            if (!player.CanBeDamagedBy(_bullet)) return;

            var existingBurnEffect = player.getComponent<BurnEffect>();
            if (existingBurnEffect != null)
            {
                existingBurnEffect.Reapply();
            }
            else
            {
                player.addComponent(new BurnEffect(3f, _bullet.Owner));
            }
        }

    }
}
