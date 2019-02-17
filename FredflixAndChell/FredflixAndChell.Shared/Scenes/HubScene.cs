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
        private VignettePostProcessor grayscalePostProcessor;
        private UICanvas canvas;
        private Cooldown grayscaleDelay;

        public HubScene() : base(new GameSettings
        {
            GameMode = GameMode.Hub,
            Map = "winter_hub",
            DamageMultiplier = 0.0f,
            KnockbackMultiplier = 0.0f
        })
        {
            grayscaleDelay = new Cooldown(2, true);
        }

        public override void initialize()
        {
            base.initialize();
            Console.WriteLine("Initializing HubScene");
            var transition = new FadeTransition
            {
                fadeInDuration = 2,
                onTransitionCompleted = ShowTitleOverlay
            };

            Core.startSceneTransition(transition);

            grayscalePostProcessor = addPostProcessor(new VignettePostProcessor(5));
            grayscalePostProcessor.effect = AssetLoader.GetEffect("grayscale_shader");
            grayscalePostProcessor.effect.Parameters["intensity"].SetValue((float)1);
        }

        public override void Setup()
        {
            base.Setup();
            Console.WriteLine("Setting up Hubscene");
        }

        public override void update()
        {
            base.update();
            grayscaleDelay.Update();
            if (!grayscaleDelay.IsReady())
            {
                Console.WriteLine("Reducing grayscale...");
                var delta = grayscaleDelay.ElapsedNormalized();
                grayscalePostProcessor.effect.Parameters["intensity"].SetValue(delta);
            }
        }

        private void ShowTitleOverlay()
        {
            // create our canvas and put it on the screen space render layer
            canvas = createEntity("ui").addComponent(new UICanvas());
            canvas.isFullScreen = true;
            canvas.renderLayer = Constants.Layers.HUD;

            var table = canvas.stage.addElement(new Table());
            table.setFillParent(true).center();

            var label = new Label("Ultimate Brodown");
            label.setFontScale(10);
            table.add(label);

            Core.schedule(3, (s) =>
            {
                canvas.removeComponent();
                grayscaleDelay.Start();
            });
        }
    }
}
