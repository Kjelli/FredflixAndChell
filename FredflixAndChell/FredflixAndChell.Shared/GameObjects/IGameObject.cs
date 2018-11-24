using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FredflixAndChell.Shared.GameObjects
{
    public interface IGameObject
    {
        void OnSpawn();
        void OnDespawn();

    }
}
