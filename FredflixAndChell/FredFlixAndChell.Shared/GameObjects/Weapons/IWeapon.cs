using FredflixAndChell.Shared.Utilities;
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
        int Ammo { get; set; }
        int MaxAmmo { get; set; }
        int MagazineSize { get; set; }
        int MagazineAmmo { get; set; }
        float ReloadTime { get; set; }


        Vector2 Offset { get; set; }
        Vector2 BarrelOffset { get; set; }

        Cooldown Cooldown { get; set; }
        Cooldown Reload { get; set; }

    }
}
