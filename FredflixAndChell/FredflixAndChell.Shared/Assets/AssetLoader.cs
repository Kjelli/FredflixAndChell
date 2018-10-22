using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using FredflixAndChell.Shared.Utilities;
using FredflixAndChell.Shared.Utilities.Graphics;
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

            // Load textures to be used in the game
            LoadTexturesFromSheet("sheet_pig", 20, 20, 6, 6);
            LoadTexturesFromSheet("sheet_kjelli",32,32,8,1);

            LoadTexture("tex_rainbow");
            LoadTexture("tex_lava1");
            LoadTexture("tex_lava2");
            LoadTexture("tex_lightmask");
            LoadTexture("tex_debug");

            // Load Gameobjects 

            //Player

            //Weapons
            LoadTexture("tex_gun_m4");

            //Bullets
            LoadTexture("tex_standard_bullet");

            // Load effects to be used in the game
            LoadEffect("shader_flash");
        }
       
        private static void LoadTexturesFromSheet(string name, int width, int height, int xCount, int yCount)
        {
            var texture = _content.Load<Texture2D>(name);
            var textures = TextureSplitter.Split(texture, width, height, out int xCountActual, out int yCountActual);
            texture.Dispose();

            if (xCount != xCountActual) Console.WriteLine($"WARNING: Expected {xCount} columns, got {xCountActual}");
            if (yCount != yCountActual) Console.WriteLine($"WARNING: Expected {yCount} rows, got {yCountActual}");

            name = name.Replace("sheet_", "");

            for (var y = 0; y < yCountActual; y++)
            {
                for (var x = 0; x < xCountActual; x++)
                {
                    _textures.Add($"{name}[{x}][{y}]", textures[x + y * xCountActual]);
                }
            }
        }

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
    }
}
