using FredflixAndChell.Shared.Utilities.Graphics.Animations;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.GameObjects.Weapons.Sprites
{
    public class GunSpritePresets
    {
        private static readonly Vector2 _defaultGunOrigin = new Vector2(10, 16);

        public static readonly GunSprite M4 = new GunSprite
        {
            Source = "guns/m4",
            Idle = new SpriteAnimationDescriptor
            {
                Frames = new int[]
                {
                    2 + 0 * 8
                },
                FPS = 1,
                Loop = false,
                Origin = _defaultGunOrigin
            },
            Fire = new SpriteAnimationDescriptor
            {
                Frames = new int[]
                {
                    1 + 1 * 8,
                    1 + 2 * 8,
                    1 + 3 * 8,
                    1 + 0 * 8
                },
                FPS = 15,
                Loop = false,
                Origin = _defaultGunOrigin
            },
            Reload = new SpriteAnimationDescriptor
            {
                Frames = new int[]
                {
                    1 + 0 * 8,
                    2 + 0 * 8,
                    2 + 1 * 8,
                    2 + 2 * 8,
                    2 + 3 * 8,
                    2 + 4 * 8,
                    2 + 3 * 8,
                    2 + 2 * 8,
                    2 + 1 * 8,
                    2 + 0 * 8,
                    1 + 0 * 8
                },
                FPS = 15,
                Loop = false,
                Origin = _defaultGunOrigin
            },
        };

        public static readonly GunSprite Fido = new GunSprite
        {
            Source = "guns/fido",
            Idle = new SpriteAnimationDescriptor
            {
                Frames = new int[]
                {
                    2 + 0 * 8,
                    3 + 0 * 8,
                    3 + 1 * 8,
                    3 + 0 * 8,
                    2 + 0 * 8,
                    3 + 0 * 8,
                    2 + 0 * 8,
                    3 + 0 * 8,
                    3 + 1 * 8,
                    3 + 0 * 8
                },
                FPS = 4,
                Loop = true,
                Origin = _defaultGunOrigin
            },
            Fire = new SpriteAnimationDescriptor
            {
                Frames = new int[]
                {
                    1 + 1 * 8,
                    1 + 2 * 8,
                    1 + 3 * 8,
                    1 + 0 * 8
                },
                FPS = 15,
                Loop = false,
                Origin = _defaultGunOrigin
            },
            Reload = new SpriteAnimationDescriptor
            {
                Frames = new int[]
                {
                    1 + 0 * 8,
                    2 + 0 * 8,
                    2 + 1 * 8,
                    2 + 2 * 8,
                    2 + 3 * 8,
                    2 + 2 * 8,
                    2 + 1 * 8,
                    2 + 0 * 8,
                    1 + 0 * 8
                },
                FPS = 15,
                Loop = false,
                Origin = _defaultGunOrigin
            }
        };
    }
}
