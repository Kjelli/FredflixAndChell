using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.Utilities.Input;
using Microsoft.Xna.Framework.Input;
using Nez;
using System;
using static Nez.VirtualJoystick;

namespace FredflixAndChell.Shared.Components.PlayerComponents
{
    public class PlayerController : Component, IUpdatable
    {
        private VirtualJoystick _leftStick;
        private VirtualJoystick _rightStick;
        private VirtualButton _fireButton;
        private VirtualButton _reloadButton;
        private VirtualButton _dropGunButton;
        private VirtualButton _debugButton;
        private VirtualButton _interactButton;
        private VirtualButton _switchWeaponButton;
        private VirtualButton _sprintButton;
        private VirtualMouseJoystick _mouseJoystick;

        private Player _player;

        private GamePadData _gp;
        private bool _inputEnabled = true;

        public float XLeftAxis => _leftStick?.value.X ?? _gp.getLeftStick().X;
        public float YLeftAxis => -1 * _leftStick?.value.Y ?? -1 * _gp.getLeftStick().Y;
        public float XRightAxis => _rightStick?.value.X ?? _gp.getRightStick().X;
        public float YRightAxis => -1 * _rightStick?.value.Y ?? -1 * _gp.getRightStick().Y;
        public bool FirePressed => _fireButton?.isDown ?? _gp.isButtonDown(Buttons.RightTrigger);
        public bool ReloadPressed => _reloadButton?.isPressed ?? _gp.isButtonPressed(Buttons.X);
        public bool DropGunPressed => _dropGunButton?.isPressed ?? _gp.isButtonPressed(Buttons.DPadUp);
        public bool InteractPressed => _interactButton?.isPressed ?? _gp.isButtonPressed(Buttons.A);
        public bool DebugModePressed => _debugButton?.isPressed ?? _gp.isButtonPressed(Buttons.Start);
        public bool SwitchWeaponPressed => _switchWeaponButton?.isPressed ?? _gp.isButtonPressed(Buttons.Y);
        public bool SprintPressed => _sprintButton?.isPressed ?? _gp.isButtonPressed(Buttons.B);
        public bool SprintDown => _sprintButton?.isDown ?? _gp.isButtonDown(Buttons.B);
        public bool InputEnabled => _inputEnabled;

        public PlayerController(GamePadData gamePadData)
        {
            _gp = gamePadData;
        }

        public override void onAddedToEntity()
        {
            _player = entity as Player;

            // Keyboard player
            if (_gp == null)
            {
                // Virtual mouse joystick
                _mouseJoystick = new VirtualMouseJoystick(_player.position);

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
        }

        public void SetInputEnabled(bool enabled)
        {
            _inputEnabled = enabled;
        }

        public void update()
        {
            if (_gp == null)
            {
                _mouseJoystick.ReferencePoint = entity.scene.camera.worldToScreenPoint(_player.position);
            }
        }
    }
}