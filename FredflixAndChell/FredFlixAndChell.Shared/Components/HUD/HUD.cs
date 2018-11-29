using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Utilities.Graphics;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.Components.HUD
{
    public class HUD : Entity
    {
        private List<PlayerPanelHUD> _playerHUDs = new List<PlayerPanelHUD>();

        public HUD(int playerCount = 2)
        {
            var hudCenterArea = Constants.ScreenWidth / 2;
            for (int i = 0; i < playerCount; i++)
            {
                _playerHUDs.Add(new PlayerPanelHUD(hudCenterArea - (i * (i % 2 == 0 ? -1 : 1)) * 300, 800));
            }
        }
    }
}
