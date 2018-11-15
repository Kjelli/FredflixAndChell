using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nez.Tiled;
using System.Collections.Generic;

namespace FredflixAndChell.Shared.Assets
{
    public static class AssetLoader
    {
        private static ContentManager _content;

        private static Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();
        private static Dictionary<string, Effect> _effects = new Dictionary<string, Effect>();
        private static Dictionary<string, SpriteFont> _fonts = new Dictionary<string, SpriteFont>();
        private static Dictionary<string, TiledMap> _maps = new Dictionary<string, TiledMap>();

        public static void LoadLoadingScene(ContentManager cm)
        {
            _content = cm;
            LoadTexture("textures/effects/rainbow");
        }

        public static void LoadBroScene(ContentManager cm)
        {
            _content = cm;

            // Load fonts to be used in the game
            LoadFont("fonts/debug");

            LoadTexture("textures/players/tormod_body");
            LoadTexture("textures/players/tormod_head");
            LoadTexture("textures/players/kjelli_body");
            LoadTexture("textures/players/kjelli_head");
            LoadTexture("textures/effects/lava1");
            LoadTexture("textures/effects/lava2");
            LoadTexture("textures/effects/lightmask");
            LoadTexture("textures/effects/lightmask_sm");
            LoadTexture("textures/effects/lightmask_xs");
            LoadTexture("textures/guns/m4");
            LoadTexture("textures/guns/fido");
            LoadTexture("textures/guns/pewpew");
            LoadTexture("textures/bullets/fido");
            LoadTexture("textures/bullets/standard");
            LoadTexture("textures/bullets/shockwave");
            LoadTexture("textures/bullets/shockwave_sm");

            LoadTexture("textures/particles/blood");


            LoadEffect("effects/shader_flash");

            LoadMap("maps/firstlevel");
            LoadMap("maps/winter_1");
            LoadMap("maps/dungeon_1");
            LoadTexture("maps/spawner_tile");

            
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
