using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.GameObjects.Players.Characters;
using FredflixAndChell.Shared.GameObjects.Players.Sprites;
using FredflixAndChell.Shared.GameObjects.Weapons;
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

        public static GunParameters DeserializeGunParameters(string filename)
        {
            var deserializer = new Serializer();
            
            var gunParam = deserializer.Deserialize<GunParameters>(File.ReadAllText(filename));
            return gunParam;
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
