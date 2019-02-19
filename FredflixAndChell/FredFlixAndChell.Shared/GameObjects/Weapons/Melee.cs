using FredflixAndChell.Shared.Components.Weapons;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.GameObjects.Weapons.Parameters;
using FredflixAndChell.Shared.Utilities;

namespace FredflixAndChell.Shared.GameObjects.Weapons
{
    public class Melee : Weapon
    {
        private MeleeRenderer _renderer;
        private Player _player;

        public MeleeParameters Parameters { get; }

        public Melee(Player player, MeleeParameters meleeParameters) : base(player)
        {
            _player = player;
            Parameters = meleeParameters;
            SetupParameters();
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            _renderer = addComponent(new MeleeRenderer(this, _player));
        }

        private void SetupParameters()
        {
            Cooldown = new Cooldown(Parameters.FireRate);
        }

        public void Fire()
        {
            if (Cooldown.IsReady())
            {
                //Functionality
                Cooldown.Start();
                //var dir = (float)Math.Atan2(_player.FacingAngle.Y, _player.FacingAngle.X);
                //var x = (float)(position.X
                //    + Math.Cos(localRotation) * _barrelOffset.X
                //    + Math.Cos(localRotation) * _barrelOffset.Y);
                //var y = (float)(position.Y
                //    + Math.Sin(localRotation) * _barrelOffset.Y
                //    + Math.Sin(localRotation) * _barrelOffset.X);
                //for (float i = 0; i < _bulletCount; i++)
                //{
                //    var direction = dir + (1 - _accuracy) * Nez.Random.minusOneToOne() / 2
                //    + (1 - _accuracy) * _player.Velocity.Length() * Nez.Random.minusOneToOne()
                //    + ((i * 2 - _bulletCount) * _bulletSpread / _bulletCount);

                //    Bullet.Create(_player, x, y, direction, Parameters.BulletParameters);
                //}

                //Animation
                _renderer?.Fire();
            }
        }

        public override void OnDespawn()
        {
        }

        public override void Update()
        {
            base.Update();
        }

        public void ToggleRunning(bool isRunning)
        {
            _renderer?.ToggleRunningDisplacement(isRunning);
        }
    }
}
