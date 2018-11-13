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

        public GameObject(float x, float y)
        {
            _spawnPosition = new Vector2(x, y);
        }
        public abstract void OnDespawn();
        public abstract void OnSpawn();
        public abstract void update();
        public override void onAddedToEntity()
        {
            entity.position = _spawnPosition;
            base.onAddedToEntity();
            if (!_hasSpawned)
            {
                OnSpawn();
                _hasSpawned = true;
            }

        }

        public override void onRemovedFromEntity()
        {
            base.onRemovedFromEntity();
            OnDespawn();
        }
    }
}
