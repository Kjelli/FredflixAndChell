using FredflixAndChell.Shared.GameObjects.Weapons.Sprites;

namespace FredflixAndChell.Shared.GameObjects.Weapons.Parameters
{
    public class MeleeParameters : WeaponParameters
    {
        public MeleeSprite Sprite { get; set; }
        public float Damage { get; set; } = 10f;
        public float Knockback { get; set; } = 1.25f;
    }
}
