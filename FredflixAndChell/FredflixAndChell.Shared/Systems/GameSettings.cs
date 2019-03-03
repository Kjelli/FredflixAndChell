using FredflixAndChell.Shared.Systems.GameModeHandlers;

namespace FredflixAndChell.Shared.Systems
{
    public enum TeamMode
    {
        FreeForAll,
        Team
    }

    public class GameSettings
    {
        public static GameSettings Default = new GameSettings
        {
            GameMode = GameMode.CTF,
            Map = "ctf_snow",
            ScoreLimit = 3,
            FriendlyFire = false
        };
        public TeamMode TeamMode { get; set; }
        public GameMode GameMode { get; set; }
        public string Map { get; set; }
        public int ScoreLimit { get; set; }
        public bool FriendlyFire { get; set; }
        public float DamageMultiplier { get; set; } = 1.0f;
        public float KnockbackMultiplier { get; set; } = 1.0f;
        public float StaminaMultiplier { get; set; } = 1.0f;
    }

}
