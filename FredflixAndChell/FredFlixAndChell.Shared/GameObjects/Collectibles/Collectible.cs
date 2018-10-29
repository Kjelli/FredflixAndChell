using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.GameObjects.Collectibles
{
    public class Collectible : GameObject
    {
        public CollectibleParameters _preset;

        private Sprite Sprite;

        private float _scaleValue = 0.0f;
        private float _maxScaleValue = 0.5f;

        private Vector2 _origin;

        public Collectible(int x, int y, CollectibleParameters preset) : base(x, y)
        {
            _preset = preset;
            _origin = new Vector2(x, y);
        }

        public override void OnDespawn()
        {
            
        }

        public override void OnSpawn()
        {
            Sprite = entity.addComponent(new Sprite(_preset.Gun.Sprite.Icon.ToSpriteAnimation(_preset.Gun.Sprite.Source).frames[0]));
            Sprite.renderLayer = Layers.Items;

            transform.setScale(0.0f);
            
        }

        public override void update()
        {
            if(_scaleValue < _maxScaleValue)
            {
                //Spawn scaling
                _scaleValue += 0.025f;
                transform.setScale(_scaleValue);
                
            }
            else
            {
                transform.position = new Vector2(_origin.X, _origin.Y + (float)Math.Sin(Time.time * 5.0f)); 
            }


        }
    }
}
