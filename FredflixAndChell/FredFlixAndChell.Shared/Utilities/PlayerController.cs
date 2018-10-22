using FredflixAndChell.Shared.GameObjects;
using FredflixAndChell.Shared.GameObjects.Bullets;
using FredflixAndChell.Shared.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.Utilities
{
    public class PlayerController
    {
        private const short LEFT = 1;
        private const short UP = 2;
        private const short DOWN = 4;
        private const short RIGHT = 8;

        public Player Player;

        public PlayerIndex PlayerIndex;

        public GamePadUtility GamePad;

        private IScene _scene;
        private bool _playingWithController;
        private int _direction;


        public PlayerController(IScene scene, int controllerIndex)
        {
            _scene = scene;

            //TODO: Spawn n stuff
            Random rnd = new Random();
            Player = new Player(_scene, rnd.Next(50, 590), rnd.Next(50, 420));
            _scene.Spawn(Player);

            //Playing with controller
            if (controllerIndex > 0 && controllerIndex < 5)
            {
                _playingWithController = true;
                PlayerIndex = GamePadUtility.ConvertToIndex(controllerIndex);
                GamePad = new GamePadUtility(PlayerIndex);
            }
            else // Playing with keyboard
            {
                _playingWithController = false;

                KeyboardUtility.While(Keys.A, () => _direction |= LEFT, () => _direction &= (_direction - LEFT));
                KeyboardUtility.While(Keys.W, () => _direction |= UP, () => _direction &= (_direction - UP));
                KeyboardUtility.While(Keys.S, () => _direction |= DOWN, () => _direction &= (_direction - DOWN));
                KeyboardUtility.While(Keys.D, () => _direction |= RIGHT, () => _direction &= (_direction - RIGHT));

               

            }


        }

        public void Update()
        {
            //If index is null, then its a keyboard bro
            if (_playingWithController)
            {
                //if (gamePad == null) gamePad = new GamePadUtility(playerIndex);
                GamePad.Poll(Player.Actions);

                Player.FacingAngle = -(float)Math.Atan2(Player.Actions.AimY, Player.Actions.AimX);

                if (Player.Actions.Attack > 0)
                {
                    Player.Attack();

                }

            }
            else
            {
                if ((_direction & LEFT) > 0)
                    Player.Actions.MoveX = -1.0f;

                if ((_direction & RIGHT) > 0)
                    Player.Actions.MoveX = 1.0f;

                if ((_direction & LEFT) == 0 && (_direction & RIGHT) == 0)
                    Player.Actions.MoveX = 0;

                if ((_direction & DOWN) > 0)
                    Player.Actions.MoveY = -1.0f;

                if ((_direction & UP) > 0)
                    Player.Actions.MoveY = 1.0f;

                if ((_direction & DOWN) == 0 && (_direction & UP) == 0)
                    Player.Actions.MoveY = 0;

                /* REPLACE ? */
                if (Player.Actions.MoveY != 0 && Player.Actions.MoveX != 0)
                {
                    Player.Actions.MoveX = Player.Actions.MoveX / (float)Math.Sqrt(2);
                    Player.Actions.MoveY = Player.Actions.MoveY / (float)Math.Sqrt(2);
                }

                // Mouse aim
                var mouseState = Mouse.GetState();

                Vector2 mousePositionInMap = Vector2.Transform(mouseState.Position.ToVector2(), Matrix.Invert(_scene.Camera.GetViewMatrix()));

                Player.FacingAngle  = (float)Math.Atan2(mousePositionInMap.Y - Player.Position.Y, mousePositionInMap.X - Player.Position.X);

                Player.Actions.AimX = (float)Math.Cos(Player.FacingAngle);
                Player.Actions.AimY = (float)Math.Sin(Player.FacingAngle);


                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    Player.Attack();
                }
               

            }
        }

    }
}
