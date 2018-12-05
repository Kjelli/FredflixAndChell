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

        public PlayerSpawner(bool RandomSpawns = true)
        {
            _spawnLocations = new List<SpawnLocation>();
            _randomSpawns = RandomSpawns;
            _indexer = 0;
            //Kanskje deklarere her senere hva slags game type det er osv brur
        }

        public void AddLocation(string name, int x, int y)
        {
            _spawnLocations.Add(new SpawnLocation(name,x,y));
        }

        public Vector2 DistributeSpawn()
        {
            Random rnd = new Random();
            try
            {
                if (_randomSpawns)
                {
                    
                    int index = rnd.Next(0, _spawnLocations.Count);
                    Vector2 s = new Vector2(_spawnLocations[index].X, _spawnLocations[index].Y);
                    _spawnLocations.Remove(_spawnLocations[index]);
                    return s;
                }
                else
                {
                    var temp = _spawnLocations[_indexer];
                    _indexer++;
                    return new Vector2(temp.X, temp.Y);
                }
            }catch(Exception e)
            {
                Console.Error.WriteLine("ERROR: Could not distribute a spawn. Getting that standard shit. \n " + e);
                return new Vector2(100 + rnd.Next(-20,20), 100 + rnd.Next(-20,20));
            }
            
        }


    }
}
