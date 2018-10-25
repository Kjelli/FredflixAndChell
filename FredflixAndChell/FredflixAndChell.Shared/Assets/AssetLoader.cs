using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using FredflixAndChell.Shared.Utilities;
using Nez.Tiled;

namespace FredflixAndChell.Shared.Assets
{
    public static class AssetLoader
    {
        private static ContentManager _content { get; set; }
        private static Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();
        private static Dictionary<string, Effect> _effects = new Dictionary<string, Effect>();
        private static Dictionary<string, SpriteFont> _fonts = new Dictionary<string, SpriteFont>();
        private static Dictionary<string, TiledMap> _maps = new Dictionary<string, TiledMap>();

        public static void Load(ContentManager cm)
        {
            _content = cm;

            // Load fonts to be used in the game
            LoadFont("font_debug");

            LoadTexture("tex_kjelli_spritesheet");
            LoadTexture("tex_rainbow");
            LoadTexture("tex_lava1");
            LoadTexture("tex_lava2");
            LoadTexture("tex_lightmask");
            LoadTexture("tex_lightmask_sm");
            LoadTexture("tex_debug");

            // Load Gameobjects 

            //Player

            //Weapons
            LoadTexture("tex_gun_m4_spritesheet");

            //Bullets
            LoadTexture("tex_standard_bullet");

            // Load effects to be used in the game
            LoadEffect("shader_flash");

            // Load maps
            LoadMap("firstlevel");
        }
        
        #region Loaders and Getters
        public static Texture2D GetTexture(string name)
        {
            return _textures[name];
        }

        public static Effect GetEffect(string name)
        {
            return _effects[name];
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
            name = name.Replace("tex_", "");

            _textures.Add(name, tex);
        }

        private static void LoadMap(string name)
        {
            var map = _content.Load<TiledMap>(name);
            //name = name.Replace("tex_", "");

            _maps.Add(name, map);
        }

        private static void LoadEffect(string name)
        {
            var shader = _content.Load<Effect>(name);
            name = name.Replace("shader_", "");

            _effects.Add(name, shader);
        }

        private static void LoadFont(string name)
        {
            var font = _content.Load<SpriteFont>(name);
            name = name.Replace("font_", "");

            _fonts.Add(name, font);
        }

        #endregion
    }
}
