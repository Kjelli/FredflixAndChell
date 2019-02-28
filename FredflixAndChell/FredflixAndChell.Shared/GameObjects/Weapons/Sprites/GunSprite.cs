using FredflixAndChell.Shared.Utilities.Graphics.Animations;

namespace FredflixAndChell.Shared.GameObjects.Weapons.Sprites
{
    public class GunSprite : WeaponSprite
    {
        public enum GunAnimations
        {
            Held_Idle,
            Held_Fired,
            Held_Empty,
            Reload
        }

        public SpriteAnimationDescriptor Reload { get; set; }
        public SpriteAnimationDescriptor Empty { get; set; }
    }
}
