using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.GameObjects.Players.Sprites
{
    public class PlayerSprite
    {
        public string Source { get; set; }
        public PlayerHeadSprite Head { get; set; }
        public PlayerBodySprite Body { get; set; }
    }

}
