﻿using FredflixAndChell.Shared.GameObjects;
using MonoGame.Extended;
using MonoGame.Extended.Input.InputListeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FredflixAndChell.Shared.Utilities.Graphics.Cameras;

namespace FredflixAndChell.Shared.Scenes
{
    public class Scene : IScene
    {
        private List<GameObject> _gameObjects = new List<GameObject>();
        public SmoothCamera2D Camera { get; }

        public List<GameObject> GameObjects { get => _gameObjects; }

        public Scene(SmoothCamera2D camera)
        {
            Camera = camera;
        }

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

        public virtual void Update(GameTime gameTime)
        {
            GameObjects.ForEach(g => g.Update(gameTime));
            Camera.Update(gameTime);
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            GameObjects.ForEach(g => g.Draw(spriteBatch, gameTime));
        }

        public void DrawDebug(SpriteBatch spriteBatch, GameTime gameTime)
        {
            GameObjects.ForEach(g => g.DebugDraw(spriteBatch, gameTime));
        }
    }
}
