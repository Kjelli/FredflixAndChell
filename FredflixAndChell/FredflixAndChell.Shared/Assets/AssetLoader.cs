using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using FredflixAndChell.Shared.Utilities;

namespace FredflixAndChell.Shared.Assets
{
    public static class AssetLoader
    {
        private static ContentManager _content { get; set; }
        private static Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();
        private static Dictionary<string, Effect> _effects = new Dictionary<string, Effect>();
        private static Dictionary<string, SpriteFont> _fonts = new Dictionary<string, SpriteFont>();

        public static void Load(ContentManager cm)
        {
            _content = cm;

            // Load fonts to be used in the game
            LoadFont("debugfont");

            // Load textures to be used in the game
            LoadTexturesFromSheet("pig", 20, 20, 6, 6);
            LoadTexture("rainbow");

            // Load effects to be used in the game
            LoadEffect("shader");
        }

        private static void LoadTexturesFromSheet(string name, int width, int height, int xCount, int yCount)
        {
            var texture = _content.Load<Texture2D>(name);
            var textures = TextureSplitter.Split(texture, width, height, out int xCountActual, out int yCountActual);
            texture.Dispose();

            if (xCount != xCountActual) Console.WriteLine($"WARNING: Expected {xCount} textures, got {xCountActual}");
            if (yCount != yCountActual) Console.WriteLine($"WARNING: Expected {yCount} textures, got {yCountActual}");

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
            _textures.Add(name, _content.Load<Texture2D>(name));
        }

        private static void LoadEffect(string name)
        {
            _effects.Add(name, _content.Load<Effect>(name));
        }

        private static void LoadFont(string name)
        {
            _fonts.Add(name, _content.Load<SpriteFont>(name));
        }
    }
}
