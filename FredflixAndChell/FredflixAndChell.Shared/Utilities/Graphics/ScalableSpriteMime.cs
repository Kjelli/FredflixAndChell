using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;

namespace FredflixAndChell.Shared.Utilities.Graphics
{
    public class ScalableSpriteMime : SpriteMime
    {
        private Vector2 _scale = Vector2.One;
        private bool _shouldRecalculateBounds;
        private RectangleF _scaledBounds;
        private Sprite _sprite;

        public ScalableSpriteMime(Sprite spriteToMime) : base(spriteToMime)
        {
            _sprite = spriteToMime;
        }

        public override void render(Nez.Graphics graphics, Camera camera)
        {
            graphics.batcher.draw(_sprite.subtexture, entity.transform.position + localOffset, color, entity.transform.rotation, _sprite.origin, entity.transform.scale * _scale, _sprite.spriteEffects, _layerDepth);
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

        public override bool isVisibleFromCamera(Camera camera)
        {
            return true;
        }

        public void SetScale(Vector2 scale)
        {
            _scale = scale;
            _shouldRecalculateBounds = true;
        }
    }
}
