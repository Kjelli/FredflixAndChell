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
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.GameObjects.Weapons
{
    public class Gun : GameObject, IWeapon
    {
        private List<Subtexture> _subtextures;
        private Texture2D _texture;
        private Sprite _sprite;
        private Player _player;

        public int Damage { get; set; }
        public float Speed { get; set; }
        public Vector2 Offset { get; set; }
        public Vector2 BarrelOffset { get; set; }
        public bool flipY { get; set; }

        public int Ammo { get; set; }
        public int MaxAmmo { get; set; }
        public int MagazineSize { get; set; }
        public int MagazineAmmo { get; set; }

        public Cooldown Cooldown { get; set; }
        public Cooldown Reload { get; set; }

        public float ReloadTime { get; set; }


        public Gun(Player owner, int x, int y, float cooldown) : base(x, y, 64, 64)
        {
            var texture = AssetLoader.GetTexture("gun_m4_spritesheet");
            _subtextures = Subtexture.subtexturesFromAtlas(texture, 32, 32);

            _texture = owner != null ? _subtextures[0] : _subtextures[1];

            BarrelOffset = new Vector2(y, x + 10);
            _player = owner;

            //TODO: Dynamicly set ammo
            MagazineSize = 30;
            MagazineAmmo = 30;
            MaxAmmo = 1200;
            Ammo = 6000;
            ReloadTime = 1f;


            Cooldown = new Cooldown(cooldown);
            Reload = new Cooldown(ReloadTime);
        }

        enum Animations
        {
            Held_Idle,
            Held_Fired,
            Reload
        }

        Sprite<Animations> _animation;

        private Sprite<Animations> SetupAnimations()
        {
            var animations = new Sprite<Animations>();

            animations.addAnimation(Animations.Held_Idle, new SpriteAnimation(new List<Subtexture>()
            {
                _subtextures[1 + 0 * 8],
            })
            {
                loop = true,
                fps = 5
            });


            animations.addAnimation(Animations.Held_Fired, new SpriteAnimation(new List<Subtexture>()
            {
                _subtextures[1 + 1 * 8],
                _subtextures[1 + 2 * 8],
                _subtextures[1 + 3 * 8],
                _subtextures[1 + 0 * 8],
            })
            {
                loop = false,
                fps = 5
            });

            animations.addAnimation(Animations.Reload, new SpriteAnimation(new List<Subtexture>()
            {
                _subtextures[1 + 0 * 8],
                _subtextures[2 + 0 * 8],
                _subtextures[2 + 1 * 8],
                _subtextures[2 + 2 * 8],
                _subtextures[2 + 3 * 8],
                _subtextures[2 + 4 * 8],
                _subtextures[2 + 3 * 8],
                _subtextures[2 + 2 * 8],
                _subtextures[2 + 1 * 8],
                _subtextures[2 + 0 * 8],
                _subtextures[1 + 0 * 8],
            })
            {
                loop = false,
                fps = 15
            });


            return animations;
        }

        public void Fire()
        {
            if (!Reload.IsOnCooldown())
            {
                checkAmmo();
                if (Cooldown.IsReady() && Reload.IsReady() && MagazineAmmo >= 0)
                {
                    //Functionality
                    Cooldown.Start();
                    var bulletEntity = entity.scene.createEntity("bullet");
                    bulletEntity.addComponent(new Bullet(_player, entity.position.X, entity.position.Y, 4, 4, _player.FacingAngle, 200 + .0f, 30.0f));
                    MagazineAmmo--;

                    //Animation
                    _animation.play(Animations.Held_Fired);
                }
            }
        }

        private void checkAmmo()
        {
            if(MagazineAmmo == 0){
                if(Ammo <= 0)
                {
                    //Totally out of ammo? 
                    //TODO: Throw away this
                }
                else
                {
                    //Reload
                    ReloadMagazine();
                }
            }
        }

        public void ReloadMagazine()
        {
            if (!Reload.IsOnCooldown() && MagazineAmmo != MagazineSize)
            {   
                //Function
                Reload.Start();
                int newBullets = Math.Min(MagazineSize - MagazineAmmo, Ammo);
                Ammo = Ammo - newBullets;
                MagazineAmmo = MagazineAmmo + newBullets;
                //Animation
                _animation.play(Animations.Reload);
            }
        }



        public override void OnDespawn()
        {
        }


        public override void OnSpawn()
        {
            var shadow = entity.addComponent(new SpriteMime(_sprite));
            shadow.color = new Color(0, 0, 0, 80);
            shadow.material = Material.stencilRead(Stencils.EntityShadowStencil);
            shadow.renderLayer = Layers.Shadow;
            shadow.localOffset = new Vector2(1, 2);

            // Assign silhouette component when gun is visually blocked
            var silhouette = entity.addComponent(new SpriteMime(_animation));
            silhouette.color = new Color(0, 0, 0, 80);
            silhouette.material = Material.stencilRead(Stencils.HiddenEntityStencil);
            silhouette.renderLayer = Layers.Foreground;
            silhouette.localOffset = new Vector2(0, 0);

            entity.setScale(0.6f);

            _animation = entity.addComponent(SetupAnimations());
            _animation.renderLayer = Layers.PlayerFront;

            Cooldown.Start();
            _animation.play(Animations.Held_Idle);
        }

        public override void update()
        {
            _animation.flipY = flipY;
            //TODO: Dont hardcore this shit 
            var offset = _player.HorizontalFacing == 2 ? 7 : -7;
            entity.position = new Vector2(_player.entity.position.X + offset, _player.entity.position.Y + 2);

            Cooldown.Update();
            Reload.Update();
        }
    }
}
