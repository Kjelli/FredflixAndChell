using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.GameObjects.Bullets;
using FredflixAndChell.Shared.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FredflixAndChell.Shared.GameObjects.Weapons
{
    public class Gun : GameObject, IWeapon
    {
        public int Damage { get; set; }
        public float Speed { get; set; }
        public Vector2 Offset { get; set; }
        public Vector2 BarrelOffset { get; set; }

        private Texture2D _sprite;
      

        private Player _player;

        public Gun(Player owner, IScene Scene, int x, int y, int width, int height) : base(Scene,x,y,width,height)
        {
            _sprite = AssetLoader.GetTexture("gun_m4");
           
            BarrelOffset = new Vector2(y,x+10);

            _player = owner;

            Scene.Spawn(this);
        }


        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {

            Rectangle sourceRectanble = new Rectangle(0,0,(int)Size.X, (int)Size.Y);
            Vector2 origin = new Vector2(Size.X/2, 0);

            spriteBatch.Draw(_sprite,position: Position, origin:origin, rotation:_player.FacingAngle);

        }

        public void Fire()
        {
            Bullet bullet = new Bullet(_player, Position.X,Position.Y, 32, 32, _player.FacingAngle, 10.0f, 30.0f);
            Scene.Spawn(bullet);
        }

       

        public override void Update(GameTime gameTime)
        {
            Position = new Vector2(_player.Position.X, _player.Position.Y);
        }

        public override void OnDespawn()
        {
        }

        public override void OnSpawn()
        {
        }
    }
}
