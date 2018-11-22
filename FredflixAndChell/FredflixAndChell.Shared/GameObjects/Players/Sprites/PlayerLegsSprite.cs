using FredflixAndChell.Shared.Utilities.Graphics.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.GameObjects.Players.Sprites
{
    public class PlayerLegsSprite
    {
        public enum LegsAnimation
        {
            Idle,
            Walking
        }

        public SpriteAnimationDescriptor Idle { get; set; }
        public SpriteAnimationDescriptor Walking { get; set; }
    }
}
