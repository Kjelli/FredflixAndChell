using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ViewportAdapters;
using Microsoft.Xna.Framework;

namespace FredflixAndChell.Shared.Utilities.Graphics.Cameras
{
    public class SmoothCamera2D : Camera2D
    {
        private ViewportAdapter _viewportAdapter;

        public float SpeedRatio { get; set; }
        public Vector2 TargetPosition { get; set; }

        public SmoothCamera2D(ViewportAdapter viewportAdapter) : base(viewportAdapter)
        {
            _viewportAdapter = viewportAdapter;
        }

        public void LookAtSmooth(Vector2 pos)
        {
            TargetPosition = pos;
        }

        public void Update(GameTime gameTime)
        {
            Origin = new Vector2(_viewportAdapter.VirtualWidth / 2f, _viewportAdapter.VirtualHeight / 2f);
            Position = Position * (1 - SpeedRatio) + (TargetPosition - new Vector2(_viewportAdapter.VirtualWidth / 2f, _viewportAdapter.VirtualHeight / 2f)) * SpeedRatio ;
        }
    }
}
