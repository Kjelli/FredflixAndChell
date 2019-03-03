using Microsoft.Xna.Framework;

namespace FredflixAndChell.Shared.Utilities
{
    public class SpawnLocation
    {
        public Vector2 Position { get; }
        public int TeamIndex { get; }

        public SpawnLocation(Vector2 position, int teamIndex = -1)
        {
            Position = position;
            TeamIndex = teamIndex;
        }
    }
}
