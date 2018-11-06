using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.Utilities.Input;
using Microsoft.Xna.Framework.Input;
using Nez;
using System;

namespace FredflixAndChell.Shared.Components.PlayerComponents
{
    public class PlayerController : Component, IUpdatable
    {

        private VirtualJoystick _leftStick;
        public VirtualJoystick _rightStick;
        private VirtualButton _fireButton;
        private VirtualButton _reloadButton;
        private VirtualButton _dropGunButton;
        private VirtualButton _debugButton;
        private VirtualButton _interactButton;
        private VirtualButton _switchWeaponButton;
        private VirtualButton _sprintButton;
        private VirtualMouseJoystick _mouseJoystick;

        private Player Player;

        private int _controllerIndex;
        private bool _inputEnabled = true;

        public float XLeftAxis => _leftStick?.value.X ?? 0;
        public float YLeftAxis => -1 * _leftStick?.value.Y ?? 0;
        public float XRightAxis => _rightStick?.value.X ?? 0;
        public float YRightAxis => -1 * _rightStick?.value.Y ?? 0;
        public bool FirePressed => _fireButton?.isDown ?? false;
        public bool ReloadPressed => _reloadButton?.isDown ?? false;
        public bool DropGunPressed => _dropGunButton?.isDown ?? false;
        public bool InteractPressed => _interactButton?.isDown ?? false;
        public bool DebugModePressed => _debugButton?.isPressed ?? false;
        public bool SwitchWeaponPressed => _switchWeaponButton?.isPressed ?? false;
        public bool SprintPressed => _sprintButton?.isDown ?? false;
        public bool InputEnabled => _inputEnabled;

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
            _reloadButton = new VirtualButton();
            _dropGunButton = new VirtualButton();
            _debugButton = new VirtualButton();
            _interactButton = new VirtualButton();
            _switchWeaponButton = new VirtualButton();
            _sprintButton = new VirtualButton();

            // Virtual mouse joystick
            _mouseJoystick = new VirtualMouseJoystick(Player.entity.position);

            // Keyboard player
            if (_controllerIndex == -1)
            {
                _leftStick.addKeyboardKeys(VirtualInput.OverlapBehavior.CancelOut, Keys.A, Keys.D, Keys.S, Keys.W);
                _rightStick.nodes.Add(_mouseJoystick);
                _fireButton.addMouseLeftButton();
                _reloadButton.addKeyboardKey(Keys.R);
                _interactButton.addKeyboardKey(Keys.E);
                _dropGunButton.addKeyboardKey(Keys.G);
                _debugButton.addKeyboardKey(Keys.F2);
                _switchWeaponButton.addKeyboardKey(Keys.Q);
                _sprintButton.addKeyboardKey(Keys.LeftShift);
            }
            // Player with controller at index {_controllerIndex}
            else if (GamePad.GetState(_controllerIndex).IsConnected)
            {
                var isControllerCompatible = VerifyControllerCompatible(_controllerIndex);
                if (!isControllerCompatible)
                {
                    throw new Exception("Controller not compatible");
                }
                _leftStick.addGamePadLeftStick(_controllerIndex);
                _rightStick.addGamePadRightStick(_controllerIndex);

                _fireButton.addGamePadButton(_controllerIndex, Buttons.RightTrigger);
                _interactButton.addGamePadButton(_controllerIndex, Buttons.A);
                _switchWeaponButton.addGamePadButton(_controllerIndex, Buttons.Y);
                _reloadButton.addGamePadButton(_controllerIndex, Buttons.X);
                _debugButton.addGamePadButton(_controllerIndex, Buttons.Start);
                _sprintButton.addGamePadButton(_controllerIndex, Buttons.B);
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

        public void SetInputEnabled(bool enabled)
        {
            _inputEnabled = enabled;
        }

        public void update()
        {
            if (_controllerIndex == -1)
            {
                _mouseJoystick.ReferencePoint = entity.scene.camera.worldToScreenPoint(Player.entity.position);
            }
        }
    }
}