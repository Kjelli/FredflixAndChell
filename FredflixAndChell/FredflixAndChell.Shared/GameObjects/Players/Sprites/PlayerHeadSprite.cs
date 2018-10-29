﻿using FredflixAndChell.Shared.Utilities.Graphics.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.GameObjects.Players.Sprites
{
    public class PlayerHeadSprite
    {
        public enum HeadAnimations
        {
            FrontFacing,
            BackFacing
        }

        public SpriteAnimationDescriptor Front { get; set; }
        public SpriteAnimationDescriptor Back { get; set; }

        // Construction limited to namespace
        internal PlayerHeadSprite() { }
    }
}