using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Systems;
using FredflixAndChell.Shared.Systems.GameModeHandlers;
using FredflixAndChell.Shared.Utilities;
using Nez;
using Nez.UI;
using System;

namespace FredflixAndChell.Shared.Scenes
{
    public class HubScene : BroScene
    {
        private VignettePostProcessor _grayscalePostProcessor;
        private UICanvas _canvas;
        private Cooldown _grayscaleDelay;

        public HubScene() : base(new GameSettings
        {
            GameMode = GameMode.HUB,
            Map = "winter_hub",
            DamageMultiplier = 0.0f,
            KnockbackMultiplier = 0.0f
        })
        {
            _grayscaleDelay = new Cooldown(2, true);
        }

        public override void initialize()
        {
            base.initialize();
            if (ContextHelper.IsGameInitialized)
                return;

            Console.WriteLine("Initializing HubScene");
            var transition = new FadeTransition
            {
                fadeInDuration = 2,
                onTransitionCompleted = ShowTitleOverlay
            };

            Core.startSceneTransition(transition);

            _grayscalePostProcessor = addPostProcessor(new VignettePostProcessor(5));
            _grayscalePostProcessor.effect = AssetLoader.GetEffect("grayscale_shader");
            _grayscalePostProcessor.effect.Parameters["intensity"].SetValue((float)1);

            ContextHelper.IsGameInitialized = true;
        }

        public override void Setup()
        {
            base.Setup();
            Console.WriteLine("Setting up Hubscene");
        }

        public override void update()
        {
            base.update();
            _grayscaleDelay.Update();
            if (!_grayscaleDelay.IsReady())
            {
                Console.WriteLine("Reducing grayscale...");
                var delta = _grayscaleDelay.ElapsedNormalized();
                _grayscalePostProcessor.effect.Parameters["intensity"].SetValue(delta);
            }
        }

        private void ShowTitleOverlay()
        {
            // create our canvas and put it on the screen space render layer
            _canvas = createEntity("ui").addComponent(new UICanvas());
            _canvas.isFullScreen = true;
            _canvas.renderLayer = Constants.Layers.HUD;

            var table = _canvas.stage.addElement(new Table());
            table.setFillParent(true).center();

            var label = new Label("Ultimate Brodown");
            label.setFontScale(10);
            table.add(label);

            Core.schedule(3, (s) =>
            {
                _canvas.removeComponent();
                _grayscaleDelay.Start();
            });
        }
    }
}
