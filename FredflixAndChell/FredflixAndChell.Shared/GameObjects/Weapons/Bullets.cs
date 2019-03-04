using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Utilities.Serialization;
using FredflixAndChell.Shared.Weapons.Parameters;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FredflixAndChell.Shared.GameObjects.Weapons
{
    public static class Bullets
    {
        private static Dictionary<string, BulletParameters> _bullets = new Dictionary<string, BulletParameters>();

        public static List<BulletParameters> All()
        {
            return _bullets.Values.ToList();
        }

        public static BulletParameters Get(string name)
        {
            return _bullets[name];
        }

        public static void LoadFromData()
        {
            var bulletFilenames = Directory.EnumerateFiles($"{Constants.Assets.DataDirectory}/bullets", "*.fml");
            foreach (var bulletFilename in bulletFilenames)
            {
                var bullet = YamlSerializer.DeserializeBulletParameters(bulletFilename);
                _bullets[bullet.Name] = bullet;
            }
        }
    }
}
