using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.GameObjects.Weapons
{
    public class GunSprite
    {
        public static readonly GunSprite M4 = "m4";
        public static readonly GunSprite Fido = "fido";

        private readonly string _spriteName;
        private GunSprite(string spriteName)
        {
            _spriteName = spriteName;
        }

        public static implicit operator GunSprite(string input)
        {
            return new GunSprite(input);
        }

        public override string ToString()
        {
            return _spriteName;
        }
    }
}
