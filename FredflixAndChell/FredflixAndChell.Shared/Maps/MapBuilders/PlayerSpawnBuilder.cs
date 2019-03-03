using FredflixAndChell.Shared.Utilities;
using Microsoft.Xna.Framework;
using Nez.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.Maps.MapBuilders
{
    public static class PlayerSpawnBuilder
    {
        public static void BuildPlayerSpawns(this Map map, TiledObjectGroup mapObjects)
        {
            foreach (var spawnObject in mapObjects.objectsWithName(TiledObjects.PlayerSpawn))
            {
                var position = new Vector2(spawnObject.x + (spawnObject.width / 2), spawnObject.y + (spawnObject.height / 2));
                spawnObject.properties.TryParseInt(TiledProperties.PlayerSpawnTeamIndex, out int teamIndex);

                var spawnLocation = new SpawnLocation(position, teamIndex);
                map.SpawnLocations.Add(spawnLocation);
            }
        }
    }
}
