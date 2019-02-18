using FredflixAndChell.Shared.Components.Weapons;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.GameObjects.Weapons.Parameters;
using FredflixAndChell.Shared.Utilities;

namespace FredflixAndChell.Shared.GameObjects.Weapons
{
    public class Melee : GameObject
    {
        private MeleeRenderer _renderer;
        private Player _player;

        public MeleeParameters Parameters { get; }
        public Cooldown Cooldown { get; set; }

        public Melee(Player player, MeleeParameters weaponParameters) : base(0, 0)
        {
            _player = player;
            Parameters = weaponParameters;
            SetupParameters();
        }

        public override void OnSpawn()
        {
            Cooldown.Start();
            _renderer = addComponent(new MeleeRenderer(this, _player));
            updateOrder = 1;
        }

        private void SetupParameters()
        {
            Cooldown = new Cooldown(Parameters.FireRate);
        }

        //public void Fire()
        //{
        //    if (Cooldown.IsReady())
        //    {
        //        //Functionality
        //        Cooldown.Start();
        //        var dir = (float)Math.Atan2(_player.FacingAngle.Y, _player.FacingAngle.X);
        //        var x = (float)(position.X
        //            + Math.Cos(localRotation) * _barrelOffset.X
        //            + Math.Cos(localRotation) * _barrelOffset.Y);
        //        var y = (float)(position.Y
        //            + Math.Sin(localRotation) * _barrelOffset.Y
        //            + Math.Sin(localRotation) * _barrelOffset.X);
        //        for (float i = 0; i < _bulletCount; i++)
        //        {
        //            var direction = dir + (1 - _accuracy) * Nez.Random.minusOneToOne() / 2
        //            + (1 - _accuracy) * _player.Velocity.Length() * Nez.Random.minusOneToOne()
        //            + ((i * 2 - _bulletCount) * _bulletSpread / _bulletCount);

        //            Bullet.Create(_player, x, y, direction, Parameters.BulletParameters);
        //        }

        //        //Animation
        //        _renderer?.Fire();
        //    }
        //}

        //private void CheckAmmo()
        //{
        //    if (_magazineAmmo == 0)
        //    {
        //        if (_ammo <= 0)
        //        {
        //            //Totally out of ammo? 
        //            //TODO: Throw away this
        //        }
        //        else
        //        {
        //            //Reload
        //            ReloadMagazine();
        //        }
        //    }
        //}

        //public void ReloadMagazine()
        //{
        //    if (!Reload.IsOnCooldown() && _magazineAmmo != _magazineSize)
        //    {
        //        //Function
        //        Reload.Start();
        //        int newBullets = Math.Min(_magazineSize - _magazineAmmo, _ammo);
        //        _ammo = _ammo - newBullets;
        //        _magazineAmmo = _magazineAmmo + newBullets;

        //        //Animation
        //        _renderer.Reload();
        //    }
        //}

        public override void OnDespawn()
        {
        }

        public override void Update()
        {
            Cooldown.Update();
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
