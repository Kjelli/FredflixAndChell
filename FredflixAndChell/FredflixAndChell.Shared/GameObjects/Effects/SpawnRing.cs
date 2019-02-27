using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Components.Effects;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Tweens;

namespace FredflixAndChell.Shared.GameObjects.Effects
{
    public class SpawnRing : Entity
    {
        private Sprite _sprite;
        private bool _shouldBeDestroyed;

        public SpawnRing(Vector2 spawnPosition, Color color)
        {
            position = spawnPosition;
            scale = new Vector2(0.15f);
            var texture = AssetLoader.GetTexture("effects/spawn_ring");
            _sprite = addComponent(new Sprite(texture));
            _sprite.color = color;

            var lightSource = addComponent(new LightSource(new Color(color, 1.0f), this));
            lightSource.LightSprite.tweenColorTo(Color.Black, 0.5f)
                .setEaseType(EaseType.SineOut)
                .start();
        }

        public override void onAddedToScene()
        {
            _sprite.tweenColorTo(Color.Transparent, 0.5f)
                .setEaseType(EaseType.SineOut)
                .start();
            this.tweenScaleTo(1.25f, 0.5f)
                .setEaseType(EaseType.SineOut)
                .setCompletionHandler(_ => _shouldBeDestroyed = true)
                .start();
        }

        public override void update()
        {
            base.update();
            if (_shouldBeDestroyed) destroy();
        }
    }
}
