using FredflixAndChell.Components.Players;
using Microsoft.Xna.Framework.Input;
using Nez;
using System;
using System.Collections.Generic;
using static FredflixAndChell.Shared.Components.HUD.DebugHud;

namespace FredflixAndChell.Shared.Systems
{

    public class ControllerSystem : SceneComponent
    {
        private List<GamePadData> _connectedControllers = new List<GamePadData>();
        private PlayerConnector _playerConnector;

        public ControllerSystem(int maxPlayersSupported = 4)
        {
            Input.maxSupportedGamePads = maxPlayersSupported;
        }

        public override void onEnabled()
        {
            base.onEnabled();
            _playerConnector = scene.getSceneComponent<PlayerConnector>();
            CheckConnectedGamepads();
            SetupDebug(checkOnlyConnected: true);
        }

        private void CheckConnectedGamepads()
        {
            //return;
            foreach (var gamePad in Input.gamePads)
            {
                if (gamePad.isConnected())
                {
                    _connectedControllers.Add(gamePad);
                    _playerConnector.ControllerConnected(gamePad);
                }
            }
        }

        private void SetupDebug(bool checkOnlyConnected = false)
        {
            var gameSystem = scene.getSceneComponent<GameSystem>();
            var controllerDebug = new DebugLine
            {
                Text = () => $"Controller data",
                SubLines = new List<DebugLine>()
            };

            int index = 0;
            foreach (var gamepad in Input.gamePads)
            {
                if (checkOnlyConnected && !gamepad.isConnected()) continue;
                index++;
                var thisIndex = index;
                var gamePadDebug = new DebugLine()
                {
                    Text = () => $"Gamepad {thisIndex}",
                    SubLines = new List<DebugLine>()
                };

                var buttons = Enum.GetValues(typeof(Buttons));

                // Debug print all button states
                foreach (var button in buttons)
                {
                    gamePadDebug.SubLines.Add(new DebugLine()
                    {
                        Text = () => $"{button}: {gamepad.isButtonDown((Buttons)button)}"
                    });
                    break; // Remove to get all buttons
                }

                gamePadDebug.SubLines.Add(new DebugLine()
                {
                    Text = () => $"Connected: {gamepad.isConnected()}"
                });

                controllerDebug.SubLines.Add(gamePadDebug);
            }

            gameSystem.DebugLines.Add(controllerDebug);
        }

        public override void update()
        {
            base.update();
        }
    }
}