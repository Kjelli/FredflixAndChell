using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.Assets
{
    public class Constants
    {
        public class TileProperties
        {
            public const string BulletPassable      = "bullet_passable";
            public const string EmitsLight          = "emits_light";
        }

        public class Layers
        {
            public const int Background         = 8;
            public const int MapBackground      = 7;
            public const int MapObstacles       = 6;
            public const int Shadow             = 5;
            public const int Bullet             = 4;
            public const int PlayerBehind       = 3;
            public const int Player             = 2;
            public const int PlayerFront        = 1;
            public const int MapForeground      = 0;
            public const int Lights             = -1;
            public const int Lights2             = -2;
            public const int Foreground         = -3;
        }

        public class Stencils
        {
            public const int EntityShadowStencil = 1;
            public const int HiddenEntityStencil = 2;
        }
    }
}
