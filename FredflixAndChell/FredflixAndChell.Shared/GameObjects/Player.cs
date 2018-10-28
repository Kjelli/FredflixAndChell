using System;
using Microsoft.Xna.Framework;
using FredflixAndChell.Shared.GameObjects.Weapons;
using FredflixAndChell.Shared.Components.PlayerComponents;
using Nez;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.GameObjects
{
    public class Player : GameObject
    {
        private Mover _mover;
        private PlayerRenderer _renderer;
        private PlayerController _controller;

        private Entity _gunEntity;
        private Gun _gun;

        private int _controllerIndex;
        private float _speed = 50f;
        public float FacingAngle { get; set; }

        public Vector2 Acceleration;
        public int VerticalFacing { get; set; }
        public int HorizontalFacing { get; set; }
        public bool IsArmed { get; set; } = true;

        public Player(int x, int y, int controllerIndex = -1) : base(x, y)
        {
            _controllerIndex = controllerIndex;
        }

        public override void OnSpawn()
        {
            entity.setTag(Tags.Player);

            // Assign controller component
            _controller = entity.addComponent(new PlayerController(_controllerIndex));

            // Assign movement component
            _mover = entity.addComponent(new Mover());

            // Assign gun component
            _gunEntity = entity.scene.createEntity("gun");
            _gun = _gunEntity.addComponent(new Gun(this, GunPresets.Fido));

            // Assign collider component
            var collider = entity.addComponent(new CircleCollider(4f));
            collider.localOffset = new Vector2(0, 2);
            Flags.setFlagExclusive(ref collider.collidesWithLayers, 0);
            Flags.setFlagExclusive(ref collider.physicsLayer, Layers.MapObstacles);

            // Assign renderer component
            _renderer = entity.addComponent(new PlayerRenderer(PlayerSprite.Tormod, _gun));
        }

        public override void update()
        {
            SetFacing();
            Move();

            if (_controller.FirePressed)
                Attack();
            if (_controller.ReloadPressed)
                Reload();

        }

        private void Move()
        {
            var deltaTime = Time.deltaTime;

            Acceleration = new Vector2(_controller.XLeftAxis, _controller.YLeftAxis);
            Acceleration *= _speed * deltaTime;

            Velocity = (0.90f * Velocity + 0.1f * Acceleration);

            if (Velocity.Length() < 0.001f) Velocity = Vector2.Zero;

            var isColliding = _mover.move(Velocity, out CollisionResult result);
        }

        public void Attack()
        {
            _gun.Fire();
        }

        public void Reload()
        {
            _gun.ReloadMagazine();
        }
        public override void OnDespawn()
        {
        }

        private void SetFacing()
        {
            if (_controller.YRightAxis == 0 && _controller.XRightAxis == 0) return;

            FacingAngle = (float)Math.Atan2(_controller.YRightAxis, _controller.XRightAxis);

            _gun.entity.rotation = FacingAngle;

            if (FacingAngle > 0 && FacingAngle < Math.PI)
            {
                VerticalFacing = (int)FacingCode.DOWN;
            }
            else
            {
                VerticalFacing = (int)FacingCode.UP;
            }
            //Prioritizing "horizontal" sprites
            if (FacingAngle > -Math.PI / 2 && FacingAngle < Math.PI / 2)
            {
                _renderer.FlipX(false);
                HorizontalFacing = (int)FacingCode.RIGHT;
            }
            else
            {
                _renderer.FlipX(true);
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
