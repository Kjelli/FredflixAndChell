using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.GameObjects;
using FredflixAndChell.Shared.GameObjects.Weapons;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using System.Collections.Generic;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.Components.PlayerComponents
{
    public class PlayerSprite
    {
        public static readonly PlayerSprite Tormod = "tormod";
        public static readonly PlayerSprite Kjelli = "kjelli";

        private readonly string _spriteName;
        private PlayerSprite(string spriteName)
        {
            _spriteName = spriteName;
        }

        public static implicit operator PlayerSprite(string input)
        {
            return new PlayerSprite(input);
        }

        public override string ToString()
        {
            return _spriteName;
        }
    }

    public class PlayerRenderer : Component, IUpdatable
    {
        private Player _player;
        private Gun _gun;
        private PlayerSprite _playerSprite;

        Sprite<BodyAnimations> _body;
        Sprite<HeadAnimations> _head;


        enum HeadAnimations
        {
            FrontFacing,
            BackFacing
        }

        enum BodyAnimations
        {
            Walk_Unarmed,
            Idle_Unarmed,
            Walk,
            Idle
        }

        public PlayerRenderer(PlayerSprite playerSprite, Gun gun)
        {
            _playerSprite = playerSprite;
            _gun = gun;
        }

        public override void onAddedToEntity()
        {
            _player = entity.getComponent<Player>();

            SetupPlayerSprites();

            // Assign faint glow to player
            var sprite = entity.addComponent(new Sprite(AssetLoader.GetTexture("effects/lightmask_xs")));
            sprite.material = Material.blendScreen();
            sprite.color = Color.White;
            sprite.renderLayer = Layers.Lights;

            // Assign renderable shadow component
            var bodyShadow = entity.addComponent(new SpriteMime(_body));
            bodyShadow.color = new Color(0, 0, 0, 80);
            bodyShadow.material = Material.stencilRead(Stencils.EntityShadowStencil);
            bodyShadow.renderLayer = Layers.Shadow;
            bodyShadow.localOffset = new Vector2(1, 2);

            var headShadow = entity.addComponent(new SpriteMime(_head));
            headShadow.color = new Color(0, 0, 0, 80);
            headShadow.material = Material.stencilRead(Stencils.EntityShadowStencil);
            headShadow.renderLayer = Layers.Shadow;
            headShadow.localOffset = new Vector2(1, 2);

            // Assign silhouette component when player is visually blocked
            var bodySilhouette = entity.addComponent(new SpriteMime(_body));
            bodySilhouette.color = new Color(0, 0, 0, 80);
            bodySilhouette.material = Material.stencilRead(Stencils.HiddenEntityStencil);
            bodySilhouette.renderLayer = Layers.Foreground;
            bodySilhouette.localOffset = new Vector2(0, 0);

            var headSilhouette = entity.addComponent(new SpriteMime(_head));
            headSilhouette.color = new Color(0, 0, 0, 80);
            headSilhouette.material = Material.stencilRead(Stencils.HiddenEntityStencil);
            headSilhouette.renderLayer = Layers.Foreground;
            headSilhouette.localOffset = new Vector2(0, 0);
        }

        private void SetupPlayerSprites()
        {
            // Assign renderable (animation) component
            var bodyAnimations = SetupBodyAnimations();
            _body = entity.addComponent(bodyAnimations);
            _body.renderLayer = Layers.Player;

            var headAnimations = SetupHeadAnimations();
            _head = entity.addComponent(headAnimations);
            _head.renderLayer = Layers.PlayerFront;
        }
        private Sprite<BodyAnimations> SetupBodyAnimations()
        {
            var animations = new Sprite<BodyAnimations>();
            var body = AssetLoader.GetTexture($"players/{_playerSprite}_body");
            var subtextures = Subtexture.subtexturesFromAtlas(body, 32, 32);
            animations.addAnimation(BodyAnimations.Idle_Unarmed, new SpriteAnimation(new List<Subtexture>()
            {
                subtextures[0 + 0 * 8],
                subtextures[0 + 1 * 8],
                subtextures[0 + 2 * 8],
                subtextures[0 + 3 * 8],
            })
            {
                loop = true,
                fps = 5
            });

            animations.addAnimation(BodyAnimations.Idle, new SpriteAnimation(new List<Subtexture>()
            {
                subtextures[2 + 0 * 8],
                subtextures[2 + 1 * 8],
                subtextures[2 + 2 * 8],
                subtextures[2 + 3 * 8],
            })
            {
                loop = true,
                fps = 5
            });

            animations.addAnimation(BodyAnimations.Walk_Unarmed, new SpriteAnimation(new List<Subtexture>()
            {
                subtextures[1 + 7 * 8],
                subtextures[1 + 0 * 8],
                subtextures[1 + 1 * 8],
                subtextures[1 + 2 * 8],
                subtextures[1 + 3 * 8],
                subtextures[1 + 4 * 8],
                subtextures[1 + 5 * 8],
                subtextures[1 + 6 * 8],
            })
            {
                loop = true,
                fps = 10
            });

            animations.addAnimation(BodyAnimations.Walk, new SpriteAnimation(new List<Subtexture>()
            {
                subtextures[3 + 7 * 8],
                subtextures[3 + 0 * 8],
                subtextures[3 + 1 * 8],
                subtextures[3 + 2 * 8],
                subtextures[3 + 3 * 8],
                subtextures[3 + 4 * 8],
                subtextures[3 + 5 * 8],
                subtextures[3 + 6 * 8],
            })
            {
                loop = true,
                fps = 10
            });

            return animations;
        }

        private Sprite<HeadAnimations> SetupHeadAnimations()
        {
            var animations = new Sprite<HeadAnimations>();
            var head = AssetLoader.GetTexture($"players/{_playerSprite}_head");
            var subtextures = Subtexture.subtexturesFromAtlas(head, 32, 32);
            animations.addAnimation(HeadAnimations.FrontFacing, new SpriteAnimation(new List<Subtexture>()
            {
                subtextures[0],
            })
            {
                loop = true,
                fps = 1
            });

            animations.addAnimation(HeadAnimations.BackFacing, new SpriteAnimation(new List<Subtexture>()
            {
                subtextures[1],
            })
            {
                loop = true,
                fps = 1
            });


            return animations;
        }

        public void update()
        {
            UpdateAnimation();
        }

        private void UpdateAnimation()
        {
            //Todo: Fix check of unmarmed. A gun type called unarmed?
            bool armed = _player.IsArmed;

            var animation = armed ? BodyAnimations.Idle : BodyAnimations.Idle_Unarmed;

            if (_player.Acceleration.Length() > 0)
            {
                animation = armed ? BodyAnimations.Walk : BodyAnimations.Walk_Unarmed;
            }

            if (!_body.isAnimationPlaying(animation))
            {
                _body.play(animation);
            }

            if (_player.VerticalFacing == (int)FacingCode.UP && !_head.isAnimationPlaying(HeadAnimations.BackFacing))
            {
                _head.play(HeadAnimations.BackFacing);
                _head.setRenderLayer(Layers.PlayerBehind);
                if (_gun != null) _gun.SetRenderLayer(Layers.PlayerBehindest);
            }

            if (_player.VerticalFacing == (int)FacingCode.DOWN && !_head.isAnimationPlaying(HeadAnimations.FrontFacing))
            {
                _head.play(HeadAnimations.FrontFacing);
                _head.setRenderLayer(Layers.PlayerFront);
                if (_gun != null) _gun.SetRenderLayer(Layers.PlayerFrontest);
            }
        }

        public void FlipX(bool isFlipped)
        {
            if (_body != null) _body.flipX = isFlipped;
            if (_head != null) _head.flipX = isFlipped;
            if (_gun != null) _gun.flipY = isFlipped;
        }
    }
}
