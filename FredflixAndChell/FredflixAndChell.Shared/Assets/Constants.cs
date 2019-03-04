using System;
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

        public class Values
        {
            public const float ExplosionDamage = 80f;
            public const float ExplosionRadius = 80f;
            public const float ExplosionKnockback = 20f;
            public const float ExplosionAerialKnockback = 600f;

            public const int TeamIndexBlue = 1;
            public const int TeamIndexRed = 2;
        }

        public class Strings
        {
            public const string CollisionMapEventEnter = "enter";
            public const string CollisionMapEventExit = "exit";

            public const string TiledMapGameModeKey = "mode";
            public const string TiledMapGameModeDisplayKey = "mode_display";
            public const string TiledMapTeamsKey = "teams";
            public const string TiledMapTeamsDisplayKey = "teams_display";
            public const string TiledMapMapKey = "map";
            public const string TiledMapMapDisplayKey = "map_display";
            public const string TiledMapCharacterSelectKey = "character";
            public const string TiledMapBlueTeamKey = "team_blue";
            public const string TiledMapRedTeamKey = "team_red";

            public const string EventReady = "ready";

            public const string DefaultStartCharacter = "Trump";
            public const string DefaultStartWeapon = "M4";
        }

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
            public const string CameraTracker = "camera_tracker";
            public const string Monitor = "monitor";
            public const string Zone = "zone";
        }

        public class TiledProperties
        {
            public const string BulletPassable = "bullet_passable";
            public const string EmitsLight = "emits_light";
            public const string SpawnerWeaponBlacklist = "weapon_blacklist";
            public const string SpawnerWeaponWhitelist = "weapon_whitelist";
            public const string SpawnerRarityBlacklist = "rarity_blacklist";
            public const string SpawnerRarityWhitelist = "rarity_whitelist";
            public const string SpawnerMinIntervalSeconds = "min_interval_seconds";
            public const string SpawnerMaxIntervalSeconds = "max_interval_seconds";
            public const string SpawnerMaxSpawns = "max_spawns";
            public const string SpawnerCameraTracking = "camera_tracking";
            public const string CaptureTheFlagBlueCollisionZone = "ctf_blue";
            public const string CaptureTheFlagRedCollisionZone = "ctf_red";
            public const string CaptureTheFlagBlueZone = "team_blue_zone";
            public const string CaptureTheFlagRedZone = "team_red_zone";
            public const string MapStartWeapon = "start_weapon";
            public const string PlayerSpawnTeamIndex = "team_index";
        }

        public class Tags
        {
            public const int Player = 1;
            public const int Pit = 2;
            public const int Collectible = 3;
            public const int EventEmitter = 4;
            public const int Obstacle = 5;
            public const int ReadyZone = 6;
            public const int Explosion = 7;
        }

        public class Layers
        {
            public const int Explosion = 18;
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
