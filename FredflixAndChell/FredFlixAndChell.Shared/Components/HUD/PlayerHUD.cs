using FredflixAndChell.Shared.Assets;
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
    public class PlayerHUD : Entity
    {
     

        private Subtexture _hudFront { get; set; }
        private Subtexture _hudBack { get; set; }
        private Subtexture _healthBar { get; set; }
        private Subtexture _staminaBar { get; set; }


        public PlayerHUD(int playerCount = 2)
        {
            var hud_texture = AssetLoader.GetTexture("UI/HUD");

            _hudFront = new Subtexture(hud_texture, 0, 0, 48, 16);
            _healthBar = new Subtexture(hud_texture, 48, 0, 25, 3);
            _staminaBar = new Subtexture(hud_texture, 48, 3, 8, 3);

            var xOffset = DetermineXOffset(playerCount);


            for(int i = 0; i < playerCount; i++)
            {
                var tempPanel = new HudPanel();

                //Front hud
                var panel  = new Sprite(_hudFront);
                panel.renderLayer = Layers.HUD;
                panel.localOffset = new Vector2(xOffset, 0);

                var hb = new Sprite(_healthBar);
                hb.localOffset = panel.localOffset + new Vector2(36,2);
                hb.renderLayer = Layers.HUD;


                addComponent(panel);
                addComponent(hb);
            
                hb.transform.scale = new Vector2(14f * hb.width / hb.width, hb.height);

               

                xOffset += 300;
            }


            position = new Vector2(ScreenWidth / 2, ScreenHeight - 40);
            this.setScale(4);


        }


        public int DetermineXOffset(int playerCount)
        {
            switch (playerCount)
            {
                
                case 2: return -150;
                case 3: return -300;
                case 4: return -450;
                default:
                    return 0;
            }
        }

              
    }
}
