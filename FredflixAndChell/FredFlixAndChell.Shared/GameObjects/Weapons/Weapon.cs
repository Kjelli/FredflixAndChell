using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.Utilities;

namespace FredflixAndChell.Shared.GameObjects.Weapons
{
    public class Weapon : GameObject
    {
        private readonly Player _player;

        public Weapon(Player player) : base(0, 0)
        {
            _player = player;
        }

        public Cooldown Cooldown { get; set; }

        public override void OnDespawn()
        {
            throw new System.NotImplementedException();
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
    }
}
