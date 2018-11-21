using FredflixAndChell.Shared.Components.Cameras;
using FredflixAndChell.Shared.Components.PlayerComponents;
using FredflixAndChell.Shared.GameObjects.Collectibles;
using FredflixAndChell.Shared.GameObjects.Players.Sprites;
using FredflixAndChell.Shared.GameObjects.Weapons;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Tweens;
using System;
using System.Collections.Generic;
using System.Linq;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.GameObjects.Players
{
    public enum PlayerState
    {
        Normal, Dying, Dead
    }

    public class Player : GameObject, ITriggerListener
    {
        private const float ThrowSpeed = 0.5f;

        private readonly int _controllerIndex;

        private PlayerState _playerState;

        private Mover _mover;
        private PlayerRenderer _renderer;
        private CameraTracker _cameraTracker;
        private PlayerController _controller;
        private Collider _proximityHitbox;
        private Collider _playerHitbox;

        private Entity _gunEntity;
        private Gun _gun;

        private int _health;
        private float _speed = 50f;
        private float _accelerationMultiplier;
        private float _stamina = 100;
        private bool _shouldRegenStamina = false;
        private readonly float _walkAcceleration = 0.05f;
        private readonly float _sprintAcceleration = 0.10f;
        static int itemId = 0;

        private List<Entity> _entitiesInProximity;
        private bool _isRolling;
        private bool _isRollingRight;
        private int _numSprintPressed = 0;

        public PlayerState PlayerState => _playerState;
        public int PlayerIndex => _controllerIndex;
        public int Health => (int)_health;
        public int Stamina => (int)_stamina;
        public Vector2 Acceleration { get; set; }
        public int VerticalFacing { get; set; }
        public int HorizontalFacing { get; set; }
        public bool IsArmed { get; set; } = true;
        public bool FlipGun { get; set; }
        public Vector2 FacingAngle { get; set; }

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
            EquipGun("M4");

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
            _renderer = entity.addComponent(new PlayerRenderer(PlayerSpritePresets.Trump, _gun));

            // Assign camera tracker component
            _cameraTracker = entity.addComponent(new CameraTracker(() => _playerState != PlayerState.Dead));

            //TODO: Character based 
            _health = 100;
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
            // TODO: Add some sort of "grace" time between clicks so that only a somewhat fast double-click
            // should trigger a dodge roll.
            if (_controller.SprintPressed)
                _numSprintPressed++;

            ToggleSprint();
            ToggleStaminaRegen();
            ToggleDodgeRoll();

            Acceleration = new Vector2(_controller.XLeftAxis, _controller.YLeftAxis);
        }

        private void ToggleDodgeRoll()
        {
            if (_shouldRegenStamina) return;
            if (_numSprintPressed == 2 && _controller.SprintPressed)
            {
                _isRolling = true;
                _isRollingRight = FacingAngle.X > 0 ? true : false;
            }

            if (_isRolling)
            {
                if (_isRollingRight)
                {
                    entity.localRotation += 4 * (float)Math.PI * Time.deltaTime;
                }
                else
                {
                    entity.localRotation -= 4 * (float)Math.PI * Time.deltaTime;
                }
            }

            if ((_isRollingRight && entity.localRotation >= (2 * Math.PI)) || (!_isRollingRight && entity.localRotation <= (-2 * Math.PI)))
            {
                _isRolling = false;
                entity.localRotation = 0;
                _numSprintPressed = 0;
                _stamina -= 50;
            }
        }

        private void ToggleStaminaRegen()
        {
            if (!_shouldRegenStamina)
                return;

            _stamina += 25 * Time.deltaTime;

            if (_stamina >= 100)
            {
                _stamina = 100;
                _shouldRegenStamina = false;
            }
        }

        private void ToggleSprint()
        {
            if (_controller.SprintDown && !_shouldRegenStamina)
            {
                _accelerationMultiplier = _sprintAcceleration;
                _stamina -= 50 * Time.deltaTime;
                _gun.ToggleRunning(true);
            }
            else
            {
                _accelerationMultiplier = _walkAcceleration;
                _gun.ToggleRunning(false);
            }

            if (_stamina <= 0)
            {
                _stamina = 0;
                _shouldRegenStamina = true;
                _accelerationMultiplier = _walkAcceleration;
                _gun.ToggleRunning(false);
            }

            if (_controller.SprintDown) return;
            if (_isRolling)
            {
                _accelerationMultiplier = _sprintAcceleration;
            }
            else
            {
                _accelerationMultiplier = _walkAcceleration;
            }
        }

        public void EquipGun(string name)
        {
            if (_gun != null)
            {
                DropGun();
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
            var nextGun = Guns.GetNextAfter(_gun?.Parameters.Name ?? "M4").Name;
            EquipGun(nextGun);
        }

        private void Move()
        {
            var deltaTime = Time.deltaTime;

            Acceleration *= _speed * deltaTime;
            Velocity = (0.95f * Velocity + _accelerationMultiplier * Acceleration);

            if (Velocity.Length() < 0.001f) Velocity = Vector2.Zero;
            if (Velocity.Length() > 0) _renderer.UpdateRenderLayerDepth();
            var isColliding = _mover.move(Velocity, out CollisionResult collision);

            if (isColliding)
            {
                if (collision.collider.entity.tag == Tags.Player)
                {
                    var player = collision.collider.entity.getComponent<Player>();
                    player.Acceleration = collision.minimumTranslationVector * 4;
                }
            }
        }

        private void FallIntoPit(Entity pitEntity)
        {
            DisablePlayer();
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
                .setCompletionHandler(_ => DeclareDead())
                .start();
            _renderer.TweenColor(targetColor, durationSeconds, easeType);
        }

        private void DeclareDead()
        {
            _playerState = PlayerState.Dead;
            _cameraTracker.setEnabled(false);
        }

        public void DisablePlayer()
        {
            _controller.SetInputEnabled(false);
            DropGun();
            Velocity = Vector2.Zero;
            Acceleration = Vector2.Zero;
            //_mover.setEnabled(false);
            //_playerHitbox.unregisterColliderWithPhysicsSystem();
            //_playerHitbox.collidesWithLayers = 0;
            //_playerHitbox.setEnabled(false);
            _proximityHitbox.unregisterColliderWithPhysicsSystem();
            _proximityHitbox.setEnabled(false);
            _proximityHitbox.collidesWithLayers = 0;
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

            // Find closest entity based on distance between player and collectible
            var closestEntity = _entitiesInProximity.Aggregate((curMin, x) =>
            ((x.position - entity.position).Length() < (curMin.position - entity.position).Length() ? x : curMin));

            var collectible = closestEntity.getComponent<Collectible>();
            if (collectible == null || !collectible.CanBeCollected()) return;

            if (collectible.Preset.Type == CollectibleType.Weapon)
            {
                EquipGun(collectible.Preset.Gun.Name);
                collectible.OnPickup();
                _entitiesInProximity.Remove(closestEntity);
            }
        }
        public void Damage(int damage)
        {
            Console.WriteLine("Health player " + _controllerIndex + ": " + _health);
            _health -= damage;
            if (_health <= 0 && _playerState != PlayerState.Dying && _playerState != PlayerState.Dead)
            {
                _playerState = PlayerState.Dying;
                DropDead();
                DisablePlayer();
            }
        }

        private void DropDead()
        {
            var easeType = EaseType.BounceOut;
            var durationSeconds = 1.5f;
            var targetRotationDegrees = 90;
            var targetColor = new Color(0.5f, 0.5f, 0.5f, 1f);

            entity.tweenRotationDegreesTo(targetRotationDegrees, durationSeconds)
                .setEaseType(easeType)
                .setCompletionHandler(_ => DeclareDead())
                .start();
            _renderer.TweenColor(targetColor, durationSeconds, easeType);
        }


        public void DropGun()
        {
            if (_gun != null)
            {
                //Throw out a new gunz
                Console.WriteLine($"Dropping {_gun.Parameters.Name}");
                var gunItem = entity.scene.createEntity($"item_{++itemId}");
                var throwedItem = gunItem.addComponent(new Collectible(transform.position.X, transform.position.Y, _gun.Parameters.Name, true));

                throwedItem.Velocity = new Vector2(
                    FacingAngle.X * ThrowSpeed,
                    FacingAngle.Y * ThrowSpeed)
                    + Velocity * 2f;
                UnEquipGun();
            }
        }

        public override void OnDespawn()
        {
            _gun?.Destroy();
        }

        private void SetFacing()
        {
            if (_controller.XRightAxis == 0 && _controller.YRightAxis == 0) return;

            FacingAngle = Lerps.angleLerp(FacingAngle, new Vector2(_controller.XRightAxis, _controller.YRightAxis), Time.deltaTime * 20f);

            if (FacingAngle.Y > 0)
            {
                VerticalFacing = (int)FacingCode.DOWN;
            }
            else
            {
                VerticalFacing = (int)FacingCode.UP;
            }
            //Prioritizing "horizontal" sprites
            if (FacingAngle.X > 0)
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

            _renderer.UpdateRenderLayerDepth();
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
            if (other == null || other.entity == null) return;
            if (_entitiesInProximity.Contains(other.entity)) return;

            Console.WriteLine($"Entered proximity: ${other.entity}");

            _entitiesInProximity.Add(other.entity);

            // TODO change tag to include other interactables if relevant
            if (Flags.isFlagSet(other.entity.tag, Tags.Collectible))
            {
                var collectible = other.entity.getComponent<Collectible>();
                if (collectible.CanBeCollected())
                {
                    collectible?.Highlight();
                }
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
            if (other == null || other.entity == null) return;
            if (!_entitiesInProximity.Contains(other.entity)) return;

            Console.WriteLine($"Left proximity: ${other.entity}");
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
