using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace FredflixAndChell.Shared.Utilities
{
    public static class KeyboardUtility
    {
        private static List<KeyAction> _keyActions = new List<KeyAction>();
        private static KeyboardState _oldState;

        public static void Poll()
        {
            var state = Keyboard.GetState();
            foreach (var keyAction in _keyActions)
            {
                var key = keyAction.Key;
                if (state.IsKeyDown(key) && _oldState.IsKeyUp(key))
                {
                    keyAction.Pressed?.Invoke();
                }
                else if (state.IsKeyUp(key) && _oldState.IsKeyDown(key))
                {
                    keyAction.Released?.Invoke();
                }
            }
            _oldState = state;
        }

        public static void While(Keys key, Action action, Action released = null)
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
