using FredflixAndChell.Shared.GameObjects;
using FredflixAndChell.Shared.GameObjects.Props;
using Nez.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.Maps.MapBuilders
{
    public static class ItemSpawnBuilder
    {
        public static void BuildItemSpawns(this Map map, TiledObjectGroup objectGroup)
        {
            foreach (var spawnObject in objectGroup.objectsWithName(TiledObjects.ItemSpawn))
            {
                var spawner = map.scene.addEntity(new Spawner(spawnObject.x + spawnObject.height / 2, spawnObject.y + spawnObject.height / 2));
            }
        }

    }
}
