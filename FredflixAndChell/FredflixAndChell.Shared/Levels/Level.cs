
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.Tiled;

namespace FredflixAndChell.Shared.Levels
{
    public class Level
    {
        private TiledMap Map { get; set; }

        public Level()
        {
            Nez.Tiled.
            Setup();
        }

        private void Setup()
        {
            var width = Map.Width;
            var height = Map.Height;

            foreach (var layer in Map.Layers)
            {
                Console.WriteLine($"Layer: {layer.Name}");
            }
            foreach (var tileLayer in Map.TileLayers)
            {
                Console.WriteLine($"TileLayer: {tileLayer.Name} (Tile dimension: {tileLayer.TileWidth}x{tileLayer.Width})");
                Console.WriteLine($"Width: {tileLayer.Width}");
                Console.WriteLine($"Height: {tileLayer.Height}");
                Console.WriteLine($"Expecting {tileLayer.Width * tileLayer.Height} tiles");
                Console.WriteLine($"Got {tileLayer.Tiles.Count} tiles");
                foreach (var tile in tileLayer.Tiles)
                {
                    Console.WriteLine($"++ GID: {tile.GlobalIdentifier}, FLAGS: {tile.Flags}");
                }
            }
        }

        public void Draw(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //for (var index = 0; index < Map.TileLayers.Count; index++)
            //{
            //    // TODO
            //    DrawLayer(index);
            //}
        }

        private void DrawLayer(int index, SpriteBatch batch)
        {
            // TODO

            //var layer = Map.TileLayers[index];
            //var layerHeight = layer.Height;
            //var layerWidth = layer.Width;
            //for (var y = 0; y < layerHeight; y++)
            //{
            //    for (var x = 0; x < layerWidth; x++)
            //    {
            //        var tile = layer.Tiles[x + y * layerWidth];
            //        var region = tile.GlobalIdentifier == 0 ? null : _regions[tile.GlobalIdentifier];

            //        if (region != null)
            //        {
            //            var tx = tile.X * Map.TileWidth;
            //            var ty = tile.Y * Map.TileHeight;
            //            var sourceRectangle = region.Bounds;
            //            var destinationRectangle = new Rectangle(tx, ty, region.Width, region.Height);

            //            _spriteBatch.Draw(region.Texture, destinationRectangle, sourceRectangle, Color.White);
            //        }
            //    }
            //}

        }

        public void Update(GameTime gameTime)
        {
        }
    }
}
