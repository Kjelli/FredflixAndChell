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
    public class PlayerPanelHUD : Component
    {
        private Sprite _panel;
        private ScalableSprite _healthBar;
        private ScalableSprite _staminaBar;

        private int _healthBarTextureWidth = 25;

        public PlayerPanelHUD(int screenX, int screenY)
        {
            var hudTexture = AssetLoader.GetTexture("UI/HUD");
            var hudFrontTexture = new Subtexture(hudTexture, 0, 0, 48, 16);

            var healthBar = new Subtexture(hudTexture, 48, 0, _healthBarTextureWidth, 3);
            var staminaBar = new Subtexture(hudTexture, 48, 3, 8, 3);

            _panel = new ScalableSprite(hudFrontTexture);
            _panel.renderLayer = Layers.HUD;
            _panel.localOffset = new Vector2(screenX, screenY);

            _healthBar = new ScalableSprite(healthBar);
            _healthBar.localOffset = _panel.localOffset + new Vector2(-16, -4);
            _healthBar.renderLayer = Layers.HUD;
            _healthBar.origin = new Vector2(0, 0);

            _healthBar.SetScale(new Vector2(0.5f, 1));
        }

        public override void onAddedToEntity()
        {
            entity.addComponent(_panel);
            entity.addComponent(_healthBar);
        }
    }

}
