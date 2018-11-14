using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Utilities;
using Microsoft.Xna.Framework;
using Nez;
using Nez.UI;

namespace FredflixAndChell.Shared.Scenes
{
    class LobbyScene : Scene
    {
        private readonly PrimitiveDrawable normalButtonColor;
        private readonly PrimitiveDrawable pressedButtonColor;
        private readonly PrimitiveDrawable hoverButtonColor;
        private readonly TextButtonStyle buttonStyle;
        public UICanvas canvas;
        Table _table;

        public LobbyScene()
        {
            addRenderer(new ScreenSpaceRenderer(100, Constants.Layers.HUD));
            addRenderer(new RenderLayerExcludeRenderer(0, Constants.Layers.HUD));

            // create our canvas and put it on the screen space render layer
            canvas = createEntity("ui").addComponent(new UICanvas());
            canvas.isFullScreen = true;
            canvas.renderLayer = Constants.Layers.HUD;

            clearColor = Color.Black;

            normalButtonColor = new PrimitiveDrawable(Color.DarkGray);
            pressedButtonColor = new PrimitiveDrawable(Color.Red);
            hoverButtonColor = new PrimitiveDrawable(Color.LightGray);

            buttonStyle = new TextButtonStyle(normalButtonColor, pressedButtonColor, hoverButtonColor);

            SetupSceneSelector();
        }

        private void SetupSceneSelector()
        {
            _table = canvas.stage.addElement(new Table());

            _table.setFillParent(true).center();

            var label = new Label("Ultimate Brodown");
            _table.add(label);

            _table.row().setPadTop(10);
            var button = _table.add(new TextButton("New Game", buttonStyle)).setFillX().setMinHeight(30).getElement<TextButton>();

            button.onClicked += ShowSelectMap;

            _table.row().setPadTop(10);
            button = _table.add(new TextButton("Exit", buttonStyle)).setFillX().setMinHeight(30).getElement<TextButton>();
            button.onClicked += b => Core.exit();
        }

        private void ShowSelectMap(Button obj)
        {
            _table.clear();
            _table = canvas.stage.addElement(new Table());

            _table.setFillParent(true).center();

            var label = new Label("Select map");
            _table.add(label);

            _table.row().setPadTop(10);
            var button = _table.add(new TextButton("Dungeon", buttonStyle)).setFillX().setMinHeight(30).getElement<TextButton>();

            button.onClicked += ShowSelectMap;

            button.onClicked += btn =>
            {
                MapHelper.CurrentMap = "dungeon_1";
                Core.startSceneTransition(new FadeTransition(() => new BroScene()));
            };

            _table.row().setPadTop(10);
            button = _table.add(new TextButton("Winter", buttonStyle)).setFillX().setMinHeight(30).getElement<TextButton>();
            button.onClicked += btn =>
            {
                MapHelper.CurrentMap = "winter_1";
                Core.startSceneTransition(new FadeTransition(() => new BroScene()));
            };
        }
    }
}
