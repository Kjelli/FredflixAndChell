﻿using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace FredflixAndChell.Shared.Utilities.Input
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
                if (state.IsKeyDown(key) && (_oldState.IsKeyUp(key) || keyAction.Repeat))
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

        public static void While(Keys key, Action action, Action released = null, bool repeat = false)
        {
            _keyActions.Add(new KeyAction { Key = key, Pressed = action, Released = released, Repeat = repeat });
        }

        private class KeyAction
        {
            public bool Repeat;
            public Keys Key;
            public Action Pressed;
            public Action Released;
        }
    }
}
