using Nez.Sprites;
using System;
using Nez;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.Textures;
using System.Collections.Generic;

namespace FredflixAndChell.Shared.Utilities.Graphics
{
    public class ScalableSprite : Sprite
    {
        public ScalableSprite() : base(){ }
        public ScalableSprite(Texture2D texture) : base(texture){ }
        public ScalableSprite(Subtexture subtexture) : base(subtexture) { }

        private Vector2 _scale = Vector2.One;
        public override void render(Nez.Graphics graphics, Camera camera)
        {
            graphics.batcher.draw(_subtexture, entity.transform.position + localOffset, color, entity.transform.rotation, origin, entity.transform.scale * _scale, spriteEffects, _layerDepth);
        }

        public void SetScale(Vector2 scale)
        {
            _scale = scale;
        }
    }

    public class ScalableSprite<TEnum> : Sprite<TEnum> where TEnum : struct, IComparable, IFormattable
    {
        public ScalableSprite() : base(){ }
        public ScalableSprite(Subtexture subtexture) : base(subtexture){ }

        private Vector2 _scale = Vector2.One;
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
