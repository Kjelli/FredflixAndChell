using FredflixAndChell.Shared.Components.PlayerComponents;
using FredflixAndChell.Shared.GameObjects.Collectibles;
using FredflixAndChell.Shared.GameObjects.Players.Sprites;
using FredflixAndChell.Shared.GameObjects.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Tweens;
using System;
using static FredflixAndChell.Shared.Assets.Constants;
using static FredflixAndChell.Shared.GameObjects.Collectibles.CollectiblePresets;

namespace FredflixAndChell.Shared.GameObjects.Players
{
    public class Player : GameObject, ITriggerListener
    {
        private const float ThrowSpeed = 1.0f;
        private readonly int _controllerIndex;

        private int _health;
        private Mover _mover;
        private PlayerRenderer _renderer;
        private PlayerController _controller;
        private Collider _collider;
        private float _accelerationMultiplier = 0.05f;
        private float _speed = 50f;

        private Entity _gunEntity;
        private Gun _gun;

        private Vector2 _previousValidRightStickInput;
        private Collider _touchingObject;

        public Vector2 Acceleration { get; set; }
        public int VerticalFacing { get; set; }
        public int HorizontalFacing { get; set; }
        public bool IsArmed { get; set; } = true;
        public bool FlipGun { get; set; }
        public float FacingAngle { get; set; }

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
            _previousValidRightStickInput = new Vector2(0, 0);

            // Assign gun component
            _gunEntity = entity.scene.createEntity("gun");
            EquipGun("M4");

            // Assign collider component
            _collider = entity.addComponent(new CircleCollider(4f));
            _collider.localOffset = new Vector2(0, 4);
            Flags.setFlagExclusive(ref _collider.collidesWithLayers, Layers.MapObstacles);
            Flags.setFlag(ref _collider.collidesWithLayers, Layers.Player);
            Flags.setFlagExclusive(ref _collider.physicsLayer, Layers.Player);

            // Assign renderer component
            _renderer = entity.addComponent(new PlayerRenderer(PlayerSpritePresets.Kjelli, _gun));

            //TODO: Character based 
            _health = 100;
        }

        public override void update()
        {
            ReadInputs();
            Move();
            SetFacing();

            var state = GamePad.GetState(0);
            //Console.WriteLine("X: " + state.ThumbSticks.Right.X + " : " + state.ThumbSticks.Right.Y);
        }

        private void ReadInputs()
        {
            if (!_controller.InputEnabled) return;

            if (_controller.FirePressed)
                Attack();
            if (_controller.ReloadPressed)
                Reload();
            if (_controller.DropGunPressed)
                DropGun();
            if (_controller.SwitchWeaponPressed)
                SwitchWeapon();
            if (_controller.InteractPressed)
                Interact();
            if (_controller.DebugModePressed)
                Core.debugRenderEnabled = !Core.debugRenderEnabled;

            ToggleSprint();

            Acceleration = new Vector2(_controller.XLeftAxis, _controller.YLeftAxis);

            _previousValidRightStickInput = new Vector2(
                _controller.XRightAxis == 0 ? _previousValidRightStickInput.X : _controller.XRightAxis,
                _controller.YRightAxis == 0 ? _previousValidRightStickInput.Y : _controller.YRightAxis
            );

            FacingAngle = (float)Math.Atan2(_previousValidRightStickInput.Y, _previousValidRightStickInput.X);
        }

        private void ToggleSprint()
        {
            if (_controller.SprintPressed)
                _accelerationMultiplier = 0.10f;
            else
                _accelerationMultiplier = 0.05f;
        }

        public void EquipGun(string name)
        {
            if (_gun != null)
            {
                UnEquipGun();
            }
            _gun = _gunEntity.addComponent(new Gun(this, Guns.Get(name)));
            IsArmed = true;
        }

        public void UnEquipGun()
        {
            IsArmed = false;
            _gunEntity.removeAllComponents();
            _gun = null;
        }

        private void SwitchWeapon()
        {
            var nextGun = Guns.GetNextAfter(_gun.Parameters.Name);
            EquipGun(nextGun.Name);
        }

        private void Move()
        {
            var deltaTime = Time.deltaTime;

            Acceleration *= _speed * deltaTime;
            Velocity = (0.95f * Velocity + _accelerationMultiplier * Acceleration);

            if (Velocity.Length() < 0.001f) Velocity = Vector2.Zero;

            var isColliding = _mover.move(Velocity, out CollisionResult collision);

            if (isColliding)
            {
                // Handle collisions here
            }
        }

        private void FallIntoPit(Entity pitEntity)
        {
            DisablePlayer();
            var easeType = EaseType.CubicOut;
            var duration = 2f;
            var targetScale = 0.2f;
            var targetRotationDegrees = 180;
            var targetColor = new Color(0, 0, 0, 0.25f);
            var destination = pitEntity.localPosition;

            entity.tweenRotationDegreesTo(targetRotationDegrees, duration)
                .setEaseType(easeType)
                .start();
            entity.tweenScaleTo(targetScale, duration)
                .setEaseType(easeType)
                .start();
            entity.tweenPositionTo(destination, duration)
                .setEaseType(easeType)
                .setCompletionHandler(_ => entity.setEnabled(false))
                .start();
            _renderer.TweenColor(targetColor, duration, easeType);
        }

        public void DisablePlayer()
        {
            _controller.SetInputEnabled(false);
            DropGun();
            Velocity = Vector2.Zero;
            Acceleration = Vector2.Zero;
            _mover.setEnabled(false);
            _collider.setEnabled(false);
        }

        public void Attack()
        {
            if (_gun != null)
                _gun.Fire();
        }

        public void Reload()
        {

            if (_gun != null)
                _gun.ReloadMagazine();
        }

        public void Interact()
        {
            if (_touchingObject != null)
            {

                var collectible = _touchingObject.entity.getComponent<Collectible>();
                if (collectible.Preset.Type == CollectibleType.Weapon)
                {
                    EquipGun(collectible.Preset.Gun.Name);
                }
                collectible.entity.destroy();
            }
        }

        public void Damage(int damage)
        {
            Console.WriteLine("Health player " + _controllerIndex + ": " + _health);
            _health -= damage;
            if (_health < 0)
            {
                DisablePlayer();
            }
        }

        public void DropGun()
        {
            if (_gun != null)
            {
                //Throw out a new gunz
                var gunItem = entity.scene.createEntity("item");
                var throwedItem = gunItem.addComponent(new Collectible(transform.position.X, transform.position.Y, _gun.Parameters.Name, true));

                throwedItem.Velocity = new Vector2(Mathf.cos(FacingAngle) * ThrowSpeed, Mathf.sin(FacingAngle) * ThrowSpeed) + Velocity * 2f;
                UnEquipGun();
            }
        }

        public override void OnDespawn()
        {
            _gun.Destroy();
        }

        private void SetFacing()
        {
            if (_controller.YRightAxis == 0 && _controller.XRightAxis == 0) return;

            if (_gun != null && _gunEntity != null)
            {
                _gunEntity.rotation = FacingAngle;
            }

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
                FlipGun = false;
                HorizontalFacing = (int)FacingCode.RIGHT;
            }
            else
            {
                _renderer.FlipX(true);
                FlipGun = true;
                HorizontalFacing = (int)FacingCode.LEFT;
            }
        }

        public void onTriggerEnter(Collider other, Collider local)
        {
            if (other.entity.tag == Tags.Pit)
            {
                FallIntoPit(other.entity);
            }

            if (other.entity.tag == Tags.Collectible)
            {
                _touchingObject = other;
            }
        }

        public void onTriggerExit(Collider other, Collider local)
        {
            //Console.WriteLine($"TriggerExit: {other}, {other.entity.tag}");
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
