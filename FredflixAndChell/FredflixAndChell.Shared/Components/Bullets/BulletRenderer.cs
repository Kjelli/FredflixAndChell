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

namespace FredflixAndChell.Shared.Components.Bullets
{
    public class BulletRenderer : Component
    {
        private BulletSprite _sprite;

        public BulletRenderer(BulletSprite sprite)
        {
            _sprite = sprite;
        }
        public override void onAddedToEntity()
        {
            base.onAddedToEntity();

            var sprite = entity.addComponent(new Sprite(AssetLoader.GetTexture($"bullets/{_sprite}")));
            sprite.renderLayer = Layers.Bullet;

            var shadow = entity.addComponent(new SpriteMime(sprite));
            shadow.color = new Color(0, 0, 0, 80);
            shadow.material = Material.stencilRead(Stencils.EntityShadowStencil);
            shadow.renderLayer = Layers.Shadow;
            shadow.localOffset = new Vector2(1, 2);
            entity.setScale(0.25f);
        }
    }
}
