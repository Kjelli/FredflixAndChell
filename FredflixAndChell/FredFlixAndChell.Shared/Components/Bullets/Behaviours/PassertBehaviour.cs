using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FredflixAndChell.Shared.GameObjects.Bullets;
using FredflixAndChell.Shared.GameObjects.Players;
using Nez;
using FredflixAndChell.Shared.GameObjects.Weapons;

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
            if(Nez.Random.nextFloat() < 0.07f)
            {
                Bullet.Create(_bullet.Owner, _bullet.position.X, _bullet.position.Y, _bullet.Direction, BulletDict.Get("PassertBulletProc"));

                _bullet.destroy();
            }
        }
    }
}
