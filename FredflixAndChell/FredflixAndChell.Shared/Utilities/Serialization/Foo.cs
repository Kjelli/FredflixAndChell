using FredflixAndChell.Shared.GameObjects.Weapons;
using SharpYaml.Serialization;
using System.IO;

namespace FredflixAndChell.Shared.Utilities.Serialization
{
    public class YamlSerializer
    {
        private readonly string Filename = "fido";
        private readonly string Extension = "fml";
        private readonly string BasePath = "Content/data";

        public void SerializeGun(GunParameters gun)
        {
            var serializer = new Serializer();
            var yaml = serializer.Serialize(GunPresets.Fido, typeof(GunParameters));
            var filename = $"{BasePath}/{Filename}.{Extension}";

            if (!Directory.Exists(BasePath))
            {
                Directory.CreateDirectory(BasePath);
            }
            File.WriteAllText(filename, yaml);
        }
    }
}
