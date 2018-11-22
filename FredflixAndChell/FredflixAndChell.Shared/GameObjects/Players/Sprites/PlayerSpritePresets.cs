using FredflixAndChell.Shared.GameObjects.Players.Characters;
using FredflixAndChell.Shared.Utilities.Graphics.Animations;
using System;

namespace FredflixAndChell.Shared.GameObjects.Players.Sprites
{
    [Obsolete]
    public static class PlayerSpritePresets
    {
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
                Torso = new PlayerTorsoSprite
                {
                    FrontUnarmed = new SpriteAnimationDescriptor
                    {
                        Frames = new int[] {
                            0 + 0 * 2,
                        },
                        FPS = 0.5f,
                        Loop = true
                    },
                    Front = new SpriteAnimationDescriptor
                    {
                        Frames = new int[] {
                            1 + 0 * 2,
                        },
                        FPS = 0.5f,
                        Loop = true
                    },
                    BackUnarmed = new SpriteAnimationDescriptor
                    {
                        Frames = new int[] {
                            0 + 1 * 2
                        },
                        FPS = 0.5f,
                        Loop = true
                    },
                    Back = new SpriteAnimationDescriptor
                    {
                        Frames = new int[] {
                            1 + 1 * 2,
                        },
                        FPS = 0.5f,
                        Loop = true
                    }
                },
                Legs = new PlayerLegsSprite
                {
                    Idle = new SpriteAnimationDescriptor
                    {
                        Frames = new int[]
                    {
                        0 + 0 * 8
                    },
                        Loop = true,
                        FPS = 7
                    },
                    Walking = new SpriteAnimationDescriptor
                    {
                        Frames = new int[]
                        {
                            1 + 7 * 2,
                            1 + 0 * 2,
                            1 + 1 * 2,
                            1 + 2 * 2,
                            1 + 3 * 2,
                            1 + 4 * 2,
                            1 + 5 * 2,
                            1 + 6 * 2
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
