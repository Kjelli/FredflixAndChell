using FredflixAndChell.Shared.Assets;
using Microsoft.Xna.Framework;
using Nez;
using Nez.UI;

namespace FredflixAndChell.Shared.Scenes
{
    class LobbyScene : Scene
    {
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
            SetupSceneSelector();
        }

        private void SetupSceneSelector()
        {
            _table = canvas.stage.addElement(new Table());

            _table.setFillParent(true).center();
            //_table.setDebug(true);

            var normalButtonColor = new PrimitiveDrawable(Color.DarkGray);
            var pressedButtonColor = new PrimitiveDrawable(Color.Red);
            var hoverButtonColor = new PrimitiveDrawable(Color.LightGray);

            var buttonStyle = new TextButtonStyle(normalButtonColor, pressedButtonColor, hoverButtonColor)
            {
                downFontColor = Color.Black
            };

            var label = new Label("Ultimate Brodown");
            _table.add(label);

            _table.row().setPadTop(10);
            var button = _table.add(new TextButton("New Game", buttonStyle)).setFillX().setMinHeight(30).getElement<TextButton>();
            button.onClicked += btn =>
            {
                Core.startSceneTransition(new FadeTransition(() => new BroScene()));
            };

            _table.row().setPadTop(10);
            button = _table.add(new TextButton("Exit", buttonStyle)).setFillX().setMinHeight(30).getElement<TextButton>();
            button.onClicked += btn =>
            {
                Core.exit();
            };
        }
    }
}
