using FredflixAndChell.Shared.GameObjects.Players.Characters;
using FredflixAndChell.Shared.Utilities.Graphics.Animations;
using System;

namespace FredflixAndChell.Shared.GameObjects.Players.Sprites
{
    [Obsolete]
    public static class PlayerSpritePresets
    {
        public static readonly CharacterParameters Tormod = new CharacterParameters
        {
            PlayerSprite = new PlayerSprite
            {
                Source = "players/tormod",
                Head = new PlayerHeadSprite
                {
                    Front = new SpriteAnimationDescriptor
                    {
                        Frames = new int[] { 0 },
                        FPS = 1,
                    },
                    Back = new SpriteAnimationDescriptor
                    {
                        Frames = new int[] { 1 },
                        FPS = 1,
                    }
                },
                Body = new PlayerBodySprite
                {
                    IdleUnarmed = new SpriteAnimationDescriptor
                    {
                        Frames = new int[]
                    {
                        0 + 0 * 8,
                        0 + 1 * 8,
                        0 + 2 * 8,
                        0 + 3 * 8
                    },
                        Loop = true,
                        FPS = 7
                    },
                    Idle = new SpriteAnimationDescriptor
                    {
                        Frames = new int[]
                    {
                        2 + 0 * 8,
                        2 + 1 * 8,
                        2 + 2 * 8,
                        2 + 3 * 8
                    },
                        Loop = true,
                        FPS = 7
                    },
                    WalkingUnarmed = new SpriteAnimationDescriptor
                    {
                        Frames = new int[]
                    {
                        1 + 7 * 8,
                        1 + 0 * 8,
                        1 + 1 * 8,
                        1 + 2 * 8,
                        1 + 3 * 8,
                        1 + 4 * 8,
                        1 + 5 * 8,
                        1 + 6 * 8
                    },
                        Loop = true,
                        FPS = 15
                    },
                    Walking = new SpriteAnimationDescriptor
                    {
                        Frames = new int[]
                    {
                        3 + 7 * 8,
                        3 + 0 * 8,
                        3 + 1 * 8,
                        3 + 2 * 8,
                        3 + 3 * 8,
                        3 + 4 * 8,
                        3 + 5 * 8,
                        3 + 6 * 8
                    },
                        Loop = true,
                        FPS = 15
                    }
                }
            },
            CharacterName = "Tormod"
        };
        public static readonly CharacterParameters Kjelli = new CharacterParameters
        {
            PlayerSprite = new PlayerSprite
            {
                Source = "players/kjelli",
                Head = new PlayerHeadSprite
                {
                    Front = new SpriteAnimationDescriptor
                    {
                        Frames = new int[] { 0 },
                        FPS = 1,
                    },
                    Back = new SpriteAnimationDescriptor
                    {
                        Frames = new int[] { 1 },
                        FPS = 1,
                    }
                },
                Body = new PlayerBodySprite
                {
                    IdleUnarmed = new SpriteAnimationDescriptor
                    {
                        Frames = new int[]
                    {
                        0 + 0 * 8,
                        0 + 1 * 8,
                        0 + 2 * 8,
                        0 + 3 * 8
                    },
                        Loop = true,
                        FPS = 7
                    },
                    Idle = new SpriteAnimationDescriptor
                    {
                        Frames = new int[]
                    {
                        2 + 0 * 8,
                        2 + 1 * 8,
                        2 + 2 * 8,
                        2 + 3 * 8
                    },
                        Loop = true,
                        FPS = 7
                    },
                    WalkingUnarmed = new SpriteAnimationDescriptor
                    {
                        Frames = new int[]
                    {
                        1 + 7 * 8,
                        1 + 0 * 8,
                        1 + 1 * 8,
                        1 + 2 * 8,
                        1 + 3 * 8,
                        1 + 4 * 8,
                        1 + 5 * 8,
                        1 + 6 * 8
                    },
                        Loop = true,
                        FPS = 15
                    },
                    Walking = new SpriteAnimationDescriptor
                    {
                        Frames = new int[]
                    {
                        3 + 7 * 8,
                        3 + 0 * 8,
                        3 + 1 * 8,
                        3 + 2 * 8,
                        3 + 3 * 8,
                        3 + 4 * 8,
                        3 + 5 * 8,
                        3 + 6 * 8
                    },
                        Loop = true,
                        FPS = 15
                    }
                }
            },
            CharacterName = "Kjelli"
        };
        public static readonly CharacterParameters Trump = new CharacterParameters
        {
            PlayerSprite = new PlayerSprite
            {
                Source = "players/trump",
                Head = new PlayerHeadSprite
                {
                    Front = new SpriteAnimationDescriptor
                    {
                        Frames = new int[] {
                        0 + 0 * 2,
                        0 + 1 * 2
                    },
                        FPS = 0.5f,
                        Loop = true
                    },
                    Back = new SpriteAnimationDescriptor
                    {
                        Frames = new int[] {
                        1 + 0 * 2,
                        1 + 1 * 2 },
                        FPS = 0.5f,
                        Loop = true
                    }
                },
                Body = new PlayerBodySprite
                {
                    IdleUnarmed = new SpriteAnimationDescriptor
                    {
                        Frames = new int[]
                    {
                        0 + 0 * 8,
                        0 + 1 * 8,
                        0 + 2 * 8,
                        0 + 3 * 8
                    },
                        Loop = true,
                        FPS = 7
                    },
                    Idle = new SpriteAnimationDescriptor
                    {
                        Frames = new int[]
                    {
                        2 + 0 * 8,
                        2 + 1 * 8,
                        2 + 2 * 8,
                        2 + 3 * 8
                    },
                        Loop = true,
                        FPS = 7
                    },
                    WalkingUnarmed = new SpriteAnimationDescriptor
                    {
                        Frames = new int[]
                    {
                        1 + 7 * 8,
                        1 + 0 * 8,
                        1 + 1 * 8,
                        1 + 2 * 8,
                        1 + 3 * 8,
                        1 + 4 * 8,
                        1 + 5 * 8,
                        1 + 6 * 8
                    },
                        Loop = true,
                        FPS = 15
                    },
                    Walking = new SpriteAnimationDescriptor
                    {
                        Frames = new int[]
                    {
                        3 + 7 * 8,
                        3 + 0 * 8,
                        3 + 1 * 8,
                        3 + 2 * 8,
                        3 + 3 * 8,
                        3 + 4 * 8,
                        3 + 5 * 8,
                        3 + 6 * 8
                    },
                        Loop = true,
                        FPS = 15
                    }
                }
            },
            CharacterName = "Trump"
        };
    }
}
