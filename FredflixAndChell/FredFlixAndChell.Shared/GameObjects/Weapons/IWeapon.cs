using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.GameObjects.Weapons
{
    interface IWeapon
    {
        int Damage { get; set; }
        float Speed {get;set;}
        

        Vector2 Offset { get; set; }
        Vector2 BarrelOffset { get; set; }

    }
}
