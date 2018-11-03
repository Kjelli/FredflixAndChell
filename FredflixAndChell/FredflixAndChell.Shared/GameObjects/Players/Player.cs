using System;
using Microsoft.Xna.Framework;
using FredflixAndChell.Shared.GameObjects.Weapons;
using FredflixAndChell.Shared.Components.PlayerComponents;
using Nez;
using static FredflixAndChell.Shared.Assets.Constants;
using FredflixAndChell.Shared.GameObjects.Players.Sprites;
using FredflixAndChell.Shared.GameObjects.Collectibles;
using Nez.Tweens;
using static FredflixAndChell.Shared.GameObjects.Collectibles.Collectibles;
using FredflixAndChell.Shared.Utilities.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace FredflixAndChell.Shared.GameObjects.Players
{
    public class Player : GameObject, ITriggerListener
    {
        private const float ThrowSpeed = 0.5f;

        private Mover _mover;
        private PlayerRenderer _renderer;
        private PlayerController _controller;
        private Collider _proximityHitbox;
        private Collider _playerHitbox;

        private Entity _gunEntity;
        private Gun _gun;

        private int _controllerIndex;
        private float _speed = 50f;

        private List<Entity> _entitiesInProximity;

        public float FacingAngle { get; set; }

        public Vector2 Acceleration;

        public int VerticalFacing { get; set; }
        public int HorizontalFacing { get; set; }
        public bool IsArmed { get; set; } = true;
        public bool FlipGun { get; set; }

        public Player(int x, int y, int controllerIndex = 0) : base(x, y)
        {
            _controllerIndex = controllerIndex;
            _entitiesInProximity = new List<Entity>();
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
            EquipGun(Guns.Get("Fido"));

            // Assign collider component
            _playerHitbox = entity.addComponent(new CircleCollider(4f));
            _playerHitbox.localOffset = new Vector2(0, 4);
            Flags.setFlagExclusive(ref _playerHitbox.collidesWithLayers, Layers.MapObstacles);
            Flags.setFlag(ref _playerHitbox.collidesWithLayers, Layers.Player);
            Flags.setFlagExclusive(ref _playerHitbox.physicsLayer, Layers.Player);

            // Assign proximity interaction hitbox
            _proximityHitbox = entity.addComponent(new CircleCollider(20f));
            Flags.setFlagExclusive(ref _proximityHitbox.collidesWithLayers, Layers.Items);
            Flags.setFlagExclusive(ref _proximityHitbox.physicsLayer, 0);
            _proximityHitbox.isTrigger = true;

            // Assign renderer component
            _renderer = entity.addComponent(new PlayerRenderer(PlayerSpritePresets.Tormod, _gun));
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

        public void EquipGun(GunParameters gun)
        {
            if (_gun != null)
            {
                DropGun();
            }
            _gun = _gunEntity.addComponent(new Gun(this, gun));
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
            EquipGun(nextGun);
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

        private void FallIntoPit(Entity pitEntity)
        {
            _controller.SetInputEnabled(false);
            DropGun();
            Velocity = Vector2.Zero;
            Acceleration = Vector2.Zero;
            _mover.setEnabled(false);
            _playerHitbox.setEnabled(false);
            _proximityHitbox.setEnabled(false);

            var easeType = EaseType.CubicOut;
            var durationSeconds = 2f;
            var targetScale = 0.2f;
            var targetRotationDegrees = 180;
            var targetColor = new Color(0, 0, 0, 0.25f);
            var destination = pitEntity.localPosition;

            entity.tweenRotationDegreesTo(targetRotationDegrees, durationSeconds)
                .setEaseType(easeType)
                .start();
            entity.tweenScaleTo(targetScale, durationSeconds)
                .setEaseType(easeType)
                .start();
            entity.tweenPositionTo(destination, durationSeconds)
                .setEaseType(easeType)
                .setCompletionHandler(_ => entity.setEnabled(false))
                .start();
            _renderer.TweenColor(targetColor, durationSeconds, easeType);
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
            if (_entitiesInProximity.Count == 0) return;

            var closestEntity = _entitiesInProximity.Aggregate((curMin, x) =>
            ((x.position - entity.position).Length() < (curMin.position - entity.position).Length() ? x : curMin));

            var collectible = closestEntity.getComponent<Collectible>();
            if (collectible == null) return;

            if (collectible.Preset.Type == CollectibleType.Weapon)
            {
                EquipGun(collectible.Preset.Gun);
                collectible.OnPickup();
                _entitiesInProximity.Remove(closestEntity);
            }
        }

        static int itemId = 0;
        public void DropGun()
        {
            if (_gun != null)
            {
                //Throw out a new gunz
                var gunItem = entity.scene.createEntity($"item_{++itemId}");
                var throwedItem = gunItem.addComponent(new Collectible(transform.position.X, transform.position.Y, _gun.Parameters.Name, true));

                throwedItem.Velocity = new Vector2(
                    Mathf.cos(FacingAngle) * ThrowSpeed, 
                    Mathf.sin(FacingAngle) * ThrowSpeed)
                    + Velocity * 2f;
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
            if (local == _playerHitbox)
            {
                HitboxTriggerEnter(other, local);
            }
            else if (local == _proximityHitbox)
            {
                ProximityTriggerEnter(other, local);
            }
        }

        private void ProximityTriggerEnter(Collider other, Collider local)
        {
            Console.WriteLine($"Adding {other.entity} to proximity");
            _entitiesInProximity.Add(other.entity);

            if (Flags.isFlagSet(other.entity.tag, Tags.Collectible))
            {
                var collectible = other.entity.getComponent<Collectible>();
                collectible?.Highlight();
            }
        }

        private void HitboxTriggerEnter(Collider other, Collider local)
        {
            if (other.entity.tag == Tags.Pit)
            {
                FallIntoPit(other.entity);
            }
        }

        public void onTriggerExit(Collider other, Collider local)
        {
            if (local == _playerHitbox)
            {
                HitboxTriggerExit(other, local);
            }
            else if (local == _proximityHitbox)
            {
                ProximityTriggerExit(other, local);
            }
        }

        private void HitboxTriggerExit(Collider other, Collider local)
        {
        }

        private void ProximityTriggerExit(Collider other, Collider local)
        {
            Console.WriteLine($"Removing {other.entity} from proximity");
            _entitiesInProximity.Remove(other.entity);
            if (other.entity != null && Flags.isFlagSet(other.entity.tag, Tags.Collectible))
            {
                var collectible = other.entity.getComponent<Collectible>();
                collectible?.Unhighlight();
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
