using FredflixAndChell.Shared.Systems;
using FredflixAndChell.Shared.Systems.GameModeHandlers;

namespace FredflixAndChell.Shared.Scenes
{
    public class HubScene : BroScene
    {
        public HubScene() : base(new GameSettings
        {
            GameMode = GameMode.Hub,
            Map = "winter_hub",
            DamageMultiplier = 0.0f,
            KnockbackMultiplier = 0.0f
        })
        {
        }

        public override void initialize()
        {
            base.initialize();
            System.Console.WriteLine("Initializing HubScene");
        }

        public override void Setup()
        {
            base.Setup();
            System.Console.WriteLine("Setting up Hubscene");
            
        }
    }
}
