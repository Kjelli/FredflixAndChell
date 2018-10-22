using FredflixAndChell.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FredflixAndChell.Shared.Utilities;
using FredflixAndChell.Shared.Utilities.Graphics;
using Nez;
using Nez.Sprites;

namespace FredflixAndChell.Shared.GameObjects
{
    public abstract class GameObject : Component, IGameObject, IUpdatable
    {
        private Vector2 _spawnPosition { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Size { get; set; }

        public GameObject(int x, int y, int width, int height)
        {
            _spawnPosition = new Vector2(x, y);
            Size = new Vector2(width, height); // does nothing
        }
        public abstract void OnDespawn();
        public abstract void OnSpawn();
        public abstract void update();
        public override void onAddedToEntity()
        {
            base.onAddedToEntity();
            OnSpawn();

            var sprite = entity.getComponent<Sprite>();
            if (sprite != null)
            {
                entity.position = _spawnPosition;
            }
        }

        public override void onRemovedFromEntity()
        {
            base.onRemovedFromEntity();
            OnDespawn();
        }
    }
}
