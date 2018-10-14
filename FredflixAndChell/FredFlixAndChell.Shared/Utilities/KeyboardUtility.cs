using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using FredflixAndChell.Shared.GameObjects;

namespace FredflixAndChell.Shared.Utilities
{
    public class KeyboardUtility
    {
        private static List<KeyAction> _keyActions;
        private static KeyboardState _oldState;

        ///KeyBoard help variables
        private int _direction;

        private const short LEFT = 1;
        private const short UP = 2;
        private const short DOWN = 4;
        private const short RIGHT = 8;

        public KeyboardUtility()
        {
            _keyActions = new List<KeyAction>();

            While(Keys.A, () => _direction |= LEFT, () => _direction &= (_direction - LEFT));
            While(Keys.W, () => _direction |= UP, () => _direction &= (_direction - UP));
            While(Keys.S, () => _direction |= DOWN, () => _direction &= (_direction - DOWN));
            While(Keys.D, () => _direction |= RIGHT, () => _direction &= (_direction - RIGHT));
        }

        public void Poll(Player p)
        {
            var state = Keyboard.GetState();
            foreach (var keyAction in _keyActions)
            {
                var key = keyAction.Key;
                if (state.IsKeyDown(key) && _oldState.IsKeyUp(key))
                {
                    keyAction.Pressed?.Invoke();
                    Console.WriteLine(key + " pressed");
                }
                else if (state.IsKeyUp(key) && _oldState.IsKeyDown(key))
                {
                    keyAction.Released?.Invoke();
                    Console.WriteLine(key + " released");
                }
            }
            _oldState = state;


            if ((_direction & LEFT) > 0)
                p.actions.MoveX = -1.0f;
 
            if ((_direction & RIGHT) > 0)
                p.actions.MoveX = 1.0f;
            
            if((_direction & LEFT) == 0 && (_direction & RIGHT) == 0)
                p.actions.MoveX = 0;

            if ((_direction & DOWN) > 0)
                p.actions.MoveY = -1.0f;
           
            if ((_direction & UP) > 0)
                p.actions.MoveY = 1.0f;

            if ((_direction & DOWN) == 0 && (_direction & UP) == 0)
                p.actions.MoveY = 0;

            if(p.actions.MoveY != 0 && p.actions.MoveX != 0)
            {
             
                p.actions.MoveX = p.actions.MoveX / (float)Math.Sqrt(2);
                p.actions.MoveY = p.actions.MoveY / (float)Math.Sqrt(2);
            }

            //Mouse 
            var mouseState = Mouse.GetState();
            


        }
        public void While(Keys key, Action action, Action released = null)
        {
            _keyActions.Add(new KeyAction { Key = key, Pressed = action, Released = released });
        }

        private class KeyAction
        {
            public Keys Key;
            public Action Pressed;
            public Action Released;
        }
    }
}
