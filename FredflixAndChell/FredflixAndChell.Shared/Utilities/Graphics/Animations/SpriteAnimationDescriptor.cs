using FredflixAndChell.Shared.Assets;
using Microsoft.Xna.Framework;
using Nez.Sprites;
using Nez.Textures;
using System.Linq;

namespace FredflixAndChell.Shared.Utilities.Graphics.Animations
{
    public class SpriteAnimationDescriptor
    {
        public int[] Frames { get; set; }
        public bool Loop { get; set; }
        public float FPS { get; set; }
        public Vector2 Origin { get; set; }

        public SpriteAnimation ToSpriteAnimation(string source)
        {
            var texture = AssetLoader.GetTexture(source);
            var subtextures = Subtexture.subtexturesFromAtlas(texture, 32, 32);
            if(Origin.Length() != 0) subtextures.ForEach(t => t.origin = Origin);

            var frames = Frames
                .Select(i => subtextures[i])
                .ToList();

            var animation = new SpriteAnimation
            {
                frames = frames,
                loop = Loop,
                fps = FPS
            };

            return animation;
        }
    }
}
