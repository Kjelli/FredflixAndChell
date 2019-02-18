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
        private Text _debugOutput;
        public DebugHud() : base("debug_hud")
        {
            _debugOutput = new Text(new NezSpriteFont(Assets.AssetLoader.GetFont("debug")), "", Vector2.Zero, Color.White);

            _debugOutput.renderLayer = Constants.Layers.HUD;
            addComponent(_debugOutput);
            updateInterval = 5;
        }

        public override void onAddedToScene()
        {
            base.onAddedToScene();
            _gameSystem = scene.getSceneComponent<GameSystem>();
        }

        public override void update()
        {
            base.update();
            var stringBuilder = BuildDebugText(new StringBuilder(""), _gameSystem.DebugLines);
            _debugOutput.text = Core.debugRenderEnabled ? stringBuilder.ToString() : " ";
        }

        private StringBuilder BuildDebugText(StringBuilder sb, List<DebugLine> debugLines, int tab = 0)
        {
            foreach(var line in debugLines)
            {
                var text = line.Text();
                if (!string.IsNullOrWhiteSpace(text)){ 
                    sb.Append(' ', tab);
                    sb.AppendLine(text);
                }

                if (line.SubLines?.Count > 0)
                {
                    BuildDebugText(sb, line.SubLines, tab + 2);
                }
            }
            return sb;
        }

        public class DebugLine
        {
            public Func<string> Text { get; set; }
            public List<DebugLine> SubLines {get; set;}
        }
    }
}
