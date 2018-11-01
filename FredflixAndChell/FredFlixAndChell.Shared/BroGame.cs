#region Using Statements
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.GameObjects;
using FredflixAndChell.Shared.Utilities;
using FredflixAndChell.Shared.Scenes;
using System.Diagnostics;
using Nez;
using FredflixAndChell.Shared.Utilities.Serialization;

#endregion

namespace FredflixAndChell.Shared
{
    public class BroGame : Core
    {
        public BroGame() : base()
        {
        }
        protected override void Initialize()
        {
            base.Initialize();

            Window.AllowUserResizing = true;
            Window.Title = "Ultimate Brodown";
            scene = new BroScene();
        }
    }
}
