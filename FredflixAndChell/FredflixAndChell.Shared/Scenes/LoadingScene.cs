using FredflixAndChell.Shared.Assets;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.Scenes
{
    class LoadingScene : Scene
    {
        public LoadingScene()
        {
            var renderer = addRenderer(new RenderLayerRenderer(0, Layers.HUD));
        }

        public override void initialize()
        {
            base.initialize();
            AssetLoader.LoadLoadingScene(content);

            var loadingSpriteEntity = createEntity("loading_sprite");
            var sprite = loadingSpriteEntity.addComponent(new Sprite(AssetLoader.GetTexture("effects/rainbow")));
            loadingSpriteEntity.localScale = new Vector2(ScreenWidth, ScreenHeight) / sprite.bounds.size;
            sprite.renderLayer = Layers.HUD;
            loadingSpriteEntity.position = sprite.bounds.size / 2;
        }

        public override void update()
        {
            base.update();
        }
    }
}
