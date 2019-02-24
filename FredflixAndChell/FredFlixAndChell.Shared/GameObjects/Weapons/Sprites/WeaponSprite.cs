using FredflixAndChell.Shared.Utilities.Graphics.Animations;

namespace FredflixAndChell.Shared.GameObjects.Weapons.Sprites
{
    public class WeaponSprite
    {
        public string Source { get; set; }
        public SpriteAnimationDescriptor Idle { get; set; }
        public SpriteAnimationDescriptor Fire { get; set; }
        public SpriteAnimationDescriptor Icon { get; set; }
    }
}
