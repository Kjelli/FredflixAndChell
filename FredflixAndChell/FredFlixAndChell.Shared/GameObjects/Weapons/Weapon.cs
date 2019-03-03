using FredflixAndChell.Shared.GameObjects.Collectibles.Metadata;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.GameObjects.Weapons.Parameters;
using FredflixAndChell.Shared.Utilities;

namespace FredflixAndChell.Shared.GameObjects.Weapons
{
    public abstract class Weapon : GameObject
    {
        private readonly Player _player;

        public abstract CollectibleMetadata Metadata { get; }
        public abstract WeaponParameters Parameters { get; }
        public Cooldown Cooldown { get; set; }
        public string Name => Parameters.Name;

        public Weapon(Player player) : base(0, 0)
        {
            _player = player;
        }

        public override void OnDespawn()
        {
        }

        public override void OnSpawn()
        {
            Cooldown.Start();
            updateOrder = 1;
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

        public virtual void ToggleRunning(bool isRunning)
        {
        }

        public virtual void Fire()
        {
        }
    }
}
