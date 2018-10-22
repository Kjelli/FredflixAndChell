using FredflixAndChell.Shared.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;

namespace FredflixAndChell.Shared.Utilities
{
    public class PlayerController : Component, IUpdatable
    {
        public float XAxis => _xAxisInput?.value ?? 0;
        public float YAxis => _yAxisInput?.value ?? 0;
        private VirtualAxis _xAxisInput;
        private VirtualAxis _yAxisInput;

        public Player Player;
        public PlayerIndex PlayerIndex;
        private int _controllerIndex;

        public PlayerController(int controllerIndex)
        {
            _controllerIndex = controllerIndex;
        }

        public override void onAddedToEntity()
        {
            base.onAddedToEntity();
            Player = entity.getComponent<Player>();

            _xAxisInput = new VirtualAxis();
            _yAxisInput = new VirtualAxis();

            _xAxisInput.nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Left, Keys.Right));
            _yAxisInput.nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Up, Keys.Down));

            //Playing with controller
            if (_controllerIndex > 0 && _controllerIndex < 5)
            {
                _xAxisInput.nodes.Add(new VirtualAxis.GamePadDpadLeftRight(0));
                _xAxisInput.nodes.Add(new VirtualAxis.GamePadLeftStickX(0));

                _yAxisInput.nodes.Add(new VirtualAxis.GamePadDpadUpDown(0));
                _yAxisInput.nodes.Add(new VirtualAxis.GamePadLeftStickY(0));
            }
        }

        public void update()
        {
        }
    }
}
