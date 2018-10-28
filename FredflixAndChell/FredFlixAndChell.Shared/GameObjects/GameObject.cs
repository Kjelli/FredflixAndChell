using FredflixAndChell.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FredflixAndChell.Shared.Utilities;
using Nez;
using Nez.Sprites;

namespace FredflixAndChell.Shared.GameObjects
{
    public abstract class GameObject : Component, IGameObject, IUpdatable
    {
        private bool _hasSpawned;
        protected bool HasSpawned => _hasSpawned;
        private Vector2 _spawnPosition { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Size { get; set; }

        public GameObject(int x, int y)
        {
            _spawnPosition = new Vector2(x, y);
        }
        public abstract void OnDespawn();
        public abstract void OnSpawn();
        public abstract void update();
        public override void onAddedToEntity()
        {
            base.onAddedToEntity();
            if (!_hasSpawned)
            {
                OnSpawn();
                _hasSpawned = true;
            }

            entity.position = _spawnPosition;
        }

        public override void onRemovedFromEntity()
        {
            base.onRemovedFromEntity();
            OnDespawn();
        }
    }
}
