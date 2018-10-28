using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.GameObjects;
using FredflixAndChell.Shared.GameObjects.Weapons;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.Components.Guns
{
    public class GunRenderer : Component, IUpdatable
    {
        enum Animations
        {
            Held_Idle,
            Held_Fired,
            Reload
        }

        private Player _player;
        private Gun _gun;
        private GunParameters _gunParameters;

        private float _renderOffset;
        Sprite<Animations> _animation;

        private bool _flipY;

        public GunRenderer(Gun gun, Player player)
        {
            _gun = gun;
            _player = player;
            _renderOffset = _gun.Parameters.RenderOffset;
        }

        private Sprite<Animations> SetupAnimations()
        {
            var animations = new Sprite<Animations>();
            var texture = AssetLoader.GetTexture($"guns/{_gun.Parameters.Sprite}");
            var subtextures = Subtexture.subtexturesFromAtlas(texture, 32, 32);
            subtextures.ForEach(t => t.origin = new Vector2(10, 16));

            animations.addAnimation(Animations.Held_Idle, new SpriteAnimation(new List<Subtexture>()
            {
                subtextures[2 + 0 * 8],
                subtextures[3 + 0 * 8], //
                subtextures[3 + 1 * 8], // Lagt til temp for Fido gunner
                subtextures[3 + 0 * 8], //
                subtextures[2 + 0 * 8], //
                subtextures[3 + 0 * 8], //
                subtextures[2 + 0 * 8], //
                subtextures[3 + 0 * 8], // 
                subtextures[3 + 1 * 8], //
                subtextures[3 + 0 * 8], //
            })
            {
                loop = true,
                fps = 4
            });


            animations.addAnimation(Animations.Held_Fired, new SpriteAnimation(new List<Subtexture>()
            {
                subtextures[1 + 1 * 8],
                subtextures[1 + 2 * 8],
                subtextures[1 + 3 * 8],
                subtextures[1 + 0 * 8],
            })
            {
                loop = false,
                fps = 15
            });

            animations.addAnimation(Animations.Reload, new SpriteAnimation(new List<Subtexture>()
            {
                subtextures[1 + 0 * 8],
                subtextures[2 + 0 * 8],
                subtextures[2 + 1 * 8],
                subtextures[2 + 2 * 8],
                subtextures[2 + 3 * 8],
                //subtextures[2 + 4 * 8], fjernet temp for fido gunner
                subtextures[2 + 3 * 8],
                subtextures[2 + 2 * 8],
                subtextures[2 + 1 * 8],
                subtextures[2 + 0 * 8],
                subtextures[1 + 0 * 8],
            })
            {
                loop = false,
                fps = 15
            });


            return animations;
        }

        public override void onAddedToEntity()
        {
            base.onAddedToEntity();
            entity.setScale(0.6f);

            _animation = entity.addComponent(SetupAnimations());
            _animation.renderLayer = Layers.PlayerFrontest;

            var shadow = entity.addComponent(new SpriteMime(_animation));
            shadow.color = new Color(0, 0, 0, 80);
            shadow.material = Material.stencilRead(Stencils.EntityShadowStencil);
            shadow.renderLayer = Layers.Shadow;
            shadow.localOffset = new Vector2(1, 2);

            // Assign silhouette component when gun is visually blocked
            var silhouette = entity.addComponent(new SpriteMime(_animation));
            silhouette.color = new Color(0, 0, 0, 80);
            silhouette.material = Material.stencilRead(Stencils.HiddenEntityStencil);
            silhouette.renderLayer = Layers.Foreground;
            silhouette.localOffset = new Vector2(0, 0);

            _animation.play(Animations.Held_Idle);
        }


        public void Fire()
        {
            _animation?.play(Animations.Held_Fired);
        }

        public void update()
        {
            _animation.flipY = _flipY;
            entity.position = new Vector2(_player.entity.position.X + (float)Math.Cos(_player.FacingAngle) * _renderOffset,
                _player.entity.position.Y + (float)Math.Sin(_player.FacingAngle) * _renderOffset / 2);
            if (!_animation.isPlaying)
            {
                _animation.play(Animations.Held_Idle);
            }
        }

        public void Reload()
        {
            _animation?.play(Animations.Reload);
        }

        public void setRenderLayer(int renderLayer)
        {
            _animation?.setRenderLayer(renderLayer);
        }


        public void FlipY(bool flipY)
        {
            _flipY = flipY;
        }
    }
}
