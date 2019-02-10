using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using Nez.Tweens;
using System.Collections.Generic;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.GameObjects.Props
{
    public class Zone : Entity
    {
        private static readonly Vector2 ZoneBorderEffectVelocity = new Vector2(0, -10);

        private bool _glow;
        private Color _color;
        private Vector2 _size;

        private Sprite _groundSprite;
        private Sprite _lightSprite;

        private Texture2D _zoneBorderTexture;
        private Effect _zoneLightShader;
        private Effect _zoneGroundShader;

        private readonly List<ZoneBorderEffect> _activeEffects;

        public Color ZoneColor
        {
            get => _color;
            set
            {
                _color = value;
                _zoneGroundShader?.Parameters["zoneColor"].SetValue(value.ToVector4());
                _zoneLightShader?.Parameters["zoneColor"].SetValue(value.ToVector4());
                foreach (var effect in (_activeEffects ?? new List<ZoneBorderEffect>()))
                {
                    effect.UpdateColor(value);
                }
            }
        }

        public Zone(Vector2 size, Color color = new Color(), bool glow = false)
        {
            _glow = glow;
            _size = size;

            _activeEffects = new List<ZoneBorderEffect>();

            SetupGroundSprite();

            if (_glow)
            {
                SetupLightSprite();
                _zoneBorderTexture =
                    ZoneBorderEffectTextureGenerator.CreateTexture(_size, 1);
            }

            scale = size / 16f;
            ZoneColor = color;

        }

        private void SetupGroundSprite()
        {
            _groundSprite = new TiledSprite(AssetLoader.GetTexture("effects/zone_1"))
            {
                textureScale = new Vector2(1 / 4f),
            };
            _groundSprite.origin = Vector2.Zero;
            _groundSprite.renderLayer = Layers.MapObstacles;
            _groundSprite.color = ZoneColor;

            _zoneGroundShader = Assets.AssetLoader.GetEffect("zone_shader");
            var zoneTexture = Assets.AssetLoader.GetTexture("effects/zone_1");

            _zoneGroundShader.Parameters["zone_texture"].SetValue(zoneTexture);
            _zoneGroundShader.Parameters["zoneColor"].SetValue(ZoneColor.ToVector4());
            _zoneGroundShader.Parameters["colorIntensity"].SetValue(0.5f);
            _zoneGroundShader.Parameters["flashRate"].SetValue(2f);
            _zoneGroundShader.Parameters["flashAmount"].SetValue(1.0f);
            _zoneGroundShader.Parameters["flashOffset"].SetValue(0.25f);
            _zoneGroundShader.Parameters["scrollSpeed"].SetValue(new Vector2(0.45f, 0.45f));

            _groundSprite.material.effect = _zoneGroundShader;
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
            _lightSprite.color = ZoneColor;
            _lightSprite.material.samplerState = SamplerState.PointClamp;
            _lightSprite.material.blendState = BlendState.Additive;

            var zoneTexture = Assets.AssetLoader.GetTexture("effects/zone_1");

            _zoneLightShader = Assets.AssetLoader.GetEffect("zone_shader");

            _zoneLightShader.Parameters["zone_texture"].SetValue(zoneTexture);
            _zoneLightShader.Parameters["colorIntensity"].SetValue(0.25f);
            _zoneLightShader.Parameters["flashRate"].SetValue(2.5f);
            _zoneLightShader.Parameters["flashAmount"].SetValue(0.8f);
            _zoneLightShader.Parameters["flashOffset"].SetValue(0.9f);
            _zoneLightShader.Parameters["scrollSpeed"].SetValue(new Vector2(-0.30f, 0.30f));

            _lightSprite.material.effect = _zoneLightShader;
            addComponent(_lightSprite);
        }

        public override void update()
        {
            base.update();
            _groundSprite.material.effect.Parameters["gameTime"].SetValue(Time.time);
            if (_glow)
            {
                _lightSprite.material.effect.Parameters["gameTime"].SetValue(Time.time);
                if (Time.checkEvery(0.9f))
                {
                    SpawnBorderEffect(_zoneBorderTexture);
                }
            }
        }

        private void SpawnBorderEffect(Texture2D zoneBorderTexture)
        {
            var zoneBorderEffect = new ZoneBorderEffect(this, zoneBorderTexture, position, ZoneBorderEffectVelocity, _color);
            scene.addEntity(zoneBorderEffect);
            _activeEffects.Add(zoneBorderEffect);
        }

        private class ZoneBorderEffect : Entity
        {
            private const float FadeDuration = 3f;

            private bool _shouldBeDestroyed;

            private Zone _parent;
            private Mover _mover;
            private Sprite _sprite;
            private Vector2 _velocity;

            private ITween<Color> _colorTween;
            private Cooldown _lifespan;


            public ZoneBorderEffect(Zone zone, Texture2D zoneBorderTexture, Vector2 zonePosition, Vector2 velocity, Color color)
            {
                _parent = zone;

                _sprite = addComponent(new Sprite(zoneBorderTexture));
                _sprite.renderLayer = Layers.Lights2;
                _sprite.origin = Vector2.Zero;
                _sprite.color = color;

                _lifespan = new Cooldown(FadeDuration);
                _colorTween = _sprite.tweenColorTo(Color.Transparent, FadeDuration)
                    .setCompletionHandler(_ =>
                    {
                        _shouldBeDestroyed = true;
                    });

                _lifespan.Start();
                _colorTween.start();

                _mover = addComponent(new Mover());

                position = zonePosition;
                _velocity = velocity;


            }
            public override void update()
            {
                base.update();
                _lifespan.Update();
                _mover.move(_velocity * Time.deltaTime, out _);
                if (_shouldBeDestroyed)
                {
                    _parent.RemoveEffect(this);
                    destroy();
                }
            }

            public void UpdateColor(Color value)
            {
                _colorTween.stop(false);
                _sprite.color.R = value.R;
                _sprite.color.G = value.G;
                _sprite.color.B = value.B;
                _sprite.color.A = (byte)(FadeDuration * _lifespan.ElapsedNormalized());
                _colorTween = _sprite.tweenColorTo(Color.Transparent, FadeDuration * _lifespan.ElapsedNormalized())
                    .setCompletionHandler(_ =>
                    {
                        _shouldBeDestroyed = true;
                    });
                _colorTween.start();
            }
        }

        private void RemoveEffect(ZoneBorderEffect zoneBorderEffect)
        {
            _activeEffects.Remove(zoneBorderEffect);
        }

        private static class ZoneBorderEffectTextureGenerator
        {
            public static Texture2D CreateTexture(Vector2 size, int borderWidth)
            {
                int width = (int)size.X;
                int height = (int)size.Y;
                var color = Color.White;
                Texture2D rect = new Texture2D(Core.graphicsDevice, width, height);

                Color[] data = new Color[width * height];
                for (int i = 0; i < data.Length; ++i)
                {
                    // Left border
                    if (i % width < borderWidth)
                    {
                        data[i] = color;
                    }
                    // Top border
                    else if (i < borderWidth * width)
                    {
                        data[i] = color;
                    }
                    // Right border
                    else if (i % width > (width - borderWidth - 1))
                    {
                        data[i] = color;
                    }
                    // Bottom border
                    else if (i > (width * height) - borderWidth * width)
                    {
                        data[i] = color;
                    }
                    else
                    {
                        data[i] = Color.TransparentBlack;
                    }
                }
                rect.SetData(data);

                return rect;
            }
        }
    }
}
