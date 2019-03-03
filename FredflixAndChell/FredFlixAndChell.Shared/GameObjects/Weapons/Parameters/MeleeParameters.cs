using FredflixAndChell.Shared.GameObjects.Weapons.Sprites;
using Microsoft.Xna.Framework;

namespace FredflixAndChell.Shared.GameObjects.Weapons.Parameters
{
    public enum MeleeType
    {
        Hold, Swing
    }
    public class MeleeParameters : WeaponParameters
    {
        public MeleeType MeleeType { get; set; } = MeleeType.Swing;
        public MeleeSprite Sprite { get; set; }
        public float Damage { get; set; } = 10f;
        public float AerialKnockback { get; set; } = 0f;
        public float Knockback { get; set; } = 1.25f;
        public bool Flip { get; set; } = true;
        public Vector2 HitboxOffset { get; set; } = new Vector2(10, 0);
        public Vector2 HitboxSize { get; set; } = new Vector2(10, 0);
    }
}
