using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.GameObjects;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.GameObjects.Weapons;
using FredflixAndChell.Shared.GameObjects.Weapons.Sprites;
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
using static FredflixAndChell.Shared.GameObjects.Weapons.Sprites.GunSprite;

namespace FredflixAndChell.Shared.Components.Guns
{
    public class GunRenderer : Component, IUpdatable
    {
        private Player _player;
        private Gun _gun;

        private float _renderOffset;
        Sprite<GunAnimations> _animation;

        private bool _flipY;

        public GunRenderer(Gun gun, Player player)
        {
            _gun = gun;
            _player = player;
            _renderOffset = _gun.Parameters.RenderOffset;

           
        }

        private Sprite<GunAnimations> SetupAnimations(GunSprite sprite)
        {
            var animations = new Sprite<GunAnimations>();
            
            animations.addAnimation(GunAnimations.Held_Fired, sprite.Fire.ToSpriteAnimation(sprite.Source));
            animations.addAnimation(GunAnimations.Reload, sprite.Reload.ToSpriteAnimation(sprite.Source));
            animations.addAnimation(GunAnimations.Held_Idle, sprite.Idle.ToSpriteAnimation(sprite.Source));


            return animations;
        }

        public override void onAddedToEntity()
        {
            base.onAddedToEntity();
            entity.setScale(0.6f);

            _animation = entity.addComponent(SetupAnimations(_gun.Parameters.Sprite));
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

            _animation.play(GunAnimations.Held_Idle);
        }


        public void Fire()
        {
            _animation?.play(GunAnimations.Held_Fired);
        }

        public void update()
        {
            _animation.flipY = _flipY;
            entity.position = new Vector2(_player.entity.position.X + (float)Math.Cos(_player.FacingAngle) * _renderOffset,
                _player.entity.position.Y + (float)Math.Sin(_player.FacingAngle) * _renderOffset / 2);
            if (!_animation.isPlaying)
            {
                _animation.play(GunAnimations.Held_Idle);
            }
        }

        public void Reload()
        {
            _animation?.play(GunAnimations.Reload);
        }

        public void setRenderLayer(int renderLayer)
        {
            _animation?.setRenderLayer(renderLayer);
        }


        public void FlipY(bool flipY)
        {
            _flipY = flipY;
        }

        public Sprite<GunAnimations> GetAnimations()
        {
            return _animation;
        }
    }
}
