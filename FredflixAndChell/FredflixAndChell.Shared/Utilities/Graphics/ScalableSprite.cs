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
        public ScalableSprite() : base() {
            _scaledBounds = new RectangleF(bounds.location, bounds.size);
        }
        public ScalableSprite(Texture2D texture) : base(texture) {
            _scaledBounds = new RectangleF(bounds.location, bounds.size);
        }
        public ScalableSprite(Subtexture subtexture) : base(subtexture) {
            _scaledBounds = new RectangleF(bounds.location, bounds.size);
        }

        private Vector2 _scale = Vector2.One;
        private bool _shouldRecalculateBounds;
        private RectangleF _scaledBounds;

        public override void render(Nez.Graphics graphics, Camera camera)
        {
            graphics.batcher.draw(_subtexture, entity.transform.position + localOffset, color, entity.transform.rotation, origin, entity.transform.scale * _scale, spriteEffects, _layerDepth);
        }

        public override RectangleF bounds
        {
            get
            {
                if (_shouldRecalculateBounds)
                {
                    _scaledBounds = new RectangleF(base.bounds.location, base.bounds.size * _scale);
                    _shouldRecalculateBounds = false;
                }
                return _scaledBounds;
            }
        }

        public void SetScale(Vector2 scale)
        {
            _scale = scale;
            _shouldRecalculateBounds = true;
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

        public Sprite tweenColorTo(int v)
        {
            throw new NotImplementedException();
        }
    }
}
