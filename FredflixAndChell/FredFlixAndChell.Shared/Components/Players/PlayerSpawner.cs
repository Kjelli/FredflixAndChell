using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.Utilities
{
    public class PlayerSpawner
    {
        private bool _shuffled;

        public class SpawnLocation
        {
            public string ID { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public int Counter { get; set; }

            public SpawnLocation(string ID, int x, int y)
            {
                this.ID = ID;
                X = x;
                Y = y;
            }
        }

        private List<SpawnLocation> _spawnLocations { get; set; }
        private bool _randomSpawns { get; set; }
        private int _indexer { get; set; }

        public List<SpawnLocation> SpawnLocations => _spawnLocations;

        public PlayerSpawner(bool RandomSpawns = true)
        {
            _spawnLocations = new List<SpawnLocation>();
            _randomSpawns = RandomSpawns;
            _indexer = 0;
            //Kanskje deklarere her senere hva slags game type det er osv brur
        }

        public void AddLocation(string name, int x, int y)
        {
            _spawnLocations.Add(new SpawnLocation(name, x, y));
        }

        public Vector2 DistributeSpawn()
        {
            if (_randomSpawns && !_shuffled)
            {
                RandomizeSpawnOrder();
                _shuffled = true;
            }
            var spawnLocation = _spawnLocations[_indexer];
            _indexer++;
            return new Vector2(spawnLocation.X, spawnLocation.Y);
        }

        private void RandomizeSpawnOrder()
        {
            var spawns = _spawnLocations.Count();
            for (var i = 0; i < spawns; i++)
            {
                var current = _spawnLocations[i];
                var next = Nez.Random.nextInt(spawns);
                var temp = _spawnLocations[next];
                _spawnLocations[next] = current;
                _spawnLocations[i] = temp;
            }
        }
    }
}
