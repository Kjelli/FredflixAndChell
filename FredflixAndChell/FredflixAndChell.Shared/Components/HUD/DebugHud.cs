using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.Components.HUD
{
    public class DebugHud : Entity
    {
        private GameSystem _gameSystem;
        private Text _text;
        public DebugHud() : base("debug_hud")
        {
            _text = new Text(new NezSpriteFont(Assets.AssetLoader.GetFont("debug")), "DENNE TEKSTEN BURDE JO KUNNE LESES DA", Vector2.Zero, Color.White);
            _text.renderLayer = Constants.Layers.HUD;
            addComponent(_text);
            updateInterval = 15;
        }

        public override void onAddedToScene()
        {
            base.onAddedToScene();
            _gameSystem = scene.getEntityProcessor<GameSystem>();
        }

        public override void update()
        {
            base.update();
            var text = "";
            text += $"Entities: {scene.entities.count}\n";
            text += "Players: \n";

            foreach (var standing in _gameSystem.PlayerStandings)
            {
                var player = standing.Value;
                var playerNo = standing.Key;
                text += $"  Player {playerNo}:\n";
                text += $"    Health: {player.Health}\n";
                text += $"    Stamina: {player.Stamina}\n";
                text += $"    State: {player.PlayerState}\n";
            }

            _text.text = text;
        }
    }
}
