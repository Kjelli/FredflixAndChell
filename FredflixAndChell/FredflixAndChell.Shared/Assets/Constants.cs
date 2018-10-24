using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.Assets
{
    public class Constants
    {
        public class Layers
        {
            public const int Background         = 7;
            public const int MapBackground      = 6;
            public const int MapObstacles       = 5;
            public const int Shadow             = 4;
            public const int PlayerBehind       = 3;
            public const int Player             = 2;
            public const int PlayerFront        = 1;
            public const int MapForeground      = 0;
            public const int Foreground         = -1;

        }

        public class Stencils
        {
            public const int EntityShadowStencil = 1;
            public const int HiddenEntityStencil = 2;
        }
    }
}
