namespace FredflixAndChell.Shared.GameObjects.Bullets.Sprites
{
    public class BulletSprite
    {
        public static readonly BulletSprite Standard = "standard";
        public static readonly BulletSprite Fido = "fido";

        private readonly string _spriteName;
        private BulletSprite(string spriteName)
        {
            _spriteName = spriteName;
        }

        public static implicit operator BulletSprite(string input)
        {
            return new BulletSprite(input);
        }

        public override string ToString()
        {
            return _spriteName;
        }
    }
}
