using FredflixAndChell.Shared.Utilities.Graphics.Animations;
using Nez.Sprites;

namespace FredflixAndChell.Shared.GameObjects.Weapons.Sprites
{
    
    public class GunSprite
    {
        public enum GunAnimations
        {
            Held_Idle,
            Held_Fired,
            Reload
        }

        public string Source { get; set; }
    

        public SpriteAnimationDescriptor Idle { get; set; }
        public SpriteAnimationDescriptor Fire { get; set; }
        public SpriteAnimationDescriptor Reload { get; set; }
        public SpriteAnimationDescriptor Icon { get; internal set; }

        // Construction limited to namespace
        internal GunSprite() { }
    }
}
