using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.Utilities
{
    public class GamePadUtility
    {

        private static GamePadCapabilities capabilities;

        private static PlayerIndex PlayerIndex;

        public GamePadUtility(PlayerIndex index)
        {
            PlayerIndex = index;
            capabilities = GamePad.GetCapabilities(PlayerIndex);

        }

        public void Poll(InputActions action)
        {
            //TODO? Fix player indexing
            var state = GamePad.GetState(PlayerIndex.One);

            //Left Movement
            if (capabilities.HasLeftXThumbStick && capabilities.HasLeftYThumbStick)
            {
                action.MoveX = state.ThumbSticks.Left.X;
                action.MoveY = state.ThumbSticks.Left.Y;
            }

            if(capabilities.HasRightXThumbStick && capabilities.HasRightYThumbStick && (state.ThumbSticks.Right.X != 0 || state.ThumbSticks.Right.Y != 0))
            {
                action.AimX = state.ThumbSticks.Right.X;
                action.AimY = state.ThumbSticks.Right.Y;
            }

            if (capabilities.HasRightTrigger)
            {
                action.Attack = state.Triggers.Right;
            }

        }

        public static PlayerIndex ConvertToIndex(int n)
        {
            switch (n)
            {
                case 1:
                    return PlayerIndex.One;
                case 2:
                    return PlayerIndex.Two;
                case 3:
                    return PlayerIndex.Three;
                case 4:
                    return PlayerIndex.Four;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

    }
}
