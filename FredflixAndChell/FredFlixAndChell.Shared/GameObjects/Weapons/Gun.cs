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
using Nez.Textures;

namespace FredflixAndChell.Shared.GameObjects.Weapons
{
    public class Gun : GameObject, IWeapon
    {
        public int Damage { get; set; }
        public float Speed { get; set; }
        public Vector2 Offset { get; set; }
        public Vector2 BarrelOffset { get; set; }
        public bool flipY { get; set; }

        private List<Subtexture> _subtextures;


        private Texture2D _texture;
        private Sprite _sprite;

        private Player _player;

        public Cooldown Cooldown { get; set; }


        public Gun(Player owner, int x, int y, float cooldown) : base(x, y, 64, 64)
        {
            var texture = AssetLoader.GetTexture("gun_m4_spritesheet");
            _subtextures = Subtexture.subtexturesFromAtlas(texture, 32, 32);

            _texture = owner != null ? _subtextures[0] : _subtextures[1];

            BarrelOffset = new Vector2(y, x + 10);
            _player = owner;

            
            Cooldown = new Cooldown(cooldown);
        }

        enum Animations
        {
            Held_Idle,
            Held_Fired
        }

        Sprite<Animations> _animation;

        private Sprite<Animations> SetupAnimations()
        {
            var animations = new Sprite<Animations>();

            animations.addAnimation(Animations.Held_Idle, new SpriteAnimation(new List<Subtexture>()
            {
                _subtextures[1 + 0 * 8],
            }));


            animations.addAnimation(Animations.Held_Fired, new SpriteAnimation(new List<Subtexture>()
            {
                _subtextures[1 + 1 * 8],
                _subtextures[1 + 2 * 8],
                _subtextures[1 + 3 * 8],
                _subtextures[1 + 0 * 8],
            }));

            Vector2 kuk = new Vector2(80,0);
            animations.setOrigin(kuk);
            return animations;
        }

        public void Fire()
        {
            if (Cooldown.IsReady())
            {
                Cooldown.Start();
                var bulletEntity = entity.scene.createEntity("bullet");
                bulletEntity.addComponent(new Bullet(_player, entity.position.X, entity.position.Y, 4, 4, _player.FacingAngle, 150+.0f, 30.0f));
                
                _animation.play(Animations.Held_Fired)
                    .setLoop(false)
                    .setFps(10f)
                    .prepareForUse();
            }

        }

        public override void OnDespawn()
        {
        }


        public override void OnSpawn()
        {
            

            var shadow = entity.addComponent(new SpriteMime(_sprite));
            shadow.color = new Color(0, 0, 0, 255);
            shadow.material = Material.defaultMaterial;
            shadow.renderLayer = 1;
            shadow.localOffset = new Vector2(1, 2);

            entity.setScale(0.75f);

            _animation = entity.addComponent(SetupAnimations());
            _animation.renderLayer = -2;

            Cooldown.Start();
            _animation.play(Animations.Held_Idle)
                .setLoop(false);
        }

        public override void update()
        {
            _animation.flipY = flipY;
            entity.position = _player.entity.position;
            Cooldown.Update();
        }
    }
}
