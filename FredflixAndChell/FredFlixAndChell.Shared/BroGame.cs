using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Scenes;
using FredflixAndChell.Shared.Utilities.Serialization;
using Nez;

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

            scene = new HubScene();
        }
    }
}
