using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FredflixAndChell.Shared.GameObjects.Bullets;
using FredflixAndChell.Shared.GameObjects.Players;
using Nez;

namespace FredflixAndChell.Shared.Components.Bullets.Behaviours
{
    public class StandardBullet : BulletBehaviour
    {
        public StandardBullet(Bullet bullet) : base(bullet) { }
    }
}
