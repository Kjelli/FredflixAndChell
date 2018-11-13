using FredflixAndChell.Shared.Utilities.Graphics.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.GameObjects.Players.Sprites
{


    public class PlayerBodySprite
    {
        public enum BodyAnimation
        {
            WalkingUnarmed,
            IdleUnarmed,
            Walking,
            Idle
        }

        public SpriteAnimationDescriptor Idle { get; set; }
        public SpriteAnimationDescriptor IdleUnarmed { get; set; }
        public SpriteAnimationDescriptor Walking { get; set; }
        public SpriteAnimationDescriptor WalkingUnarmed { get; set; }

        // Construction limited to namespace
        internal PlayerBodySprite() { }
    }
}
