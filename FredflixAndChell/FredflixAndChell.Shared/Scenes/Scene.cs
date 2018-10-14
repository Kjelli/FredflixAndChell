using FredflixAndChell.Shared.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.Scenes
{
    public class Scene : IScene
    {
        private List<GameObject> _gameObjects = new List<GameObject>();
        public List<GameObject> GameObjects { get => _gameObjects; }

        public void Spawn(GameObject gameObject)
        {
            GameObjects.Add(gameObject);
            gameObject?.OnSpawn();
        }

        public void Despawn(GameObject gameObject)
        {
            GameObjects.Remove(gameObject);
            gameObject?.OnDespawn();
        }
    }
}
