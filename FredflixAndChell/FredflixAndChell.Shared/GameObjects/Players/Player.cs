using FredflixAndChell.Shared.Components.Cameras;
using FredflixAndChell.Shared.Components.PlayerComponents;
using FredflixAndChell.Shared.Components.Players;
using FredflixAndChell.Shared.GameObjects.Bullets;
using FredflixAndChell.Shared.GameObjects.Collectibles;
using FredflixAndChell.Shared.GameObjects.Players.Characters;
using FredflixAndChell.Shared.GameObjects.Weapons;
using FredflixAndChell.Shared.Systems;
using FredflixAndChell.Shared.Utilities.Events;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Tweens;
using System;
using System.Collections.Generic;
using FredflixAndChell.Shared.Utilities;
using static FredflixAndChell.Shared.Assets.Constants;
using static FredflixAndChell.Shared.Components.HUD.DebugHud;
using FredflixAndChell.Shared.Assets;

namespace FredflixAndChell.Shared.GameObjects.Players
{
    public enum PlayerState
    {
        Normal, Dying, Dead
    }

    public enum PlayerMobilityState
    {
        Walking, Running, Rolling
    }

    public class Player : GameObject
    {
        private const float ThrowSpeed = 0.5f;
        private const float WalkAcceleration = 0.25f;
        private const float SprintAcceleration = 0.40f;
        private const float RollAcceleration = 1.20f;
        private const float BaseSlownessFactor = 20f;
        private const int DodgeRollStaminaCost = 50;

        private static bool DebugToggledRecently { get; set; }

        private readonly CharacterParameters _params;
        private Mover _mover;
        private PlayerCollisionHandler _playerCollisionHandler;
        private PlayerRenderer _renderer;
        private CameraTracker _cameraTracker;
        private PlayerController _controller;
        private Collider _proximityHitbox;
        private Collider _playerHitbox;
        private BloodEngine _blood;
        private GameSystem _gameSystem;

        private Gun _gun;

        private float _health;
        private float _maxHealth;
        private float _stamina;
        private float _maxStamina;
        private float _speed = 50f;
        private float _accelerationMultiplier;
        private bool _isRegeneratingStamina = false;

        private List<Entity> _entitiesInProximity;
        private bool _isRollingRight;
        private int _numSprintPressed = 0;
        private bool _isWithinDodgeRollGracePeriod;
        private float _gracePeriod;
        private float _slownessFactor;
        private Vector2 _initialRollingDirection;

        public PlayerMobilityState PlayerMobilityState { get; set; }
        public PlayerState PlayerState { get; set; }
        public CharacterParameters Parameters => _params;
        public float SlownessFactor { get => _slownessFactor; set => _slownessFactor = value; }
        public Vector2 Acceleration { get; set; }
        public Vector2 FacingAngle { get; set; }
        public int PlayerIndex { get; set; }
        public int Health => (int)_health;
        public int MaxHealth => (int)_maxHealth;
        public float MaxStamina => (int)_maxStamina;
        public int Stamina => (int)_stamina;
        public int VerticalFacing { get; set; }
        public int HorizontalFacing { get; set; }
        public bool IsArmed { get; set; } = true;
        public bool FlipGun { get; set; }
        public int TeamIndex { get; set; }

        public Player(CharacterParameters characterParameters, int x, int y, int playerIndex) : base(x, y)
        {
            _params = characterParameters;
            _entitiesInProximity = new List<Entity>();
            PlayerIndex = playerIndex;
            name = $"Player {PlayerIndex}";
        }

        public override void OnSpawn()
        {
            SetupComponents();
            SetupParameters();
            SetupDebug();

            _gameSystem.RegisterPlayer(this);
            updateOrder = 0;
        }

        private void SetupDebug()
        {
            var gameSystem = scene.getSceneComponent<GameSystem>();
            gameSystem.DebugLines.Add(new DebugLine
            {
                Text = () => $"Player {PlayerIndex}",
                SubLines = new List<DebugLine>
                {
                    new DebugLine{ Text = () => $"Health: {Health}"},
                    new DebugLine{ Text = () => $"Stamina: {Stamina}"},
                    new DebugLine{ Text = () => $"Weapon: {_gun?.Parameters.Name ?? "Unarmed"}"},
                    new DebugLine{ Text = () => $"Mobility state: {PlayerMobilityState.ToString()}"},
                }
            });
        }

        private void SetupComponents()
        {
            setTag(Tags.Player);
            
            // Assign movement component
            _mover = addComponent(new Mover());

            // Assigned by player connector
            _controller = getComponent<PlayerController>();

            // Assign gun component
            EquipGun("M4");

            // Assign collider component
            _playerHitbox = addComponent(new CircleCollider(4f));
            _playerHitbox.localOffset = new Vector2(0, 4);
            Flags.setFlagExclusive(ref _playerHitbox.collidesWithLayers, Layers.MapObstacles);
            Flags.setFlag(ref _playerHitbox.collidesWithLayers, Layers.Player);
            Flags.setFlagExclusive(ref _playerHitbox.physicsLayer, Layers.Player);

            // Assign proximity interaction hitbox
            _proximityHitbox = addComponent(new CircleCollider(20f));
            Flags.setFlagExclusive(ref _proximityHitbox.collidesWithLayers, Layers.Interactables);
            Flags.setFlagExclusive(ref _proximityHitbox.physicsLayer, 0);
            _proximityHitbox.isTrigger = true;

            _playerCollisionHandler = addComponent(new PlayerCollisionHandler(_playerHitbox, _proximityHitbox));

            // Assign renderer component
            _renderer = addComponent(new PlayerRenderer(_params.PlayerSprite, _gun));

            // Assign camera tracker component
            _cameraTracker = addComponent(new CameraTracker(() => PlayerState != PlayerState.Dead));

            // Blood
            _blood = addComponent(new BloodEngine());

            _gameSystem = scene.getSceneComponent<GameSystem>();
            SetWalkingState();
        }

        private void SetupParameters()
        {
            _health = _params.MaxHealth;
            _maxHealth = _params.MaxHealth;
            _stamina = _params.MaxStamina;
            _maxStamina = _params.MaxStamina;
            _speed = _params.Speed;
        }

        public override void Update()
        {
            ReadInputs();
            Move();
            SetFacing();
            if (Time.frameCount % 5 == 0)
            {
                DebugToggledRecently = false;
            }
        }

        private void ReadInputs()
        {
            if (_controller == null || !_controller.InputEnabled) return;

            if (_controller.FirePressed)
                Attack();
            if (!_controller.FirePressed)
                SlownessFactor = BaseSlownessFactor;
            if (_controller.ReloadPressed)
                Reload();
            if (_controller.DropGunPressed)
                DropGun();
            if (_controller.SwitchWeaponPressed)
                SwitchWeapon();
            if (_controller.InteractPressed)
                Interact();
            if (_controller.SprintPressed)
                ToggleDodgeRoll();
            if (_controller.DebugModePressed && !DebugToggledRecently)
            {
                Core.debugRenderEnabled = !Core.debugRenderEnabled;
                DebugToggledRecently = true;
            }

            HandleDodgeRollGracePeriod();
            PerformDodgeRoll();
            ToggleSprint();
            ToggleStaminaRegeneration();

            Acceleration = new Vector2(_controller.XLeftAxis, _controller.YLeftAxis);
        }

        private void HandleDodgeRollGracePeriod()
        {
            if (_isWithinDodgeRollGracePeriod)
            {
                _gracePeriod += 1 * Time.deltaTime;
            }
            if (_gracePeriod > 1)
            {
                _gracePeriod = 0;
                _numSprintPressed = 0;
                _isWithinDodgeRollGracePeriod = false;
            }
        }

        private void ToggleDodgeRoll()
        {
            if (PlayerMobilityState == PlayerMobilityState.Rolling) return;
            if (_numSprintPressed == 0)
            {
                _numSprintPressed++;
                _isWithinDodgeRollGracePeriod = true;
            }

            else if (_numSprintPressed == 1 && _gracePeriod < 1)
            {
                _numSprintPressed++;
            }
            else
            {
                _isWithinDodgeRollGracePeriod = false;
                _numSprintPressed = 0;
                _gracePeriod = 0;
            }
        }

        private void PerformDodgeRoll()
        {
            if (_numSprintPressed == 2 && _controller.SprintPressed && _stamina > DodgeRollStaminaCost && (Acceleration.X != 0 || Acceleration.Y != 0))
            {
                SetRollingState();
                _isRollingRight = FacingAngle.X > 0 ? true : false;
                _numSprintPressed = 0;
                _stamina -= DodgeRollStaminaCost;

                _initialRollingDirection = (1f * Velocity + _accelerationMultiplier * Acceleration);
            }

            if (PlayerMobilityState == PlayerMobilityState.Rolling)
            {
                _mover.move(_initialRollingDirection, out CollisionResult collision);

                if (_isRollingRight)
                {
                    localRotation += 4 * (float)Math.PI * Time.deltaTime;
                }
                else
                {
                    localRotation -= 4 * (float)Math.PI * Time.deltaTime;
                }
            }

            if ((_isRollingRight && localRotation >= (2 * Math.PI)) || (!_isRollingRight && localRotation <= (-2 * Math.PI)))
            {
                SetWalkingState();
                localRotation = 0;
                _numSprintPressed = 0;
            }
        }

        private void ToggleStaminaRegeneration()
        {
            if (!_isRegeneratingStamina)
                return;

            _stamina += 25 * Time.deltaTime;
            if (_stamina >= _maxStamina)
            {
                _stamina = _maxStamina;
                _isRegeneratingStamina = false;
            }
        }

        private void ToggleSprint()
        {
            if (PlayerMobilityState == PlayerMobilityState.Rolling) return;

            if (_controller.SprintDown && (Acceleration.X != 0 || Acceleration.Y != 0))
            {
                if (_stamina <= 0)
                {
                    SetWalkingState();
                }
                else
                {
                    SetRunningState();
                    _stamina -= 50 * Time.deltaTime;
                    _isRegeneratingStamina = false;
                }
            }
            else
            {
                if (_stamina >= 100) return;
                _isRegeneratingStamina = true;
                SetWalkingState();
            }
        }

        private void SetRollingState()
        {
            _accelerationMultiplier = RollAcceleration;
            PlayerMobilityState = PlayerMobilityState.Rolling;
        }

        private void SetWalkingState()
        {
            _accelerationMultiplier = WalkAcceleration;
            _gun?.ToggleRunning(false);
            PlayerMobilityState = PlayerMobilityState.Walking;
        }

        private void SetRunningState()
        {
            _accelerationMultiplier = SprintAcceleration;
            _gun?.ToggleRunning(true);
            PlayerMobilityState = PlayerMobilityState.Running;
        }

        public void EquipGun(string name)
        {
            if (_gun != null)
            {
                DropGun();
            }
            _gun = scene.addEntity(new Gun(this, Guns.Get(name)));
            IsArmed = true;
        }

        public void UnEquipGun()
        {
            IsArmed = false;
            _gun.destroy();
            _gun = null;
        }

        private void SwitchWeapon()
        {
            var nextGun = Guns.GetNextAfter(_gun?.Parameters.Name ?? "M4").Name;
            EquipGun(nextGun);
        }

        private void Move()
        {
            if (PlayerMobilityState == PlayerMobilityState.Rolling) return;
            var deltaTime = Time.deltaTime;

            Acceleration *= _speed * deltaTime;
            Velocity = (0.8f * Velocity + _accelerationMultiplier * Acceleration);

            if (Velocity.Length() < 0.001f) Velocity = Vector2.Zero;
            if (Velocity.Length() > 0) _renderer.UpdateRenderLayerDepth();
            var isColliding = _mover.move(Velocity, out CollisionResult collision);

            if (isColliding)
            {
                if (collision.collider.entity is Player player)
                {
                    player.Acceleration = collision.minimumTranslationVector * 4;
                }
            }
        }

        public void FallIntoPit(Entity pitEntity)
        {
            if (PlayerState != PlayerState.Normal) return;

            _health = 0;
            DisablePlayer();
            DisableHitbox();
            PlayerState = PlayerState.Dying;
            var easeType = EaseType.CubicOut;
            var durationSeconds = 2f;
            var targetScale = 0.2f;
            var targetRotationDegrees = 180;
            var targetColor = new Color(0, 0, 0, 0.25f);
            var destination = pitEntity.localPosition;

            this.tweenRotationDegreesTo(targetRotationDegrees, durationSeconds)
                .setEaseType(easeType)
                .start();
            this.tweenScaleTo(targetScale, durationSeconds)
                .setEaseType(easeType)
                .start();
            this.tweenPositionTo(destination, durationSeconds)
                .setEaseType(easeType)
                .setCompletionHandler(_ => DeclareDead())
                .start();
            _renderer.TweenColor(targetColor, durationSeconds, easeType);

            _gameSystem.Publish(GameEvents.PlayerKilled, new PlayerKilledEventParameters
            {
                Killed = this
            });
        }

        private void EnableProximityHitbox()
        {
            Flags.setFlagExclusive(ref _proximityHitbox.collidesWithLayers, Layers.Interactables);
            _proximityHitbox.registerColliderWithPhysicsSystem();
            _proximityHitbox.setEnabled(true);
        }

        private void DisableProximityHitbox()
        {
            _proximityHitbox.collidesWithLayers = 0;
            _proximityHitbox.unregisterColliderWithPhysicsSystem();
            _proximityHitbox.setEnabled(false);
        }

        private void EnableHitbox()
        {
            _mover.setEnabled(true);
            Flags.setFlagExclusive(ref _playerHitbox.collidesWithLayers, Layers.MapObstacles);
            Flags.setFlag(ref _playerHitbox.collidesWithLayers, Layers.Player);
            Flags.setFlagExclusive(ref _playerHitbox.physicsLayer, Layers.Player);
            _playerHitbox.registerColliderWithPhysicsSystem();
            _playerHitbox.setEnabled(true);
        }

        private void DisableHitbox()
        {
            _mover.setEnabled(false);
            _playerHitbox.unregisterColliderWithPhysicsSystem();
            _playerHitbox.collidesWithLayers = 0;
            _playerHitbox.setEnabled(false);
        }

        private void DeclareDead()
        {
            PlayerState = PlayerState.Dead;
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
            DisableProximityHitbox();
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
            InteractWithNearestEntity();
        }

        private void InteractWithNearestEntity()
        {
            _playerCollisionHandler.InteractWithNearestEntity();
        }

        public void Damage(Bullet bullet)
        {
            if (!CanBeDamagedBy(bullet)) return;
            var damage = bullet.Parameters.Damage;
            _health -= damage;
            _blood.Sprinkle(damage, bullet.Velocity);
            Velocity += bullet.Velocity * bullet.Parameters.Knockback * Time.deltaTime;

            if (_health <= 0 && PlayerState != PlayerState.Dying && PlayerState != PlayerState.Dead)
            {
                PlayerState = PlayerState.Dying;
                DropDead();
                DisablePlayer();

                _gameSystem.Publish(GameEvents.PlayerKilled, new PlayerKilledEventParameters
                {
                    Killed = this,
                    Killer = bullet.Owner
                });
            }
        }

        public bool CanBeDamagedBy(Bullet bullet)
        {
            var isFriendlyFire = bullet.Owner.TeamIndex > 0
                && TeamIndex > 0
                && bullet.Owner.TeamIndex == TeamIndex;
            var isFriendlyFireEnabled = _gameSystem.Settings.FriendlyFire;

            return isFriendlyFire && isFriendlyFireEnabled
                || !isFriendlyFire;
        }

        private void DropDead()
        {
            var easeType = EaseType.BounceOut;
            var durationSeconds = 1.5f;
            var targetRotationDegrees = 90;
            var targetColor = new Color(0.5f, 0.5f, 0.5f, 1f);

            this.tweenRotationDegreesTo(targetRotationDegrees, durationSeconds)
                .setEaseType(easeType)
                .setCompletionHandler(_ => DeclareDead())
                .start();
            _renderer.TweenColor(targetColor, durationSeconds, easeType);
            _blood.Blast();
            _blood.Leak();
        }

        public void DropGun()
        {
            if (_gun != null)
            {
                //Throw out a new gunz
                var throwedItem = scene.addEntity(new Collectible(transform.position.X, transform.position.Y, _gun.Parameters.Name, true));

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
            if (PlayerState != PlayerState.Normal) return;

            if (_controller == null || (_controller.XRightAxis == 0 && _controller.YRightAxis == 0)) return;


            FacingAngle = Lerps.angleLerp(FacingAngle, new Vector2(_controller.XRightAxis, _controller.YRightAxis), Time.deltaTime * _slownessFactor);

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

        public void Respawn(Vector2 furthestSpawn)
        {
            position = furthestSpawn;
            PlayerState = PlayerState.Normal;

            localRotation = 0;
            scale = new Vector2(1.0f, 1.0f);
            _renderer.TweenColor(Color.White, 0.1f);
            _controller.SetInputEnabled(true);
            _cameraTracker.setEnabled(true);
            _blood.StopLeaking();

            SetupParameters();
            EnableHitbox();
            EnableProximityHitbox();
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
