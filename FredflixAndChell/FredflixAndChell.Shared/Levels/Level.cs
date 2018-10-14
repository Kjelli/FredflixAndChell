
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;

namespace FredflixAndChell.Shared.Levels
{
    public class Level
    {
        private TiledMap Map { get; set; }

        public Level()
        {
            Map = new TiledMap("firstlevel.tmx", 60, 30, 8, 8, TiledMapTileDrawOrder.RightDown, TiledMapOrientation.Orthogonal);
        }

        public void Draw(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            for (var index = 0; index < Map.TileLayers.Count; index++)
            {
                // TODO
                //DrawLayer(index);
            }
        }

        private void DrawLayer(int index, SpriteBatch batch)
        {
            // TODO
            /*
            var layer = Map.TileLayers[index];
            var layerHeight = layer.Height;
            var layerWidth = layer.Width;
            for (var y = 0; y < layerHeight; y++)
            {
                for (var x = 0; x < layerWidth; x++)
                {
                    var tile = layer.Tiles[x + y * layerWidth];
                    var region = tile.GlobalIdentifier == 0 ? null : _regions[tile.GlobalIdentifier];

                    if (region != null)
                    {
                        var tx = tile.X * Map.TileWidth;
                        var ty = tile.Y * Map.TileHeight;
                        var sourceRectangle = region.Bounds;
                        var destinationRectangle = new Rectangle(tx, ty, region.Width, region.Height);

                        _spriteBatch.Draw(region.Texture, destinationRectangle, sourceRectangle, Color.White);
                    }
                }
            }
            */
        }

        public void Update(GameTime gameTime)
        {
        }
    }
}
