using Microsoft.Xna.Framework.Input;
using Nez;

namespace FredflixAndChell.Shared.Utilities.Input
{
    public class MouseFollow : Component, IUpdatable
    {
        public void update()
        {
            entity.setPosition(Nez.Input.mousePosition);
        }
    }
}
