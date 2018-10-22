using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.GameObjects.Bullets;
using FredflixAndChell.Shared.Scenes;
using FredflixAndChell.Shared.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.Sprites;
using Nez;

namespace FredflixAndChell.Shared.GameObjects.Weapons
{
    public class Gun : GameObject, IWeapon
    {
        public int Damage { get; set; }
        public float Speed { get; set; }
        public Vector2 Offset { get; set; }
        public Vector2 BarrelOffset { get; set; }
        public bool flipY { get; set; }

        private Texture2D _texture;
        private Sprite _sprite;

        private Player _player;

        public Cooldown Cooldown { get; set; }


        public Gun(Player owner, int x, int y, float cooldown) : base(x, y, 32, 32)
        {
            _texture = AssetLoader.GetTexture("gun_m4");

            BarrelOffset = new Vector2(y, x + 10);
            _player = owner;

            Cooldown = new Cooldown(cooldown);
        }

        public void Fire()
        {
            if (Cooldown.IsReady())
            {
                var bulletEntity = entity.scene.createEntity("bullet");
                bulletEntity.addComponent(new Bullet(_player, entity.position.X, entity.position.Y, 4, 4, _player.FacingAngle, 40.0f, 30.0f));

                Cooldown.Start();
            }

        }

        public override void OnDespawn()
        {
        }


        public override void OnSpawn()
        {
            _sprite = entity.addComponent(new Sprite(_texture));
            _sprite.renderLayer = -3;

            var shadow = entity.addComponent(new SpriteMime(_sprite));
            shadow.color = new Color(0, 0, 0, 255);
            shadow.material = Material.defaultMaterial;
            shadow.renderLayer = 1;
            shadow.localOffset = new Vector2(1, 2);

            entity.setScale(0.25f);
        }

        public override void update()
        {
            _sprite.flipY = flipY;
            entity.position = _player.entity.position;
            Cooldown.Update();
        }
    }
}
