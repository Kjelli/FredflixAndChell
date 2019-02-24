using System;
using FredflixAndChell.Shared.Assets;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.Components.Effects
{
    public class LightSource : Component
    {
        private bool _initialized;
        private Color _lightColor;
        public Sprite LightSprite { get; set; }

        public LightSource(Color lightColor, Entity parentEntity = null)
        {
            _lightColor = lightColor;
            if(parentEntity != null)
            {
                SetupLightSprite(parentEntity);
            }
        }

        private void SetupLightSprite(Entity entity)
        {
            LightSprite = entity.addComponent(new Sprite(AssetLoader.GetTexture("effects/lightmask_xs")));
            LightSprite.material = Material.blendScreen();
            LightSprite.color = _lightColor;
            LightSprite.renderLayer = Layers.Lights;

            _initialized = true;
        }

        public override void onAddedToEntity()
        {
            if (!_initialized)
            {
                SetupLightSprite(entity);
            }
        }

        public override void onDisabled()
        {
            LightSprite.removeComponent();
        }
    }
}
