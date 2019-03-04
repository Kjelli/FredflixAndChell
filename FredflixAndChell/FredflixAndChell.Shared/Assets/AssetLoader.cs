using FredflixAndChell.Shared.GameObjects.Weapons;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nez.Tiled;
using System.Collections.Generic;
using System.Linq;

namespace FredflixAndChell.Shared.Assets
{
    public static class AssetLoader
    {
        private static ContentManager _content;

        private static Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();
        private static Dictionary<string, Effect> _effects = new Dictionary<string, Effect>();
        private static Dictionary<string, SpriteFont> _fonts = new Dictionary<string, SpriteFont>();
        private static Dictionary<string, TiledMap> _maps = new Dictionary<string, TiledMap>();

        public static void LoadBroScene(ContentManager cm)
        {
            _content = cm;

            // Load fonts to be used in the game
            LoadFont("fonts/debug");
            LoadFont("fonts/monitor");

            LoadPlayerTexture("textures/players/trump");
            LoadPlayerTexture("textures/players/doge");
            LoadPlayerTexture("textures/players/masschin");
            LoadPlayerTexture("textures/players/kjelli");
            LoadPlayerTexture("textures/players/tormod");

            LoadTexture("textures/effects/lava1");
            LoadTexture("textures/effects/lava2");
            LoadTexture("textures/effects/lightmask");
            LoadTexture("textures/effects/lightmask_sm");
            LoadTexture("textures/effects/lightmask_xs");
            LoadTexture("textures/effects/inverted_lightmask_xs");
            LoadTexture("textures/effects/zone_1");
            LoadTexture("textures/effects/smoke");
            LoadTexture("textures/effects/explosion");
            LoadTexture("textures/effects/fireball");
            LoadTexture("textures/effects/spawn_ring");

            LoadTexture("textures/guns/m4");
            LoadTexture("textures/guns/fido");
            LoadTexture("textures/guns/pewpew");
            LoadTexture("textures/guns/fidgetspinner");
            LoadTexture("textures/guns/goggles");
            LoadTexture("textures/guns/bazooka");
            LoadTexture("textures/guns/stick");
            LoadTexture("textures/guns/flag");
            LoadTexture("textures/guns/chainsaw");
            LoadTexture("textures/guns/passertGun");
            LoadTexture("textures/guns/flamethrower");

            LoadTexture("textures/bullets/fido");
            LoadTexture("textures/bullets/standard");
            LoadTexture("textures/bullets/shockwave");
            LoadTexture("textures/bullets/shockwave_sm");
            LoadTexture("textures/bullets/fidgetspinner");
            LoadTexture("textures/bullets/laser");
            LoadTexture("textures/bullets/rocket");
            LoadTexture("textures/bullets/fireball");

            LoadTexture("textures/UI/HUD");
            LoadTexture("textures/UI/screen_bg");

            LoadTexture("textures/particles/blood");
            LoadTexture("textures/particles/crystal");

            LoadTexture("textures/statuseffects/slow");

            LoadTexture("maps/spawner_tile");

            LoadEffect("effects/shader_flash");
            LoadEffect("effects/zone_shader");
            LoadEffect("effects/grayscale_shader");
            LoadEffect("effects/weapon_hand_color");

            LoadMap("maps/ctf_x");
            LoadMap("maps/winter_hub");
            LoadMap("maps/winter_debug");
            LoadMap("maps/winter_1");
            LoadMap("maps/dungeon_1");
            LoadMap("maps/dungeon_2");
            LoadMap("maps/fido_fighters");
            LoadMap("maps/fido_fighters2");
            LoadMap("maps/ctf_snow");
            LoadMap("maps/ctf_snow2");
            LoadMap("maps/stickfight");


            Bullets.LoadFromData();
            Guns.LoadFromData();
            Melees.LoadFromData();
        }

        public static List<string> GetMaps()
        {
            return _maps.Keys.ToList();
        }

        public static List<KeyValuePair<string, string>> GetMapsWithDisplayName()
        {
            return _maps.Select(m => new KeyValuePair<string, string>(m.Key, m.Value.properties["name"])).ToList();
        }

        private static void LoadPlayerTexture(string playerDirectory)
        {
            LoadTexture(playerDirectory + "/head");
            LoadTexture(playerDirectory + "/torso");
            LoadTexture(playerDirectory + "/legs");
        }

        public static void Dispose()
        {
            _content.Dispose();

            foreach (var texture in _textures.Values) texture.Dispose();
            _textures.Clear();
            foreach (var effect in _effects.Values) effect.Dispose();
            _effects.Clear();

            _fonts.Clear();
            _maps.Clear();
        }

        #region Loaders and Getters
        public static Texture2D GetTexture(string name)
        {
            return _textures[name];
        }

        public static Effect GetEffect(string name)
        {
            return _effects[name].Clone();
        }

        public static SpriteFont GetFont(string name)
        {
            return _fonts[name];
        }


        public static TiledMap GetMap(string name)
        {
            return _maps[name];
        }


        private static void LoadTexture(string name)
        {
            var tex = _content.Load<Texture2D>(name);
            name = name.Replace("textures/", "");

            _textures[name] = tex;
        }

        private static void LoadMap(string name)
        {
            var map = _content.Load<TiledMap>(name);
            name = name.Replace("maps/", "");

            _maps[name] = map;
        }

        private static void LoadEffect(string name)
        {
            var shader = _content.Load<Effect>(name);
            name = name.Replace("effects/", "");

            _effects[name] = shader;
        }

        private static void LoadFont(string name)
        {
            var font = _content.Load<SpriteFont>(name);
            name = name.Replace("fonts/", "");

            _fonts[name] = font;
        }

        #endregion


    }
    public static class ParticleDesigner
    {
        public const string flame = @"textures/particles/Blue_Flame";
    }
}
