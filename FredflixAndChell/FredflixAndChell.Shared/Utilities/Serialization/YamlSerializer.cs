using FredflixAndChell.Shared.Components.Bullets.Behaviours;
using FredflixAndChell.Shared.GameObjects.Players.Characters;
using FredflixAndChell.Shared.GameObjects.Players.Sprites;
using FredflixAndChell.Shared.GameObjects.Weapons.Parameters;
using FredflixAndChell.Shared.Weapons.Parameters;
using SharpYaml.Serialization;
using System.IO;

namespace FredflixAndChell.Shared.Utilities.Serialization
{
    public static class YamlSerializer
    {
        private static readonly string Extension = "fml";
        private static readonly string BasePath = "Content/data/player";

        // Only to be called ad-hoc when serializing one or more presets
        public static void SerializeAll()
        {
            SerializeCharacter(PlayerSpritePresets.Trump);
        }

        public static BulletParameters DeserializeBulletParameters(string filename)
        {
            var deserializer = new Serializer();

            var bulletParam = deserializer.Deserialize<BulletParameters>(File.ReadAllText(filename));
            return bulletParam;
        }

        public static GunParameters DeserializeGunParameters(string filename)
        {
            var deserializer = new Serializer();

            var gunParam = deserializer.Deserialize<GunParameters>(File.ReadAllText(filename));
            return gunParam;
        }

        public static MeleeParameters DeserializeMeleeParameters(string filename)
        {
            var deserializer = new Serializer();

            var meleeParam = deserializer.Deserialize<MeleeParameters>(File.ReadAllText(filename));
            return meleeParam;
        }

        public static CharacterParameters DeserializeCharacterParameters(string filename)
        {
            var deserializer = new Serializer();

            var characterParameters = deserializer.Deserialize<CharacterParameters>(File.ReadAllText(filename));
            return characterParameters;
        }

        public static void SerializeCharacter(CharacterParameters player)
        {
            var serializer = new Serializer();
            var yaml = serializer.Serialize(player, typeof(CharacterParameters));
            var filename = $"{BasePath}/{player.CharacterName}.{Extension}";

            if (!Directory.Exists(BasePath))
            {
                Directory.CreateDirectory(BasePath);
            }
            File.WriteAllText(filename, yaml);
        }
    }
}
