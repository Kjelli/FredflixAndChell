using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.GameObjects.Bullets;
using FredflixAndChell.Shared.GameObjects.Bullets.Sprites;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FredflixAndChell.Shared.Assets.Constants;
using static FredflixAndChell.Shared.GameObjects.Bullets.Sprites.BulletSprite;

namespace FredflixAndChell.Shared.Components.Bullets
{
    public class BulletRenderer : Component
    {
        private BulletSprite _bulletSprite;
        private Sprite<BulletAnimations> _sprite;

        public BulletRenderer(BulletSprite sprite)
        {
            _bulletSprite = sprite;
        }
        public override void onAddedToEntity()
        {
            base.onAddedToEntity();

            _sprite = entity.addComponent(SetupAnimations(_bulletSprite));
            _sprite.renderLayer = Layers.Player;

            var shadow = entity.addComponent(new SpriteMime(_sprite));
            shadow.color = new Color(0, 0, 0, 80);
            shadow.material = Material.stencilRead(Stencils.EntityShadowStencil);
            shadow.renderLayer = Layers.Shadow;
            shadow.localOffset = new Vector2(1, 2);

            _sprite.play(BulletAnimations.Bullet);
        }

        private Sprite<BulletAnimations> SetupAnimations(BulletSprite sprite)
        {
            var animations = new Sprite<BulletAnimations>();

            animations.addAnimation(BulletAnimations.Bullet, sprite.Bullet.ToSpriteAnimation(sprite.Source, 16, 16));

            return animations;
        }

        public void UpdateRenderLayerDepth()
        {
            if (_sprite == null) return;
            _sprite.layerDepth = 1 - (entity?.position.Y ?? 0) * Constants.RenderLayerDepthFactor;
        }
    }
}
