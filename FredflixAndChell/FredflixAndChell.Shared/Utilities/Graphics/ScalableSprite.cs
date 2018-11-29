using Nez.Sprites;
using System;
using Nez;
using Microsoft.Xna.Framework;

namespace FredflixAndChell.Shared.Utilities.Graphics
{
    public class ScalableSprite<TEnum> : Sprite<TEnum> where TEnum : struct, IComparable, IFormattable
    {
        private Vector2 _scale;
        public override void render(Nez.Graphics graphics, Camera camera)
        {
            graphics.batcher.draw(_subtexture, entity.transform.position + localOffset, color, entity.transform.rotation, origin, entity.transform.scale * _scale, spriteEffects, _layerDepth);

        }

        public void SetScale(Vector2 scale)
        {
            _scale = scale;
        }
    }
}
