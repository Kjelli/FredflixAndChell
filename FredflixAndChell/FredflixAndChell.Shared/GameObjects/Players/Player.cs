using System;
using Microsoft.Xna.Framework;
using FredflixAndChell.Shared.GameObjects.Weapons;
using FredflixAndChell.Shared.Components.PlayerComponents;
using Nez;
using static FredflixAndChell.Shared.Assets.Constants;
using FredflixAndChell.Shared.GameObjects.Players.Sprites;
using FredflixAndChell.Shared.GameObjects.Collectibles;
using Nez.Tweens;
using static FredflixAndChell.Shared.GameObjects.Collectibles.CollectiblePresets;
using FredflixAndChell.Shared.Utilities.Serialization;

namespace FredflixAndChell.Shared.GameObjects.Players
{
    public class Player : GameObject, ITriggerListener
    {
        private const float ThrowSpeed = 1.0f;

        private Mover _mover;
        private PlayerRenderer _renderer;
        private PlayerController _controller;
        private Collider _collider;

        private Entity _gunEntity;
        private Gun _gun;

        private int _controllerIndex;
        private float _speed = 50f;
        public float FacingAngle { get; set; }

        public Vector2 Acceleration;

        public int VerticalFacing { get; set; }
        public int HorizontalFacing { get; set; }
        public bool IsArmed { get; set; } = true;
        public bool FlipGun { get; set; }

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
            EquipGun("Fido");

            // Assign collider component
            _collider = entity.addComponent(new CircleCollider(4f));
            _collider.localOffset = new Vector2(0, 4);
            Flags.setFlagExclusive(ref _collider.collidesWithLayers, Layers.MapObstacles);
            Flags.setFlag(ref _collider.collidesWithLayers, Layers.Player);
            Flags.setFlagExclusive(ref _collider.physicsLayer, Layers.Player);

            // Assign renderer component
            _renderer = entity.addComponent(new PlayerRenderer(PlayerSpritePresets.Kjelli, _gun));
        }


        public override void update()
        {
            ReadInputs();
            Move();
            SetFacing();
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

            Acceleration = new Vector2(_controller.XLeftAxis, _controller.YLeftAxis);
            FacingAngle = (float)Math.Atan2(_controller.YRightAxis, _controller.XRightAxis);

        }

        private void SwitchWeapon()
        {
            var nextGun = Guns.GetNextAfter(_gun.Parameters.Name);
            _gunEntity.removeAllComponents();
            _gun = _gunEntity.addComponent(new Gun(this, nextGun));
        }

        private void Move()
        {
            var deltaTime = Time.deltaTime;

            Acceleration *= _speed * deltaTime;
            Velocity = (0.95f * Velocity + 0.05f * Acceleration);

            if (Velocity.Length() < 0.001f) Velocity = Vector2.Zero;

            var isColliding = _mover.move(Velocity, out CollisionResult collision);

            if (isColliding)
            {
                // Handle collisions here
            }
        }

        public void EquipGun(string name)
        {
            if (_gun != null)
            {
                UnEquipGun();
            }
            _gunEntity = entity.scene.createEntity("gun");
            _gun = _gunEntity.addComponent(new Gun(this, Guns.Get(name)));
            IsArmed = true;
        }

        public void UnEquipGun()
        {
            IsArmed = false;
            //Destroying current gunz
            _gunEntity.destroy();
            _gun = null;
        }

        private void FallIntoPit(Entity pitEntity)
        {
            _controller.SetInputEnabled(false);
            DropGun();
            Velocity = Vector2.Zero;
            Acceleration = Vector2.Zero;
            _mover.setEnabled(false);
            _collider.setEnabled(false);

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


        public void Attack()
        {
            if(_gun != null)
                _gun.Fire();
        }

        public void Reload()
        {

            if(_gun != null)
            _gun.ReloadMagazine();
        }

        public void Interact()
        {
            Console.WriteLine("Interact");
        }

        public void DropGun()
        {
            if(_gun != null)
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
            _gun.entity.setEnabled(false);
            _gun.Destroy();
        }

        private void SetFacing()
        {
            if (_controller.YRightAxis == 0 && _controller.XRightAxis == 0) return;

            if(_gun != null && _gunEntity != null)
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

            if(other.entity.tag == Tags.Pit)
            {
                FallIntoPit(other.entity);
            }

            if(other.entity.tag == Tags.Collectible)
            {
                //other.entity.destroy();
                var collectible= other.entity.getComponent<Collectible>();
                if(collectible.Preset.Type == CollectibleType.Weapon)
                {
                    EquipGun(collectible.Preset.Gun.Name);
                }
                collectible.entity.destroy();
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
