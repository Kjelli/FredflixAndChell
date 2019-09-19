using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.GameObjects.Effects;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.Utilities;
using FredflixAndChell.Shared.Utilities.Graphics;
using Microsoft.Xna.Framework;
using Nez;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.Components.StatusEffects
{
    public class BurnEffect : StatusEffect
    {
        private float _initialSpeed;
        private float _initialDeaccelerationFactor;
        private float _initialAccelerationFactor;

        private Entity _effectEntity;

        public BurnEffect(float durationSeconds, Player sourceOfStatus) : base(durationSeconds, 0.5f, sourceOfStatus) { }

        protected override void Effect(Player player)
        {
            player.Damage(new DirectionalDamage
            {
                SourceOfDamage = _sourceOfStatus,
                Damage = 2.5f,
            });
        }

        public override void update()
        {
            base.update();
            if (Time.checkEvery(0.15f))
            {
                var offset = new Vector2(Random.nextFloat() * 16 - 8, Random.nextFloat() * 16 - 8);
                var fireball = new BurnEffectParticle(_affectedPlayer.position + offset);
                fireball.setScale(new Vector2(0.1f));
                _affectedPlayer.scene.addEntity(fireball);
            }
        }

        protected override void OnEffectAdded(Player player)
        {

        }

        protected override void OnEffectRemoved(Player player)
        {
        }

    }
}
