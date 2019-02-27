using FredflixAndChell.Shared.GameObjects.Players;
using Microsoft.Xna.Framework;

namespace FredflixAndChell.Shared.Utilities
{
    public class DirectionalDamage
    {
        public float Damage { get; set; }
        public float Knockback { get; set; }
        public Vector2 Direction { get; set; }
        public Player SourceOfDamage { get; set; }
    }
}
