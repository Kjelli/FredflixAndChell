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

        private int _ammo;
        private int _maxAmmo;
        private int _magazineAmmo;
        private int _magazineSize;
        private int _bulletCount;

        private float _accuracy;
        private float _bulletSpread;

        private Vector2 _barrelOffset;

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
            _renderer = addComponent(new GunRenderer(this, _player));
            updateOrder = 1;
        }

        private void SetupParameters()
        {
            _accuracy = _params.Accuracy;
            _ammo = _params.Ammo;
            _maxAmmo = _params.MaxAmmo;
            _magazineAmmo = _params.MagazineAmmo;
            _magazineSize = _params.MagazineSize;
            _barrelOffset = _params.BarrelOffset;
            Cooldown = new Cooldown(_params.FireRate);
            Reload = new Cooldown(_params.ReloadTime);

            _bulletCount = _params.BulletCount;
            _bulletSpread = _params.BulletSpread;

        }

        public void Fire()
        {
            CheckAmmo();
            if (Cooldown.IsReady() && Reload.IsReady() && _magazineAmmo >= 0)
            {
                //Functionality
                Cooldown.Start();
                var dir = (float) Math.Atan2(_player.FacingAngle.Y,_player.FacingAngle.X);
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

                    Bullet.Create(_player, x, y, direction, _params.BulletParameters);
                }
                _magazineAmmo--;

                //Animation
                _renderer?.Fire();
            }
        }

        private void CheckAmmo()
        {
            if (_magazineAmmo == 0)
            {
                if (_ammo <= 0)
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
            if (!Reload.IsOnCooldown() && _magazineAmmo != _magazineSize)
            {
                //Function
                Reload.Start();
                int newBullets = Math.Min(_magazineSize - _magazineAmmo, _ammo);
                _ammo = _ammo - newBullets;
                _magazineAmmo = _magazineAmmo + newBullets;

                //Animation
                _renderer.Reload();
            }
        }

        public override void OnDespawn()
        {
        }

        public override void Update()
        {
            Cooldown.Update();
            Reload.Update();
        }

        public void Destroy()
        {
            removeAllComponents();
            setEnabled(false);
            destroy();
        }

        public void ToggleRunning(bool isRunning)
        {
            _renderer?.ToggleRunningDisplacement(isRunning);
        }
    }
}
