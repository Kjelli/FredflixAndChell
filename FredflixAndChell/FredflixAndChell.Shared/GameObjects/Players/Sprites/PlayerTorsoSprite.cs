using FredflixAndChell.Shared.Utilities.Graphics.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.GameObjects.Players.Sprites
{
    public class PlayerTorsoSprite
    {
        public enum TorsoAnimation
        {
            Front,
            FrontUnarmed,
            Back,
            BackUnarmed
        }

        public SpriteAnimationDescriptor Front { get; set; }
        public SpriteAnimationDescriptor FrontUnarmed { get; set; }
        public SpriteAnimationDescriptor Back { get; set; }
        public SpriteAnimationDescriptor BackUnarmed { get; set; }
    }
}
