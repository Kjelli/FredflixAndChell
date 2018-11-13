using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.Utilities
{
    public class PlayerSpawner
    {
        public string ID { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Counter { get; set; }
        
        public PlayerSpawner(string name,int x, int y)
        {
            this.X = x;
            this.Y = y;

        }
    }
}
