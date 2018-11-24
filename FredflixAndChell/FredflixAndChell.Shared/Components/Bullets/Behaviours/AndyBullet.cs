using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FredflixAndChell.Shared.GameObjects.Bullets;
using FredflixAndChell.Shared.GameObjects.Players;
using Nez;
using Microsoft.Xna.Framework;

namespace FredflixAndChell.Shared.Components.Bullets.Behaviours
{
    public class AndyBullet : BulletBehaviour
    {
        public AndyBullet(Bullet bullet) : base(bullet) { }

        public override void Move()
        {
            _mover.move(new Vector2(Nez.Random.minusOneToOne(), Nez.Random.minusOneToOne()));
        }

        public override void OnImpact(Player player)
        {
            base.OnImpact(player);
        }

        public override void OnNonPlayerImpact(CollisionResult collision)
        {
            base.OnNonPlayerImpact(collision);
        }
    }
}
