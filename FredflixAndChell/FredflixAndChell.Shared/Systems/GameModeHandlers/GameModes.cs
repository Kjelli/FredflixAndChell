
namespace FredflixAndChell.Shared.Systems.GameModeHandlers
{
    public enum GameModes
    {
        Hub, // Used for game settings configuration
        Rounds, // Score by being last player alive
        Deathmatch, // Score by killing another player
        CaptureTheFlag, // Score by retrieving opposite team's flag
    }
}
