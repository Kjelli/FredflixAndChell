using FredflixAndChell.Shared.GameObjects.Players.Sprites;

namespace FredflixAndChell.Shared.GameObjects.Players.Characters
{
    public class CharacterParameters
    {
        public PlayerSprite PlayerSprite { get; set; }
        public string CharacterName { get; set; }
        public float MaxHealth { get; set; } = 100;
        public float MaxStamina { get; set; } = 100;
        public float Speed { get; set; } = 50f;
    }
}
