using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.GameObjects.Players;
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
    public class HUD : SceneComponent
    {
        private const int HUDCenterX = Constants.ScreenWidth / 2;
        private const int HUDY = Constants.ScreenHeight - 100;
        private const int Spacing = 300;

        private List<PlayerPanelHUD> _playerHUDs = new List<PlayerPanelHUD>();

        public HUD()
        {}

        public void AddPlayers(List<Player> players)
        {
            if(_playerHUDs.Count > 0)
            {
                foreach(var playerHud in _playerHUDs)
                {
                    playerHud.destroy();
                }
            }
            for (int i = 0; i < players.Count; i++)
            {
                _playerHUDs.Add(scene.addEntity(new PlayerPanelHUD(players[i], HUDCenterX - players.Count * Spacing / 2 + i * Spacing, HUDY)));
            }
        }
    }
}
