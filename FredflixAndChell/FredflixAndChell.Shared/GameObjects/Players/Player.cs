using FredflixAndChell.Shared.Components.Cameras;
using FredflixAndChell.Shared.Components.Players;
using FredflixAndChell.Shared.Components.StatusEffects;
using FredflixAndChell.Shared.GameObjects.Bullets;
using FredflixAndChell.Shared.GameObjects.Collectibles;
using FredflixAndChell.Shared.GameObjects.Collectibles.Metadata;
using FredflixAndChell.Shared.GameObjects.Players.Characters;
using FredflixAndChell.Shared.GameObjects.Players.Sprites;
using FredflixAndChell.Shared.GameObjects.Weapons;
using FredflixAndChell.Shared.GameObjects.Weapons.Parameters;
using FredflixAndChell.Shared.Scenes;
using FredflixAndChell.Shared.Systems;
using FredflixAndChell.Shared.Utilities;
using FredflixAndChell.Shared.Utilities.Events;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Tweens;
using System;
using System.Collections.Generic;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.GameObjects.Players
{
    public enum PlayerState
    {
        Idle, Normal, Dying, Dead
    }

    public enum PlayerMobilityState
    {
        Walking, Running, Rolling
    }

    public class Player : GameObject
    {
        private const float WalkAcceleration = 0.25f;
        private const float SprintAcceleration = 0.40f;
        private const float RollAcceleration = 1.20f;
        private const float BaseSlownessFactor = 20f;
        private const int DodgeRollStaminaCost = 50;
        private const float LastHitTimeoutSeconds = 3.0f;

        private static bool DebugToggledRecently { get; set; }

        private Mover _mover;
        private PlayerCollisionHandler _playerCollisionHandler;
        private PlayerRenderer _renderer;
        private CameraTracker _cameraTracker;
        private PlayerController _controller;
        private PlayerInventory _inventory;
        private Collider _proximityHitbox;
        private Collider _playerHitbox;
        private BloodEngine _blood;
        private GameSystem _gameSystem;

        private float _accelerationPlayerFactor;
        private float _deaccelerationPlayerFactor = 0.2f;
        private bool _isRegeneratingStamina = false;

        private List<Entity> _entitiesInProximity;
        private List<Entity> _pitsBelowPlayer;

        private bool _isRollingRight;
        private int _numSprintPressed = 0;
        private bool _isWithinDodgeRollGracePeriod;
        private float _gracePeriod;
        private Vector2 _initialRollingDirection;
        private Player _lastHitPlayerSource;
        private float _lastHitTime;

        public int PlayerIndex { get; set; }
        public PlayerMobilityState PlayerMobilityState { get; set; }
        public PlayerState PlayerState { get; set; }
        public CharacterParameters Parameters { get; private set; }
        public Vector2 Acceleration { get; set; }
        public Vector2 FacingAngle { get; set; }
        public float AimingSlownessFactor { get; set; }
        public float Speed { get; set; } = 50f;
        public float DeaccelerationExternalFactor { get; set; } = 1f;


        public float AccelerationExternalFactor { get; set; } = 1f;
        public float Health { get; set; }
        public float MaxHealth { get; private set; }
        public float MaxStamina { get; private set; }
        public float Stamina { get; private set; }
        public int VerticalFacing { get; set; }
        public int HorizontalFacing { get; set; }
        public int TeamIndex { get; set; }
        public bool FlipGun { get; set; }
        public bool Disarmed { get; set; }

        public Player(CharacterParameters characterParameters, int x, int y, int playerIndex) : base(x, y)
        {
            Parameters = characterParameters;
            PlayerIndex = playerIndex;
            name = $"Player {PlayerIndex}";

            _entitiesInProximity = new List<Entity>();
            _pitsBelowPlayer = new List<Entity>();
        }

        public override void OnSpawn()
        {
            _gameSystem = scene.getSceneComponent<GameSystem>();

            var metadata = ContextHelper.PlayerMetadataByIndex(PlayerIndex);
            if (metadata != null)
            {
                if (metadata.IsInitialized)
                {
                    JoinGame();
                }
            }
            else
            {
                _controller = getComponent<PlayerController>();
            }
        }

        public void OnPitHitboxEnter(Entity entity)
        {
            switch (PlayerState)
            {
                case PlayerState.Normal:
                    break;
                default:
                case PlayerState.Dead:
                case PlayerState.Dying:
                case PlayerState.Idle:
                    return;
            }

            _pitsBelowPlayer.addIfNotPresent(entity);
        }
        public void OnPitHitboxExit(Entity entity)
        {
            _pitsBelowPlayer.Remove(entity);
        }

        //private void SetupDebug()
        //{
        //    var gameSystem = scene.getSceneComponent<GameSystem>();
        //    gameSystem.DebugLines.Add(new DebugLine
        //    {
        //        Text = () => $"Player {PlayerIndex}",
        //        SubLines = new List<DebugLine>
        //        {
        //            new DebugLine{ Text = () => $"Health: {Health}"},
        //            new DebugLine{ Text = () => $"Stamina: {Stamina}"},
        //            new DebugLine{ Text = () => $"Weapon: {_gun?.Parameters.Name ?? "Unarmed"}"},
        //            new DebugLine{ Text = () => $"Mobility state: {PlayerMobilityState.ToString()}"},
        //        }
        //    });
        //}

        private void SetupComponents()
        {
            setTag(Tags.Player);

            // Assign movement component
            _mover = addComponent(new Mover());

            // Assigned by player connector
            _controller = getComponent<PlayerController>();

            // Assign inventory component
            _inventory = addComponent(new PlayerInventory());

            // Assign collider component
            _playerHitbox = addComponent(new CircleCollider(4f));
            _playerHitbox.localOffset = new Vector2(0, 4);
            Flags.setFlagExclusive(ref _playerHitbox.collidesWithLayers, Layers.MapObstacles);
            Flags.setFlag(ref _playerHitbox.collidesWithLayers, Layers.Player);
            Flags.setFlagExclusive(ref _playerHitbox.physicsLayer, Layers.Player);

            // Assign proximity interaction hitbox
            _proximityHitbox = addComponent(new CircleCollider(20f));
            Flags.setFlagExclusive(ref _proximityHitbox.collidesWithLayers, Layers.Interactables);
            Flags.setFlag(ref _proximityHitbox.collidesWithLayers, Layers.Explosion);
            Flags.setFlagExclusive(ref _proximityHitbox.physicsLayer, 0);
            _proximityHitbox.isTrigger = true;

            _playerCollisionHandler = addComponent(new PlayerCollisionHandler(_playerHitbox, _proximityHitbox));

            // Assign renderer component
            SetupRenderer(Parameters.PlayerSprite);

            // Assign camera tracker component
            _cameraTracker = addComponent(new CameraTracker(() => PlayerState != PlayerState.Dead && PlayerState != PlayerState.Idle));

            // Blood
            _blood = addComponent(new BloodEngine(Parameters.BloodColor));

            SetWalkingState();
        }

        private void JoinGame()
        {
            _gameSystem.RegisterPlayer(this);
            SetupComponents();
            var metadata = ContextHelper.PlayerMetadataByIndex(PlayerIndex);
            SetupParameters(metadata);
            //SetupDebug();

            //addComponent(new RegenEffect());

            updateOrder = 0;

            PlayerState = PlayerState.Normal;
            metadata.IsInitialized = true;
        }

        private void SetupRenderer(PlayerSprite playerSprite)
        {
            _renderer = addComponent(new PlayerRenderer(playerSprite, _inventory.Weapon));
        }

        public void ChangePlayerSprite(PlayerSprite playerSprite)
        {
            if (_renderer != null)
            {
                _renderer.setEnabled(false);
                removeComponent(_renderer);
            }
            SetupRenderer(playerSprite);
        }

        private void SetupParameters(PlayerMetadata metadata)
        {
            TeamIndex = metadata.TeamIndex;
            Health = Parameters.MaxHealth;
            MaxHealth = Parameters.MaxHealth;
            Stamina = Parameters.MaxStamina;
            MaxStamina = Parameters.MaxStamina;
            Speed = Parameters.Speed;
        }

        public bool IsArmed()
        {
            return _inventory.IsArmed;
        }

        public override void Update()
        {
            ReadInputs();

            if (PlayerState == PlayerState.Idle) return;

            Move();
            SetFacing();

            // State specific updates
            switch (PlayerMobilityState)
            {
                case PlayerMobilityState.Rolling:
                    break;
                default:
                case PlayerMobilityState.Running:
                case PlayerMobilityState.Walking:
                    if (_pitsBelowPlayer.Count > 0)
                    {
                        FallIntoPit(_pitsBelowPlayer.randomItem());
                    }
                    break;
            }

            if(_lastHitTime > 0 && Time.time > _lastHitTime + LastHitTimeoutSeconds)
            {
                _lastHitTime = -1;
                _lastHitPlayerSource = null;
            }

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
                AimingSlownessFactor = BaseSlownessFactor;
            if (_controller.ReloadPressed)
                Reload();
            if (_controller.DropWeaponPressed)
                _inventory.DropWeapon();
            if (_controller.SwitchWeaponPressed)
                _inventory.SwitchWeapon();
            if (_controller.InteractPressed)
                Interact();
            if (_controller.SprintPressed)
                ToggleDodgeRoll();
            if (_controller.DebugModePressed && !DebugToggledRecently)
            {
                Core.debugRenderEnabled = !Core.debugRenderEnabled;
                DebugToggledRecently = true;
            }

            if (_controller.ExitGameButtonPressed)
            {
                Core.scene = new HubScene();
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
            if (_numSprintPressed == 2 && _controller.SprintPressed && Stamina > DodgeRollStaminaCost && (Acceleration.X != 0 || Acceleration.Y != 0))
            {
                SetRollingState();
                _isRollingRight = FacingAngle.X > 0 ? true : false;
                _numSprintPressed = 0;
                Stamina -= DodgeRollStaminaCost * _gameSystem.Settings.StaminaMultiplier;

                _initialRollingDirection = 1f * Velocity + CalculateAcceleration();
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

        private Vector2 CalculateAcceleration()
        {
            return (_accelerationPlayerFactor * AccelerationExternalFactor) * Acceleration - (_deaccelerationPlayerFactor * DeaccelerationExternalFactor) * Velocity;
        }

        private void ToggleStaminaRegeneration()
        {
            if (!_isRegeneratingStamina)
                return;

            Stamina += 25 * Time.deltaTime;
            if (Stamina >= MaxStamina)
            {
                Stamina = MaxStamina;
                _isRegeneratingStamina = false;
            }
        }

        private void ToggleSprint()
        {
            if (PlayerMobilityState == PlayerMobilityState.Rolling) return;

            if (_controller.SprintDown && (Acceleration.X != 0 || Acceleration.Y != 0))
            {
                if (Stamina <= 0)
                {
                    SetWalkingState();
                }
                else
                {
                    SetRunningState();
                    Stamina -= 50 * _gameSystem.Settings.StaminaMultiplier * Time.deltaTime;
                    _isRegeneratingStamina = false;
                }
            }
            else
            {
                if (Stamina >= Parameters.MaxStamina) return;
                _isRegeneratingStamina = true;
                SetWalkingState();
            }
        }

        private void SetRollingState()
        {
            _accelerationPlayerFactor = RollAcceleration;
            PlayerMobilityState = PlayerMobilityState.Rolling;
        }

        private void SetWalkingState()
        {
            _accelerationPlayerFactor = WalkAcceleration;
            _inventory?.Weapon?.ToggleRunning(false);
            PlayerMobilityState = PlayerMobilityState.Walking;
        }

        private void SetRunningState()
        {
            _accelerationPlayerFactor = SprintAcceleration;
            _inventory?.Weapon?.ToggleRunning(true);
            PlayerMobilityState = PlayerMobilityState.Running;
        }

        public void EquipWeapon(string name, CollectibleMetadata metadata = null)
        {
            _inventory.EquipWeapon(name, metadata);
        }

        private void Move()
        {
            if (PlayerMobilityState == PlayerMobilityState.Rolling) return;
            var deltaTime = Time.deltaTime;

            Acceleration *= Speed * deltaTime;
            Velocity = Velocity + CalculateAcceleration();


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
            Health = 0;
            DisablePlayer(resetVelocity: true);
            DisableHitbox();

            _inventory.DropWeapon();
            _pitsBelowPlayer.Clear();

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

            if (PlayerState != PlayerState.Dying && PlayerState != PlayerState.Dead)
            {
                _gameSystem.Publish(GameEvents.PlayerKilled, new PlayerKilledEventParameters
                {
                    Killed = this,
                    Killer = _lastHitPlayerSource
                });

                PlayerState = PlayerState.Dying;
            }
        }

        private void EnableProximityHitbox()
        {
            Flags.setFlagExclusive(ref _proximityHitbox.collidesWithLayers, Layers.Interactables);
            Flags.setFlag(ref _proximityHitbox.collidesWithLayers, Layers.Explosion);
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

        public void DisablePlayer(bool resetVelocity)
        {
            _controller.SetInputEnabled(false);
            if (resetVelocity)
            {
                Velocity = Vector2.Zero;
            }
            Acceleration = Vector2.Zero;
        }

        public void Attack()
        {
            if (Disarmed || PlayerState == PlayerState.Idle) return;

            _inventory.Attack();
        }

        public void Reload()
        {
            _inventory.Reload();
        }

        public void Interact()
        {
            if (PlayerState == PlayerState.Idle)
            {
                JoinGame();
            }
            else
            {
                InteractWithNearestEntity();
            }
        }


        private void InteractWithNearestEntity()
        {
            _playerCollisionHandler.InteractWithNearestEntity();
        }

        public void Damage(Bullet bullet)
        {
            if (!CanBeDamagedBy(bullet)) return;
            var directionalDamage = new DirectionalDamage
            {
                Damage = bullet.Parameters.Damage,
                Knockback = bullet.Parameters.Knockback,
                Direction = bullet.Velocity,
                SourceOfDamage = bullet.Owner
            };
            Damage(directionalDamage);
        }

        public void Damage(Melee melee)
        {
            //if (!CanBeDamagedBy(bullet)) return;
            var directionalDamage = new DirectionalDamage
            {
                Damage = (melee.Parameters as MeleeParameters).Damage,
                Knockback = (melee.Parameters as MeleeParameters).Knockback,
                Direction = melee.Velocity,
                SourceOfDamage = melee.Player
            };
            Damage(directionalDamage);
        }

        public void Damage(DirectionalDamage dd)
        {
            var scaledDamage = dd.Damage * _gameSystem.Settings.DamageMultiplier;
            var scaledKnockback = dd.Knockback * _gameSystem.Settings.KnockbackMultiplier;

            Health -= scaledDamage;
            Velocity += dd.Direction * scaledKnockback * Time.deltaTime;

            _blood.Sprinkle(scaledDamage, dd.Direction);

            // Override last-hit-by player if directionaldamage came from a player (used for pushing into pit)
            _lastHitPlayerSource = dd.SourceOfDamage ?? _lastHitPlayerSource;
            _lastHitTime = Time.time;

            if (Health <= 0 && PlayerState != PlayerState.Dying && PlayerState != PlayerState.Dead)
            {
                PlayerState = PlayerState.Dying;
                DropDead();
                DisablePlayer(resetVelocity: false);

                _gameSystem.Publish(GameEvents.PlayerKilled, new PlayerKilledEventParameters
                {
                    Killed = this,
                    Killer = _lastHitPlayerSource
                });
            }
        }

        public bool CanBeDamagedBy(Bullet bullet)
        {
            var isFriendlyFire = bullet.Owner.TeamIndex > 0
                && bullet.Owner != this
                && TeamIndex > 0
                && bullet.Owner.TeamIndex == TeamIndex;
            var isFriendlyFireEnabled = _gameSystem.Settings.FriendlyFire;

            return isFriendlyFire && isFriendlyFireEnabled
                || !isFriendlyFire;
        }

        public void SetParameters(CharacterParameters characterParams)
        {
            Parameters = characterParams;
            var meta = ContextHelper.PlayerMetadataByIndex(PlayerIndex);
            SetupParameters(meta);
            ChangePlayerSprite(characterParams.PlayerSprite);
            if (meta != null)
            {
                meta.Character = characterParams;
            }
        }

        public void SetTeamIndex(int teamIndex)
        {
            TeamIndex = teamIndex;
            _renderer.UpdateTeamIndex(teamIndex);

            var meta = ContextHelper.PlayerMetadataByIndex(PlayerIndex);
            meta.TeamIndex = teamIndex;
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

        public override void OnDespawn()
        {
            _inventory?.Weapon?.Destroy();
        }

        private void SetFacing()
        {
            if (PlayerState != PlayerState.Normal) return;

            if (_controller == null || (_controller.XRightAxis == 0 && _controller.YRightAxis == 0)) return;

            FacingAngle = Lerps.angleLerp(FacingAngle, new Vector2(_controller.XRightAxis, _controller.YRightAxis), Time.deltaTime * AimingSlownessFactor);

            if (FacingAngle.Y > 0)
            {
                VerticalFacing = (int)FacingCode.DOWN;
            }
            else
            {
                VerticalFacing = (int)FacingCode.UP;
            }

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
            PlayerMobilityState = PlayerMobilityState.Walking;

            localRotation = 0;
            scale = new Vector2(1.0f, 1.0f);
            _renderer.TweenColor(Color.White, 0.1f);
            _controller.SetInputEnabled(true);
            _cameraTracker.setEnabled(true);
            _blood.StopLeaking();

            _lastHitPlayerSource = null;
            _lastHitTime = -1;

            var meta = ContextHelper.PlayerMetadataByIndex(PlayerIndex);

            SetupParameters(meta);
            EnableHitbox();
            EnableProximityHitbox();

            
            var weapon = meta.Weapon.Name;
            EquipWeapon(weapon);

            // Hack to prevent newly respawned players from being invincible
            Move();
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
