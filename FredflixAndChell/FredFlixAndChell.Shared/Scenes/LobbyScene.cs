using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Systems;
using FredflixAndChell.Shared.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.UI;
using System.Collections.Generic;

namespace FredflixAndChell.Shared.Scenes
{
    class LobbyScene : Scene
    {
        private GameSettings _gameSettings;

        private readonly PrimitiveDrawable normalButtonColor;
        private readonly PrimitiveDrawable pressedButtonColor;
        private readonly PrimitiveDrawable hoverButtonColor;
        private readonly PrimitiveDrawable disabledButtonColor;
        private readonly TextButtonStyle normalButtonStyle;
        private readonly TextButtonStyle disabledButtonStyle;
        private UICanvas canvas;
        private Table _table;

        public LobbyScene()
        {
            _gameSettings = GameSettings.Default;

            addRenderer(new ScreenSpaceRenderer(100, Constants.Layers.HUD));
            addRenderer(new RenderLayerExcludeRenderer(0, Constants.Layers.HUD));

            // create our canvas and put it on the screen space render layer
            canvas = createEntity("ui").addComponent(new UICanvas());
            canvas.isFullScreen = true;
            canvas.renderLayer = Constants.Layers.HUD;

            clearColor = Color.Black;

            normalButtonColor = new PrimitiveDrawable(Color.Gray);
            pressedButtonColor = new PrimitiveDrawable(Color.Red);
            hoverButtonColor = new PrimitiveDrawable(Color.LightGray);
            disabledButtonColor = new PrimitiveDrawable(Color.Pink);

            normalButtonStyle = new TextButtonStyle(normalButtonColor, pressedButtonColor, hoverButtonColor);
            disabledButtonStyle = new TextButtonStyle(disabledButtonColor, disabledButtonColor, disabledButtonColor);

            SetupSceneSelector();
        }

        private void SetupSceneSelector()
        {
            _table = canvas.stage.addElement(new Table());
            _table.setFillParent(true).center();

            var label = new Label("Ultimate Brodown");
            _table.add(label);

            _table.row().setPadTop(10);
            var newGameButton = _table.add(new TextButton("New Game", normalButtonStyle)).setFillX().setMinHeight(30).getElement<TextButton>();
            newGameButton.onClicked += ShowSelectPlayers;

            _table.row().setPadTop(10);
            var exitButton = _table.add(new TextButton("Exit", normalButtonStyle)).setFillX().setMinHeight(30).getElement<TextButton>();
            exitButton.onClicked += b => Core.exit();

            newGameButton.gamepadDownElement = exitButton;
            exitButton.gamepadUpElement = newGameButton;

            canvas.stage.setGamepadFocusElement(newGameButton);
        }

        private void ShowSelectPlayers(Button obj)
        {
            _table.clear();
            _table = canvas.stage.addElement(new Table());
            _table.setFillParent(true).center();

            var label = new Label("How many players?");
            _table.add(label);

            _table.row().setPadTop(10);

            // TODO: limit maps based on number of players here?
            var twoPlayersbutton = _table.add(new TextButton("2 players", normalButtonStyle)).setFillX().setMinHeight(30).getElement<TextButton>();
            twoPlayersbutton.onClicked += btn =>
            {
                ShowSelectMap(btn);
            };

            _table.row().setPadTop(10);
            var threePlayersButton = _table.add(new TextButton("3 players", normalButtonStyle)).setFillX().setMinHeight(30).getElement<TextButton>();
            threePlayersButton.onClicked += btn =>
            {
                ShowSelectMap(btn);
            };

            _table.row().setPadTop(10);
            var fourPlayersButton = _table.add(new TextButton("4 players", normalButtonStyle)).setFillX().setMinHeight(30).getElement<TextButton>();
            fourPlayersButton.onClicked += btn =>
            {
                ShowSelectMap(btn);
            };

            twoPlayersbutton.gamepadDownElement = threePlayersButton;
            threePlayersButton.gamepadUpElement = twoPlayersbutton;
            threePlayersButton.gamepadDownElement = fourPlayersButton;
            fourPlayersButton.gamepadUpElement = threePlayersButton;

            canvas.stage.setGamepadFocusElement(twoPlayersbutton);

            DisableButtonsBasedOnNumPlayers(threePlayersButton, fourPlayersButton);
        }

        private void DisableButtonsBasedOnNumPlayers(Button threePlayersButton, Button fourPlayersButton)
        {
            var _connectedPlayers = new List<int>();
            for (var playerIndex = 0; playerIndex < 4; playerIndex++)
            {
                var gamePadState = GamePad.GetState(playerIndex);

                if (gamePadState.IsConnected && !_connectedPlayers.Contains(playerIndex))
                {
                    _connectedPlayers.Add(playerIndex);
                }
            }

            if (!_connectedPlayers.Contains(-1))
            {
                _connectedPlayers.Add(-1);
            }

            if (_connectedPlayers.Count == 2)
            {
                threePlayersButton.setDisabled(true);
                threePlayersButton.setStyle(disabledButtonStyle);
                fourPlayersButton.setDisabled(true);
                fourPlayersButton.setStyle(disabledButtonStyle);
            }

            if (_connectedPlayers.Count == 3)
            {
                fourPlayersButton.setDisabled(true);
                fourPlayersButton.setStyle(disabledButtonStyle);
            }
        }

        private void ShowSelectMap(Button obj)
        {
            _table.clear();
            _table = canvas.stage.addElement(new Table());
            _table.setFillParent(true).center();

            var label = new Label("Select map");
            _table.add(label);

            _table.row().setPadTop(10);
            var dungeonButton = _table.add(new TextButton("Dungeon", normalButtonStyle)).setFillX().setMinHeight(30).getElement<TextButton>();
            dungeonButton.onClicked += btn =>
            {
                _gameSettings.Map = "dungeon_2";
                Core.startSceneTransition(new FadeTransition(() => new BroScene(_gameSettings)));
            };

            _table.row().setPadTop(10);

            // Mister Winterbottom!
            var winterButton = _table.add(new TextButton("Winter", normalButtonStyle)).setFillX().setMinHeight(30).getElement<TextButton>();
            winterButton.onClicked += btn =>
            {
                _gameSettings.Map = "winter_1";
                Core.startSceneTransition(new FadeTransition(() => new BroScene(_gameSettings)));
            };

            dungeonButton.gamepadDownElement = winterButton;
            winterButton.gamepadUpElement = dungeonButton;

            canvas.stage.setGamepadFocusElement(dungeonButton);
        }
    }
}
