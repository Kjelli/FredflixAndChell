using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Utilities.Graphics;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.GameObjects
{
    public class Monitor : Entity
    {
        private Vector2 _size;
        private Text _textComponent;
        private Entity _textEntity;

        public string Text
        {
            get => _textComponent?.text ?? null;
            set
            {
                _textComponent?.setText(value);
            }
        }

        public Monitor(Vector2 position, Vector2 size, string setKey = "", string text = "")
        {
            this.position = position;
            _size = size;
            AddTextToScreen(text);
            AddBackgroundToScreen();
        }

        public override void onAddedToScene()
        {
            AddLightsToScreen();
            scene.addEntity(_textEntity);
        }

        private void AddTextToScreen(string text)
        {
            _textEntity = new Entity();
            _textEntity.setScale(1/8f);
            _textEntity.position = position;
            var font = Assets.AssetLoader.GetFont("monitor");
            var spritefont = new NezSpriteFont(font);
            _textComponent = new Text(spritefont, text, new Vector2(_size.X / 2, 4), Color.White)
            {
                horizontalOrigin = HorizontalAlign.Center
            };
            _textEntity.addComponent(_textComponent);
        }

        private void AddBackgroundToScreen()
        {
            var screenTexture = AssetLoader.GetTexture("UI/screen_bg");
            var sprite = addComponent(new ScalableSprite(screenTexture));
            sprite.renderLayer = Layers.MapObstacles;
            sprite.material = Materials.ReflectionMaterial;
            sprite.material.effect = new ReflectionEffect
            {
                normalMap = AssetLoader.GetTexture("effects/lava2")
            };
            sprite.setOrigin(Vector2.Zero);
            sprite.SetScale(new Vector2(_size.X / 8, _size.Y / 8));
        }

        private void AddLightsToScreen()
        {
            for (var x = position.X; x < position.X + _size.X; x += 8)
            {
                for (var y = position.Y; y < position.Y + _size.Y; y += 8)
                {
                    var lightEntity = scene.createEntity("screen-light", new Vector2(x + 4, y + 4));
                    lightEntity.setScale(0.5f);

                    var sprite = lightEntity.addComponent(new Sprite(AssetLoader.GetTexture("effects/lightmask_xs")));
                    sprite.material = Material.blendLinearDodge();
                    sprite.color = Color.Wheat;
                    sprite.renderLayer = Layers.Lights;
                }
            }
        }

    }
}