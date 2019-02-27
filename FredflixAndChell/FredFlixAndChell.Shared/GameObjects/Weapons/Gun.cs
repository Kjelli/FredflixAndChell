using FredflixAndChell.Shared.Components.Weapons;
using FredflixAndChell.Shared.GameObjects.Bullets;
using FredflixAndChell.Shared.GameObjects.Collectibles;
using FredflixAndChell.Shared.GameObjects.Collectibles.Metadata;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.GameObjects.Weapons.Parameters;
using FredflixAndChell.Shared.Utilities;
using Microsoft.Xna.Framework;
using System;

namespace FredflixAndChell.Shared.GameObjects.Weapons
{
    public class Gun : Weapon
    {
        private GunRenderer _renderer;
        private Player _player;
        private int _maxAmmo;
        private int _magazineSize;
        private int _bulletCount;
        private float _accuracy;
        private float _bulletSpread;

        private Vector2 _barrelOffset;

        public Cooldown Reload { get; set; }
        public GunParameters Parameters { get; }
        public int Ammo { get; set; }
        public int MagazineAmmo { get; set; }

        public Gun(Player player, GunParameters gunParameters, GunMetadata metadata = null) : base(player)
        {
            _player = player;
            Parameters = gunParameters;
            SetupParameters(metadata);
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            _renderer = addComponent(new GunRenderer(this, _player));
        }

        private void SetupParameters(GunMetadata metadata = null)
        {
            _accuracy = Parameters.Accuracy;
            Ammo = metadata?.GetAmmo() ?? Parameters.Ammo;
            MagazineAmmo = metadata?.GetMagazineAmmo() ?? Parameters.MagazineAmmo;
            _maxAmmo = Parameters.MaxAmmo;
            _magazineSize = Parameters.MagazineSize;
            _barrelOffset = Parameters.BarrelOffset;
            Cooldown = new Cooldown(Parameters.FireRate);
            Reload = new Cooldown(Parameters.ReloadTime);

            _bulletCount = Parameters.BulletCount;
            _bulletSpread = Parameters.BulletSpread;
        }

        public override void Fire()
        {
            CheckAmmo();
            if (Cooldown.IsReady() && Reload.IsReady() && MagazineAmmo > 0)
            {
                //Functionality
                Cooldown.Start();
                var dir = (float)Math.Atan2(_player.FacingAngle.Y, _player.FacingAngle.X);
                var x = (float)(position.X
                    + Math.Cos(localRotation) * _barrelOffset.X
                    + Math.Cos(localRotation) * _barrelOffset.Y);
                var y = (float)(position.Y
                    + Math.Sin(localRotation) * _barrelOffset.Y
                    + Math.Sin(localRotation) * _barrelOffset.X);
                for (float i = 0; i < _bulletCount; i++)
                {
                    var direction = dir + (1 - _accuracy) * Nez.Random.minusOneToOne() / 2
                    + (1 - _accuracy) * _player.Velocity.Length() * Nez.Random.minusOneToOne()
                    + ((i * 2 - _bulletCount) * _bulletSpread / _bulletCount);

                    Bullet.Create(_player, x, y, direction, Parameters.BulletParameters);
                }
                MagazineAmmo--;

                //Animation
                _renderer?.Fire();
            }
        }

        private void CheckAmmo()
        {
            if (MagazineAmmo == 0)
            {
                if (Ammo <= 0)
                {
                    //Totally out of ammo? 
                    _renderer.Empty();
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
            if (!Reload.IsOnCooldown() && MagazineAmmo != _magazineSize)
            {
                //Function
                Reload.Start();
                int newBullets = Math.Min(_magazineSize - MagazineAmmo, Ammo);
                Ammo = Ammo - newBullets;
                MagazineAmmo = MagazineAmmo + newBullets;

                //Animation
                _renderer.Reload();
            }
        }

        public override void OnDespawn()
        {
        }

        public override void Update()
        {
            base.Update();
            Reload.Update();
        }

        public override void ToggleRunning(bool isRunning)
        {
            _renderer?.ToggleRunningDisplacement(isRunning);
        }
    }
}
