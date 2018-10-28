using System;
using FredflixAndChell.Shared.GameObjects;
using FredflixAndChell.Shared.Utilities.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;

namespace FredflixAndChell.Shared.Components.PlayerComponents
{
    public class PlayerController : Component, IUpdatable
    {
        public float XLeftAxis => _leftStick?.value.X ?? 0;
        public float YLeftAxis => -1 * _leftStick?.value.Y ?? 0;
        public float XRightAxis => _rightStick?.value.X ?? 0;
        public float YRightAxis => -1 * _rightStick?.value.Y ?? 0;
        public bool FirePressed => _fireButton?.isDown ?? false;
        public bool ReloadPressed => _reload?.isDown ?? false;

        private VirtualJoystick _leftStick;
        private VirtualJoystick _rightStick;
        private VirtualButton _fireButton;
        private VirtualButton _reload;
        private VirtualMouseJoystick _mouseJoystick;
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

            // Virtual joysticks
            _leftStick = new VirtualJoystick(false);
            _rightStick = new VirtualJoystick(true);

            // Buttons
            _fireButton = new VirtualButton();
            _reload = new VirtualButton();

            // Virtual mouse joystick
            _mouseJoystick = new VirtualMouseJoystick(Player.entity.position);

            // Keyboard player
            if (_controllerIndex == 0)
            {
                _leftStick.addKeyboardKeys(VirtualInput.OverlapBehavior.CancelOut, Keys.A, Keys.D, Keys.S, Keys.W);
                _rightStick.nodes.Add(_mouseJoystick);
                _fireButton.addMouseLeftButton();
                _reload.addKeyboardKey(Keys.R);
                
            }
            // Player with controller at index {_controllerIndex}
            else if(GamePad.GetState(_controllerIndex).IsConnected)
            {
                var isControllerCompatible = VerifyControllerCompatible(_controllerIndex);
                if (!isControllerCompatible)
                {
                    throw new Exception("Controller not compatible");
                }
                _leftStick.addGamePadLeftStick(_controllerIndex);
                _rightStick.addGamePadRightStick(_controllerIndex);

                _fireButton.addGamePadButton(_controllerIndex, Buttons.RightTrigger);
                _reload.addGamePadButton(_controllerIndex, Buttons.X);

            }
            // Ghost player..?
            else
            {
                throw new Exception("What");
            }
        }

        private bool VerifyControllerCompatible(int controllerIndex)
        {
            var capabilities = GamePad.GetCapabilities(controllerIndex);
            var isCompatible = capabilities.HasLeftXThumbStick
                && capabilities.HasLeftYThumbStick
                && capabilities.HasRightXThumbStick
                && capabilities.HasRightYThumbStick
                && capabilities.HasXButton
                && capabilities.HasRightTrigger;

            return isCompatible;
        }

        public void update()
        {
            if (_controllerIndex == 0)
            {
                _mouseJoystick.ReferencePoint = entity.scene.camera.worldToScreenPoint(Player.entity.position);
            }

        }
    }
}