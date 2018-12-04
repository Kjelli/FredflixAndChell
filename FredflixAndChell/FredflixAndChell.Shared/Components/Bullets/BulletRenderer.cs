using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.GameObjects.Bullets;
using FredflixAndChell.Shared.GameObjects.Bullets.Sprites;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.Utilities.Graphics;
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
        private Bullet _bullet;
        private ScalableSprite<BulletAnimations> _sprite;

        public ScalableSprite<BulletAnimations> Sprite => _sprite;

        public BulletRenderer(Bullet bullet)
        {
            _bullet = bullet;
            _sprite = bullet.addComponent(SetupAnimations(_bullet.Parameters.Sprite));

        }
        public override void onAddedToEntity()
        {
            entity.updateOrder = 2;
            
            if (_bullet.Parameters.BulletType == BulletType.Entity)
            {
                SetupEntityBullet();
            }

            _sprite.play(BulletAnimations.Bullet);
        }

        private void SetupEntityBullet()
        {
            var shadow = entity.addComponent(new SpriteMime(_sprite));
            shadow.color = new Color(0, 0, 0, 80);
            shadow.material = Material.stencilRead(Stencils.EntityShadowStencil);
            shadow.renderLayer = Layers.Shadow;
            shadow.localOffset = new Vector2(1, 2);
        }

        private ScalableSprite<BulletAnimations> SetupAnimations(BulletSprite sprite)
        {
            var animations = new ScalableSprite<BulletAnimations>();

            animations.addAnimation(BulletAnimations.Bullet, sprite.Bullet.ToSpriteAnimation(sprite.Source, 16, 16));

            return animations;
        }

        public void UpdateRenderLayerDepth()
        {
            if (_sprite == null) return;
            if (_bullet.Parameters.BulletType == BulletType.Entity)
            {
                _sprite.layerDepth = 1 - (entity?.position.Y ?? 0) * Constants.RenderLayerDepthFactor;
            }
            else
            {
                _sprite.layerDepth = 1 - (_bullet.Owner.position.Y 
                    + _bullet.Owner.VerticalFacing == (int)FacingCode.UP ? -200 
                    : _bullet.Owner.VerticalFacing == (int) FacingCode.DOWN ? 200 : 0) * Constants.RenderLayerDepthFactor;
            }
        }
    }
}
