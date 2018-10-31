using System;
using FredflixAndChell.Shared.GameObjects.Bullets;
using FredflixAndChell.Shared.Utilities;
using FredflixAndChell.Shared.Components.Guns;
using Microsoft.Xna.Framework;
using FredflixAndChell.Shared.GameObjects.Players;
using Nez;

namespace FredflixAndChell.Shared.GameObjects.Weapons
{
    public class Gun : GameObject
    {
        private GunParameters _params;
        private GunRenderer _renderer;
        private Player _player;

        private int Ammo;
        private int MaxAmmo;
        private int MagazineAmmo;
        private int MagazineSize;

        private int _bulletCount;
        private float _bulletSpread;

        private Vector2 BarrelOffset;

        public Cooldown Cooldown { get; set; }
        public Cooldown Reload { get; set; }
        public GunParameters Parameters => _params;

        public Gun(Player owner, GunParameters gunParameters) : base(0, 0)
        {
            _player = owner;
            _params = gunParameters;
            SetupParameters();
        }

        public override void OnSpawn()
        {
            Cooldown.Start();
            _renderer = entity.addComponent(new GunRenderer(this, _player));
        }

        private void SetupParameters()
        {
            Ammo = _params.Ammo;
            MaxAmmo = _params.MaxAmmo;
            MagazineAmmo = _params.MagazineAmmo;
            MagazineSize = _params.MagazineSize;
            BarrelOffset = _params.BarrelOffset;
            Cooldown = new Cooldown(_params.FireRate);
            Reload = new Cooldown(_params.ReloadTime);

            _bulletCount = _params.BulletCount;
            _bulletSpread = _params.BulletSpread;
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
                    var bulletSpawnX = entity.position.X + (float)Math.Cos(_player.FacingAngle) * BarrelOffset.X + (float)Math.Cos(_player.FacingAngle) * BarrelOffset.Y;
                    var bulletSpawnY = entity.position.Y + (float)Math.Sin(_player.FacingAngle) * BarrelOffset.Y + (float)Math.Sin(_player.FacingAngle) * BarrelOffset.X;
                    for (float i = 0; i < _bulletCount; i++)
                    {
                        var bulletEntity = entity.scene.createEntity("bullet");
                        bulletEntity.addComponent(
                            new Bullet(_player,
                                bulletSpawnX,
                                bulletSpawnY,
                                _player.FacingAngle + ((i * 2 - _bulletCount) * _bulletSpread / _bulletCount),
                                _params.BulletParameters));
                    }
                    MagazineAmmo--;

                    //Animation
                    _renderer?.Fire();
                }
            }
        }

        private void CheckAmmo()
        {
            if (MagazineAmmo == 0)
            {
                if (Ammo <= 0)
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
                _renderer.Reload();
            }
        }

        public override void OnDespawn()
        {
        }

        public void SetRenderLayer(int renderLayer)
        {
            _renderer?.setRenderLayer(renderLayer);
        }

        public override void update()
        {
            Cooldown.Update();
            Reload.Update();
        }

        public void FlipY(bool isFlipped)
        {
            _renderer?.FlipY(isFlipped);
        }

        public void Destroy()
        {
            entity.destroy();
        }
    }
}
