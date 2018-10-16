using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.GameObjects.Bullets;
using FredflixAndChell.Shared.Scenes;
using FredflixAndChell.Shared.Utilities;
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

        public Cooldown Cooldown { get; set; }

        public Gun(Player owner, IScene Scene, int x, int y, int width, int height, float cooldown) : base(Scene,x,y,width,height)
        {
            _sprite = AssetLoader.GetTexture("gun_m4");
           
            BarrelOffset = new Vector2(y,x+10);

            _player = owner;

            Scene.Spawn(this);

            Cooldown = new Cooldown(cooldown);

            Console.WriteLine("Width : " + width + " - Height: " + height );
        }


        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
             
            Rectangle sourceRectanble = new Rectangle(0,0,(int)Size.X, (int)Size.Y);
            Vector2 origin = new Vector2(Size.X/2, Size.Y/2);
            
            spriteBatch.Draw(_sprite, destinationRectangle: Bounds, rotation:_player.FacingAngle ,effects: (_player.HorizontalFacing == (int)FacingCode.LEFT ? SpriteEffects.FlipVertically : SpriteEffects.None));

        }

        public void Fire()
        {
            if (Cooldown.IsReady())
            {
                Bullet bullet = new Bullet(_player, Position.X, Position.Y, 32, 32, _player.FacingAngle, 1.0f, 30.0f);
                Scene.Spawn(bullet);
                Cooldown.Start();
            }
            
        }

       

        public override void Update(GameTime gameTime)
        {
            
            Position = new Vector2(_player.Position.X + _player.Size.X /2, _player.Position.Y + _player.Size.Y / 2);
            Cooldown.Update(gameTime);
        }

        public override void OnDespawn()
        {
        }


        public override void OnSpawn()
        {
        }
    }
}
