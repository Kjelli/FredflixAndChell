using System;
using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.Utilities.Graphics;
using Microsoft.Xna.Framework;
using Nez;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.Components.StatusEffects
{
    public class SlowEffect : StatusEffect
    {
        private float _initialSpeed;
        private float _initialDeaccelerationFactor;
        private float _initialAccelerationFactor;

        private Entity _effectEntity;

        public SlowEffect(float durationSeconds) : base(durationSeconds) { }

        protected override void Effect(Player player)
        {
            player.Speed = _initialSpeed * 0.25f;
            player.DeaccelerationExternalFactor = 0.5f;
            player.AccelerationExternalFactor = 0.5f;
            _effectEntity.position = player.position;
            _effectEntity.rotation += 0.1f;
        }

        protected override void OnEffectAdded(Player player)
        {
            _initialSpeed = player.Speed;
            _initialDeaccelerationFactor = player.DeaccelerationExternalFactor;
            _initialAccelerationFactor = player.AccelerationExternalFactor;

            _effectEntity = player.scene.createEntity("");
            _effectEntity.setPosition(player.position);
            var sprite = _effectEntity.addComponent(
                new ScalableSprite(AssetLoader.GetTexture("statuseffects/slow")));
            sprite.SetScale(new Vector2(0.5f));
            sprite.renderLayer = Layers.PlayerFront;
            sprite.setLocalOffset(new Vector2(0,-10f));
        }

        protected override void OnEffectRemoved(Player player)
        {
            player.Speed = _initialSpeed;
            player.DeaccelerationExternalFactor = _initialDeaccelerationFactor;
            player.AccelerationExternalFactor = _initialAccelerationFactor;
            _effectEntity.destroy();
        }

    }
}
