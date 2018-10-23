using FredflixAndChell.Shared.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using System;

namespace FredflixAndChell.Shared.Components
{
    public class PlayerController : Component, IUpdatable
    {
        public float XLeftAxis => _xLeftAxisInput?.value ?? 0;
        public float YLeftAxis => _yLeftAxisInput?.value ?? 0;
        public float XRightAxis => _xRightAxisInput?.value ?? 0;
        public float YRightAxis => _yRightAxisInput?.value ?? 0;
        public bool FirePressed => _fireButton?.isDown ?? false;
        public bool ReloadPressed => _reload?.isDown ?? false;

        private VirtualAxis _xLeftAxisInput;
        private VirtualAxis _yLeftAxisInput;
        private VirtualAxis _xRightAxisInput;
        private VirtualAxis _yRightAxisInput;
        private VirtualButton _fireButton;
        private VirtualButton _reload;

        // Only used when not playing with controller
        private VirtualMouseXAxis _mouseXAxis;
        private VirtualMouseYAxis _mouseYAxis;

        public Player Player;
        public PlayerIndex PlayerIndex;
        private int _controllerIndex;
        private bool _isPlayingWithController = false;

        public PlayerController(int controllerIndex)
        {
            _controllerIndex = 1;
        }

        public override void onAddedToEntity()
        {
            base.onAddedToEntity();
            Player = entity.getComponent<Player>();

            // Movement Axes
            _xLeftAxisInput = new VirtualAxis();
            _yLeftAxisInput = new VirtualAxis();

            // Aiming Axes
            _xRightAxisInput = new VirtualAxis();
            _yRightAxisInput = new VirtualAxis();

            // Fire button
            _fireButton = new VirtualButton();

            //Reload
            _reload = new VirtualButton();

            _xLeftAxisInput.nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.A, Keys.D));
            _yLeftAxisInput.nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.W, Keys.S));
            _fireButton.addMouseLeftButton();
            _reload.addKeyboardKey(Keys.R);


            //Playing with controller
            if (_controllerIndex > 0 && _controllerIndex < 5)
            {
                _isPlayingWithController = true;

                //_xLeftAxisInput.nodes.Add(new VirtualAxis.GamePadDpadLeftRight(0));
                //_yLeftAxisInput.nodes.Add(new VirtualAxis.GamePadDpadUpDown(0));


                _xLeftAxisInput.nodes.Add(new VirtualAxis.GamePadLeftStickX(0));
                _yLeftAxisInput.nodes.Add(new VirtualAxis.GamePadLeftStickY(0));

                _xRightAxisInput.nodes.Add(new VirtualAxis.GamePadRightStickX(0));
                _yRightAxisInput.nodes.Add(new VirtualAxis.GamePadRightStickY(0));

                _fireButton.addGamePadButton(0, Buttons.RightTrigger);
                _reload.addGamePadButton(0, Buttons.X);


            }
            else
            {
                _mouseXAxis = new VirtualMouseXAxis(Player.entity.position);
                _mouseYAxis = new VirtualMouseYAxis(Player.entity.position);
                _xRightAxisInput.nodes.Add(_mouseXAxis);
                _yRightAxisInput.nodes.Add(_mouseYAxis);
            }
        }
        public void update()
        {
            if (!_isPlayingWithController)
            {
                _mouseXAxis.ReferencePoint = entity.scene.camera.worldToScreenPoint(Player.entity.position);
                _mouseYAxis.ReferencePoint = entity.scene.camera.worldToScreenPoint(Player.entity.position);
            }
        }
    }
}