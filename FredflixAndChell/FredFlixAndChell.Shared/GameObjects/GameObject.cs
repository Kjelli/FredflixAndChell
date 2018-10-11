using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace FredFlixAndChell.Shared
{
    public interface GameObject
    {
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
    }
}
