using FredflixAndChell.Shared.Assets;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Tweens;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.GameObjects.Effects
{
    public class Smoke : Entity
    {
        public Smoke(Vector2 spawnPosition, bool startSmall = true)
        {
            position = spawnPosition;
            if (startSmall)
            {
                scale = new Vector2(0.15f);
            }
        }

        public override void onAddedToScene()
        {
            var texture = AssetLoader.GetTexture("effects/smoke");
            var smokeVariant = Random.range(0, texture.Width / 16);
            var sprite = new Sprite(new Nez.Textures.Subtexture(texture, new Rectangle(smokeVariant * 16, 0, 16, 16)))
            {
                material = Material.blendAdditive(),
                renderLayer = Layers.PlayerFront,
                layerDepth = 0.75f
            };

            sprite.tweenColorTo(Color.Transparent, 0.5f)
                .setEaseType(EaseType.Linear)
                .start();
            TweenExt.tweenScaleTo(this, 0.75f, 0.5f)
                .setEaseType(EaseType.Linear)
                .start();
            TweenExt.tweenRotationDegreesTo(this, 180, 0.5f)
                .setEaseType(EaseType.Linear)
                .setCompletionHandler(_ => destroy())
                .start();

            addComponent(sprite);
        }
    }
}
