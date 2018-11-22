using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FredflixAndChell.Shared.Assets.Constants;
using Random = Nez.Random;

namespace FredflixAndChell.Shared.Components.Effects.Weather
{
    public class DungeonGloom : SceneComponent, IAtmosphere
    {
        private Texture2D _fogTexture;
        private Texture2D _crystalTexture;
        private TiledMapComponent _map;
        private int _fogId;

        public DungeonGloom()
        {
            _fogTexture = Assets.AssetLoader.GetTexture("effects/lightmask_sm");
            _crystalTexture = Assets.AssetLoader.GetTexture("particles/crystal");
        }

        public override void onEnabled()
        {
            base.onEnabled();
            _map = scene.findComponentOfType<TiledMapComponent>();
        }

        public override void update()
        {
            base.update();
            if (Time.frameCount % 2 == 0)
            {
                SpawnFog();
            }
        }

        private void SpawnFog()
        {
            var x = Random.range(0, _map.width);
            var y = Random.range(0, _map.height);
            var fogEntity = scene.createEntity($"fog{++_fogId}", new Vector2(x,y));
            fogEntity.addComponent(new Fog(_fogTexture, _crystalTexture));
        }
    }

    internal class Fog : Component, IUpdatable
    {
        private Sprite _fog;
        private Sprite _crystal;

        private float _fadeInTimeSeconds = 2.5f;
        private float _fadeOutTimeSeconds =  2.5f;
        private float _durationSeconds = 4f;

        private float _yVelocity = -8f;
        public Fog(Texture2D fogTexture, Texture2D crystalTexture)
        {
            _fog = new Sprite(fogTexture);
            _fog.color = new Color(1f, 0, 0, 0);
            _fog.renderLayer = Layers.Lights;
            _fog.material = new Material { blendState = BlendState.Additive };

            _crystal = new Sprite(crystalTexture);
            _crystal.color = new Color(1f, 0, 0, 0);
            _crystal.renderLayer = Layers.Lights2;
            _crystal.material = new Material { blendState = BlendState.AlphaBlend };
        }

        public override void onAddedToEntity()
        {
            entity.addComponent(_fog);
            entity.addComponent(_crystal);

            var scale = Random.range(0.05f, 0.2f);
            entity.scale = new Vector2(scale);

            TweenExt.tweenColorTo(_fog, new Color(1f, 0, 0, 1f), _fadeInTimeSeconds).start();
            TweenExt.tweenColorTo(_crystal, new Color(1f, 0, 0, 1f), _fadeInTimeSeconds).start();

            Core.schedule(_durationSeconds, _ =>
                TweenExt.tweenColorTo(_fog, new Color(0.1f, 0, 0, 0), _fadeOutTimeSeconds)
                .start());
            Core.schedule(_durationSeconds, _ =>
                TweenExt.tweenColorTo(_crystal, new Color(0.1f, 0, 0, 0), _fadeOutTimeSeconds)
                .setCompletionHandler(__ => entity?.destroy())
                .start());
        }

        public void update()
        {
            entity.localPosition = new Vector2(entity.localPosition.X, entity.localPosition.Y + Time.deltaTime * _yVelocity);
        }
    }
}
