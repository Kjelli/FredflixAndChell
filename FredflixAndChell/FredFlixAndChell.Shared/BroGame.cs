using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Scenes;
using FredflixAndChell.Shared.Systems;
using FredflixAndChell.Shared.Systems.GameModeHandlers;
using FredflixAndChell.Shared.Utilities;
using FredflixAndChell.Shared.Utilities.Serialization;
using Nez;
using Nez.Systems;
using System;

namespace FredflixAndChell.Shared
{
    public class BroGame : Core
    {
        protected override void Initialize()
        {
            base.Initialize();

            Window.AllowUserResizing = true;
            Window.Title = "Ultimate Brodown";
            Screen.setSize(Constants.ScreenWidth, Constants.ScreenHeight);


            //scene = new LobbyScene();

            // YamlSerializer.SerializeAll();

            var settings = new GameSettings
            {
                GameMode = GameModes.Rounds,
                Map = "winter_debug"
            };
            scene = new BroScene(settings);
        }
    }
}
