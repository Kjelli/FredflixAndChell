﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.Assets
{
    public class Constants
    {
        public const int ScreenWidth = 1280;
        public const int ScreenHeight = 720;
        public const float RenderLayerDepthFactor = 1 / 10000f;

        public class Assets
        {
            public const string DataDirectory = "Content/data";
        }
        public class TileProperties
        {
            public const string BulletPassable = "bullet_passable";
            public const string EmitsLight = "emits_light";
        }

        public class Tags
        {
            public const int Player = 1;
            public const int Pit = 2;
            public const int Collectible = 3;
        }

        public class Layers
        {
            public const int Background = 15;
            public const int MapBackground = 14;
            public const int MapObstacles = 13;
            public const int Items = 12;
            public const int Shadow = 11;
            public const int Bullet = 10;
            public const int PlayerBehindest = 9;
            public const int PlayerBehind = 8;
            public const int Player = 7;
            public const int PlayerFront = 6;
            public const int PlayerFrontest = 5;
            public const int MapForeground = 4;
            public const int Lights = 3;
            public const int Lights2 = 2;
            public const int Foreground = 1;
        }

        public class Stencils
        {
            public const int EntityShadowStencil = 1;
            public const int HiddenEntityStencil = 2;
        }
    }
}
