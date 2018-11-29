using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.Components.HUD
{
    public class PlayerPanelHUD : Component
    {
        private Sprite _front;
        private Sprite _background;

        private Sprite _healthBar;
        private Sprite _staminaBar;


        public PlayerPanelHUD()
        {
            /*

            var pla = new Sprite(_hudFront);
            pla.renderLayer = Layers.HUD;
            pla.localOffset = new Vector2(xOffset, 0);

            var hb = new Sprite(_healthBar);
            hb.localOffset = pla.localOffset;
            hb.renderLayer = Layers.HUD;

            */
        }

    }
}
