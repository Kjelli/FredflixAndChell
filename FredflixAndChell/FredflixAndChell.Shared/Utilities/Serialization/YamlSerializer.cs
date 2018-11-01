using FredflixAndChell.Shared.GameObjects.Weapons;
using SharpYaml.Serialization;
using System.IO;

namespace FredflixAndChell.Shared.Utilities.Serialization
{
    public static class YamlSerializer
    {
        private static readonly string Extension = "fml";
        private static readonly string BasePath = "Content/data/guns";
        /*
        public static void SerializeAll()
        {
            SerializeGun(Guns.Fido);
            SerializeGun(Guns.M4);
            SerializeGun(Guns.PewPew);
        }*/

        public static GunParameters Deserialize(string filename)
        {
            var deserializer = new Serializer();
            
            var gunParam = deserializer.Deserialize<GunParameters>(File.ReadAllText(filename));
            return gunParam;
        }

        public static void SerializeGun(GunParameters gun)
        {
            var serializer = new Serializer();
            var yaml = serializer.Serialize(gun, typeof(GunParameters));
            var filename = $"{BasePath}/{gun.Name}.{Extension}";

            if (!Directory.Exists(BasePath))
            {
                Directory.CreateDirectory(BasePath);
            }
            File.WriteAllText(filename, yaml);
        }
    }
}
