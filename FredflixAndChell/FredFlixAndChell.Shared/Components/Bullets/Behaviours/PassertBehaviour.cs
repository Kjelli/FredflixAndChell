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
    public class PassertBehaviour : BulletBehaviour
    {
        public PassertBehaviour(Bullet bullet) : base(bullet)
        {

        }

        public override void OnFired()
        {
            //Change to proc
            if(Nez.Random.nextFloat() < 0.2f)
            {
                _bullet.destroy();
            }
        }
    }
}
