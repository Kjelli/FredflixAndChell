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
        public class TiledObjects
        {
            public const string TiledMapEntity = "TiledMapEntity";
            public const string ObjectGroup = "Objects";

            public const string Collision = "collision";
            public const string PlayerSpawn = "player_spawn";
            public const string ItemSpawn = "item_spawn";
            public const string Pit = "pit";
            public const string EventEmitter = "event_emitter";
        }

        public class TiledProperties
        {
            public const string BulletPassable = "bullet_passable";
            public const string EmitsLight = "emits_light";
        }

        public class Tags
        {
            public const int Player = 1;
            public const int Pit = 2;
            public const int Collectible = 3;
            public const int EventEmitter = 4;
            public const int Obstacle = 5;
        }

        public class Layers
        {
            public const int Background = 17;
            public const int MapBackground = 16;
            public const int MapObstacles = 15;
            public const int Interactables = 14;
            public const int Shadow = 13;
            public const int Bullet = 12;
            public const int PlayerBehindest = 11;
            public const int PlayerBehind = 10;
            public const int Player = 9;
            public const int PlayerFront = 8;
            public const int PlayerFrontest = 7;
            public const int MapForeground = 6;
            public const int Lights = 5;
            public const int Lights2 = 4;
            public const int Foreground = 3;
            public const int Weather = 2;
            public const int HUD = 1;
        }

        public class Stencils
        {
            public const int EntityShadowStencil = 1;
            public const int HiddenEntityStencil = 2;
        }
    }
}
