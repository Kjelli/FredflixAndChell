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
        public Vector2 Velocity { get; set; }
        public Vector2 Size { get; set; }

        public GameObject() : this(16, 16)
        {
        }

        public GameObject(int width, int height)
        {
            Size = new Vector2(width, height);
        }

        public abstract void OnDespawn();
        public abstract void OnSpawn();
        public abstract void update();
        public override void onEnabled()
        {
            base.onEnabled();
            OnSpawn();
            var sprite = entity.getComponent<Sprite>();
        }

        public override void onDisabled()
        {
            base.onDisabled();
            OnDespawn();
        }
    }
}
