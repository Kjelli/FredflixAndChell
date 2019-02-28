using FredflixAndChell.Shared.GameObjects.Props;
using FredflixAndChell.Shared.Utilities;
using Nez.Tiled;
using System.Collections.Generic;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.Maps.MapBuilders
{
    public static class ItemSpawnBuilder
    {
        public static void BuildItemSpawns(this Map map, TiledObjectGroup objectGroup)
        {
            foreach (var spawnObject in objectGroup.objectsWithName(TiledObjects.ItemSpawn))
            {
                var props = spawnObject.properties;
                var spawnerParameters = ParseProperties(props);
                var spawnX = spawnObject.x + spawnObject.height / 2;
                var spawnY = spawnObject.y + spawnObject.height / 2;

                map.scene.addEntity(new Spawner(spawnX, spawnY, spawnerParameters));
            }
        }

        private static Spawner.SpawnerParameters ParseProperties(Dictionary<string, string> props)
        {
            var parameters = new Spawner.SpawnerParameters();

            if (props.TryParseFloat(TiledProperties.SpawnerMinIntervalSeconds, out float minInterval))
                parameters.MinIntervalSeconds = minInterval;
            if (props.TryParseFloat(TiledProperties.SpawnerMaxIntervalSeconds, out float maxInterval))
                parameters.MaxIntervalSeconds = maxInterval;
            if (props.TryParseInt(TiledProperties.SpawnerMaxSpawns, out int maxSpawns))
                parameters.MaxSpawns = maxSpawns;
            if (props.TryParseBool(TiledProperties.SpawnerCameraTracking, out bool cameraTracking))
                parameters.CameraTracking = cameraTracking;

            parameters.WeaponBlacklist = props.ParseCommaSeparatedList(TiledProperties.SpawnerWeaponBlacklist);
            parameters.WeaponWhitelist = props.ParseCommaSeparatedList(TiledProperties.SpawnerWeaponWhitelist);
            parameters.RarityBlacklist = props.ParseCommaSeparatedList(TiledProperties.SpawnerRarityBlacklist);
            parameters.RarityWhitelist = props.ParseCommaSeparatedList(TiledProperties.SpawnerRarityWhitelist);

            return parameters;
        }
    }
}
