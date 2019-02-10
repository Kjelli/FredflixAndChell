using FredflixAndChell.Shared.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.GameObjects.Props
{
    public class Zone : Entity
    {
        private bool _glow;
        private Color _color;

        private Sprite _groundSprite;
        private Sprite _lightSprite;

        public Zone(Vector2 size, Color color = new Color(), bool glow = false)
        {
            _color = color;
            _glow = glow;

            SetupGroundSprite();

            if (_glow)
            {
                SetupLightSprite();
            }
            scale = size / 16f;
        }

        private void SetupGroundSprite()
        {
            _groundSprite = new TiledSprite(AssetLoader.GetTexture("effects/zone_1"))
            {
                textureScale = new Vector2(1 / 4f),
            };
            _groundSprite.origin = Vector2.Zero;
            _groundSprite.renderLayer = Layers.MapObstacles;
            _groundSprite.color = _color;

            var zoneShader = Assets.AssetLoader.GetEffect("zone_shader");
            var zoneTexture = Assets.AssetLoader.GetTexture("effects/zone_1");

            zoneShader.Parameters["zone_texture"].SetValue(zoneTexture);
            zoneShader.Parameters["zoneColor"].SetValue(_color.ToVector4());
            zoneShader.Parameters["colorIntensity"].SetValue(0.5f);
            zoneShader.Parameters["flashRate"].SetValue(2f);
            zoneShader.Parameters["flashAmount"].SetValue(0.8f);
            zoneShader.Parameters["flashOffset"].SetValue(0.9f);
            zoneShader.Parameters["scrollSpeed"].SetValue(new Vector2(0.45f, 0.45f));

            _groundSprite.material.effect = zoneShader;
            addComponent(_groundSprite);

        }
        private void SetupLightSprite()
        {
            _lightSprite = new TiledSprite(AssetLoader.GetTexture("effects/zone_1"))
            {
                textureScale = new Vector2(1 / 4f),
            };
            _lightSprite.origin = Vector2.Zero;
            _lightSprite.renderLayer = Layers.Lights;
            _lightSprite.color = _color;
            _lightSprite.material.samplerState = SamplerState.PointClamp;
            _lightSprite.material.blendState = BlendState.Additive;

            var zoneShader = Assets.AssetLoader.GetEffect("zone_shader");
            var zoneTexture = Assets.AssetLoader.GetTexture("effects/zone_1");

            zoneShader.Parameters["zone_texture"].SetValue(zoneTexture);
            zoneShader.Parameters["zoneColor"].SetValue(_color.ToVector4());
            zoneShader.Parameters["colorIntensity"].SetValue(0.25f);
            zoneShader.Parameters["flashRate"].SetValue(2.5f);
            zoneShader.Parameters["flashAmount"].SetValue(0.8f);
            zoneShader.Parameters["flashOffset"].SetValue(0.9f);
            zoneShader.Parameters["scrollSpeed"].SetValue(new Vector2(-0.30f, 0.30f));

            _lightSprite.material.effect = zoneShader;
            addComponent(_lightSprite);
        }

        public override void update()
        {
            base.update();
            _groundSprite.material.effect.Parameters["gameTime"].SetValue(Time.time);
            _lightSprite.material.effect.Parameters["gameTime"].SetValue(Time.time);

            if (Time.checkEvery(3f))
            {
                
            }
        }

        private class ZoneBorderEffect : Sprite
        {
            public ZoneBorderEffect(Vector2 size, int borderWidth)
            {
                Texture2D rect = new Texture2D(Core.graphicsDevice, (int)size.X, (int)size.Y);

                Color[] data = new Color[80 * 30];
                for (int i = 0; i < data.Length; ++i) data[i] = Color.Chocolate;
                rect.SetData(data);

            }
        }
    }
}
