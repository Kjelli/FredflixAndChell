using FredflixAndChell.Shared.GameObjects;
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
    class PlayerController
    {
        public Player player;

        public PlayerIndex playerIndex;

        public KeyboardUtility keyboard;
        public GamePadUtility gamePad;

        private IScene scene;
        private bool playingWithController;


        public PlayerController(IScene scene, int controllerIndex)
        {
            this.scene = scene;
          

            //TODO: Spawn n stuff
            Random rnd = new Random();
            player = new Player(rnd.Next(50,590), rnd.Next(50, 420));
            scene.Spawn(player);

            //Playing with controller
            if (controllerIndex > 0 && controllerIndex < 5)
            {
                playingWithController = true;
                playerIndex = GamePadUtility.ConvertToIndex(controllerIndex);
                gamePad = new GamePadUtility(playerIndex);
            }
            else // Playing with keyboard
            {
                playingWithController = false;
                keyboard = new KeyboardUtility();

                
            }
        }

        public void Update()
        {
            //If index is null, then its a keyboard bro
            if (playingWithController)
            {
                //if (gamePad == null) gamePad = new GamePadUtility(playerIndex);
                gamePad.Poll(this.player.actions);
            }
            else
            {
                keyboard.Poll(this.player);
            }
            
        }

    }
}
