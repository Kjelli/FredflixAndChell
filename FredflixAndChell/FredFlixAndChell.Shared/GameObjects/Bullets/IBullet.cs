using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.GameObjects.Bullets
{
    interface IBullet : IGameObject
    {

        float Damage { get; set; }
        float Direction { get; set; }
        float Speed { get; set; }

    }
}
