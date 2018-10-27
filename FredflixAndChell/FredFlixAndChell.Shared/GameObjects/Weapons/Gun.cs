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
            BarrelOffset = new Vector2(10, 0);
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
            var texture = AssetLoader.GetTexture("guns/m4");
            var subtextures = Subtexture.subtexturesFromAtlas(texture, 32, 32);
            subtextures.ForEach(t => t.origin = new Vector2(10, 16));

            animations.addAnimation(Animations.Held_Idle, new SpriteAnimation(new List<Subtexture>()
            {
                subtextures[1 + 0 * 8],
            })
            {
                loop = true,
                fps = 5
            });


            animations.addAnimation(Animations.Held_Fired, new SpriteAnimation(new List<Subtexture>()
            {
                subtextures[1 + 1 * 8],
                subtextures[1 + 2 * 8],
                subtextures[1 + 3 * 8],
                subtextures[1 + 0 * 8],
            })
            {
                loop = false,
                fps = 5
            });

            animations.addAnimation(Animations.Reload, new SpriteAnimation(new List<Subtexture>()
            {
                subtextures[1 + 0 * 8],
                subtextures[2 + 0 * 8],
                subtextures[2 + 1 * 8],
                subtextures[2 + 2 * 8],
                subtextures[2 + 3 * 8],
                subtextures[2 + 4 * 8],
                subtextures[2 + 3 * 8],
                subtextures[2 + 2 * 8],
                subtextures[2 + 1 * 8],
                subtextures[2 + 0 * 8],
                subtextures[1 + 0 * 8],
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
                CheckAmmo();
                if (Cooldown.IsReady() && Reload.IsReady() && MagazineAmmo >= 0)
                {
                    //Functionality
                    Cooldown.Start();
                    var bulletEntity = entity.scene.createEntity("bullet");
                    var bulletSpawnX = entity.position.X + (float) Math.Cos(_player.FacingAngle) * BarrelOffset.X + (float)Math.Cos(_player.FacingAngle) * BarrelOffset.Y;
                    var bulletSpawnY = entity.position.Y + (float) Math.Sin(_player.FacingAngle) * BarrelOffset.Y + (float)Math.Sin(_player.FacingAngle) * BarrelOffset.X;
                    bulletEntity.addComponent(new Bullet(_player, bulletSpawnX, bulletSpawnY, 1, 1, _player.FacingAngle, 200f, 30f));
                    MagazineAmmo--;

                    //Animation
                    _animation?.play(Animations.Held_Fired);
                }
            }
        }

        private void CheckAmmo()
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
            entity.setScale(0.6f);

            _animation = entity.addComponent(SetupAnimations());
            _animation.renderLayer = Layers.PlayerFrontest;

            var shadow = entity.addComponent(new SpriteMime(_animation));
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

            Cooldown.Start();
            _animation.play(Animations.Held_Idle);
        }

        public void SetRenderLayer(int renderLayer)
        {
            _animation?.setRenderLayer(renderLayer);
        }

        public override void update()
        {
            _animation.flipY = flipY;
            var offset = 7;
            entity.position = new Vector2(_player.entity.position.X + (float)Math.Cos(_player.FacingAngle)*offset,
                _player.entity.position.Y + (float)Math.Sin(_player.FacingAngle) * offset/2);

            Cooldown.Update();
            Reload.Update();
        }
    }
}
