using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FredflixAndChell.Shared.Assets;
using Microsoft.Xna.Framework.Input;
using FredflixAndChell.Shared.Utilities;

using FredflixAndChell.Shared.Scenes;
using FredflixAndChell.Shared.GameObjects.Bullets;
using FredflixAndChell.Shared.GameObjects.Weapons;
using Nez;
using Nez.Sprites;
using Nez.Tiled;
using FredflixAndChell.Shared.Components;
using Nez.Textures;
using static FredflixAndChell.Shared.Assets.Constants;
using Nez.Shadows;

namespace FredflixAndChell.Shared.GameObjects
{
    public class Player : GameObject
    {
        private Vector2 Acceleration;
        private Mover _mover;
        private PlayerController _controller;

        private Entity _gunEntity;
        private Gun _gun;

        private Entity _lightEntity;

        private float _speed = 200f;
        public float FacingAngle { get; set; }

        public int VerticalFacing { get; set; }
        public int HorizontalFacing { get; set; }

        enum Animations
        {
            Walk_Unarmed,
            Idle_Unarmed,
            Walk,
            Idle

        }

        Sprite<Animations> _animation;



        public Player(int x, int y) : base(x, y, 64, 64)
        {
            SetupAnimations();
        }

        private Sprite<Animations> SetupAnimations()
        {
            var animations = new Sprite<Animations>();
            var texture = AssetLoader.GetTexture("kjelli_spritesheet");
            var subtextures = Subtexture.subtexturesFromAtlas(texture, 32, 32);
            animations.addAnimation(Animations.Idle_Unarmed, new SpriteAnimation(new List<Subtexture>()
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

            animations.addAnimation(Animations.Idle, new SpriteAnimation(new List<Subtexture>()
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

            animations.addAnimation(Animations.Walk_Unarmed, new SpriteAnimation(new List<Subtexture>()
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

            animations.addAnimation(Animations.Walk, new SpriteAnimation(new List<Subtexture>()
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

        public override void OnDespawn()
        {
        }

        public override void OnSpawn()
        {
            // Assign controller component
            _controller = entity.addComponent(new PlayerController(0));

            // Assign movement component
            _mover = entity.addComponent(new Mover());

            // Assign gun component
            _gunEntity = entity.scene.createEntity("gun");
            _gun = _gunEntity.addComponent(new Gun(this, (int)entity.position.X, (int)entity.position.Y, 0.1f));

            // Assign collider component
            var collider = entity.addComponent(new CircleCollider(4f));
            collider.localOffset = new Vector2(0, 4);
            Flags.setFlagExclusive(ref collider.collidesWithLayers, 0);
            Flags.setFlagExclusive(ref collider.physicsLayer, Layers.MapObstacles);

            // Assign renderable (animation) component
            var animations = SetupAnimations();
            _animation = entity.addComponent(animations);
            _animation.renderLayer = Layers.Player;

            // Assign faint glow to player
            var sprite = entity.addComponent(new Sprite(AssetLoader.GetTexture("lightmask_sm")));
            sprite.material = Material.blendLighten();
            sprite.color = Color.Gray;
            sprite.renderLayer = Layers.Lights;

            // Assign renderable shadow component
            var shadow = entity.addComponent(new SpriteMime(_animation));
            shadow.color = new Color(0, 0, 0, 80);
            shadow.material = Material.stencilRead(Stencils.EntityShadowStencil);
            shadow.renderLayer = Layers.Shadow;
            shadow.localOffset = new Vector2(1, 2);

            // Assign silhouette component when player is visually blocked
            var silhouette = entity.addComponent(new SpriteMime(_animation));
            silhouette.color = new Color(0, 0, 0, 80);
            silhouette.material = Material.stencilRead(Stencils.HiddenEntityStencil);
            silhouette.renderLayer = Layers.Foreground;
            silhouette.localOffset = new Vector2(0, 0);
        }

        public override void update()
        {
            SetFacing();
            Move();
            UpdateAnimation();

            if (_controller.FirePressed)
                Attack();
            if (_controller.ReloadPressed)
                Reload();

        }

        private void Move()
        {
            var deltaTime = Time.deltaTime;

            Acceleration = new Vector2(_controller.XLeftAxis, _controller.YLeftAxis);
            Acceleration *= _speed;

            Velocity = new Vector2(Velocity.X * 0.75f + Acceleration.X * 0.25f, Velocity.Y * 0.75f + Acceleration.Y * 0.25f);
            Velocity *= deltaTime;

            var isColliding = _mover.move(Velocity, out CollisionResult result);

            if (isColliding)
            {
                Console.WriteLine(result.minimumTranslationVector);
            }
        }

        private void UpdateAnimation()
        {

            //Todo: Fix check of unmarmed. A gun type called unarmed?
            bool armed = _gun != null ? true : false;

            var animation = armed ? Animations.Idle : Animations.Idle_Unarmed;

            if (_controller.XLeftAxis < 0 || _controller.XLeftAxis > 0 || _controller.YLeftAxis != 0)
            {
                animation = armed ? Animations.Walk : Animations.Walk_Unarmed;
            }

            if (!_animation.isAnimationPlaying(animation))
            {
                _animation.play(animation);
            }
        }

        public void Attack()
        {
            _gun.Fire();
        }

        public void Reload()
        {
            _gun.ReloadMagazine();
        }

        private void SetFacing()
        {
            FacingAngle = (float)Math.Atan2(_controller.YRightAxis, _controller.XRightAxis);
            _gun.entity.rotation = FacingAngle;

            if (FacingAngle < -Math.PI / 2 || FacingAngle > Math.PI / 2)
            {
                VerticalFacing = (int)FacingCode.UP;
            }
            else
            {
                VerticalFacing = (int)FacingCode.DOWN;
            }
            //Prioritizing "horizontal" sprites
            if (FacingAngle > -Math.PI / 2 && FacingAngle < Math.PI / 2)
            {
                _animation.flipX = false;
                _gun.flipY = false;
                HorizontalFacing = (int)FacingCode.RIGHT;
            }
            else
            {
                _gun.flipY = true;
                _animation.flipX = true;
                HorizontalFacing = (int)FacingCode.LEFT;
            }
        }
    }

    public enum FacingCode
    {
        UP = 1,
        RIGHT = 2,
        DOWN = 3,
        LEFT = 4

    }
}
