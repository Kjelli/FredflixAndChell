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
    public abstract class GameObject : Entity, IGameObject
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
        public abstract void Update();
        public override void onAddedToScene()
        {
            position = _spawnPosition;
            if (!_hasSpawned)
            {
                OnSpawn();
                _hasSpawned = true;
            }
        }

        public override void onRemovedFromScene()
        {
            base.onRemovedFromScene();
            OnDespawn();
        }

        public override void update()
        {
            base.update();
            Update();
        }
    }
}
