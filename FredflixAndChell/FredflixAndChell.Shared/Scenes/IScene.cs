using FredflixAndChell.Shared.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.Scenes
{
    public interface IScene
    {

        List<GameObject> GameObjects { get; }

        void Spawn(GameObject gameObject);
        void Despawn(GameObject gameObject);
    }
}
