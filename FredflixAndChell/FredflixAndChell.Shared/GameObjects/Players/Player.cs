using FredflixAndChell.Shared.Components.Cameras;
using FredflixAndChell.Shared.Components.Players;
using FredflixAndChell.Shared.Components.StatusEffects;
using FredflixAndChell.Shared.GameObjects.Bullets;
using FredflixAndChell.Shared.GameObjects.Collectibles;
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
using System.Linq;
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
        private const float ThrowSpeed = 0.5f;
        private const float WalkAcceleration = 0.25f;
        private const float SprintAcceleration = 0.40f;
        private const float RollAcceleration = 1.20f;
        private const float BaseSlownessFactor = 20f;
        private const int DodgeRollStaminaCost = 50;

        private static bool DebugToggledRecently { get; set; }

        private Mover _mover;
        private PlayerCollisionHandler _playerCollisionHandler;
        private PlayerRenderer _renderer;
        private CameraTracker _cameraTracker;
        private PlayerController _controller;
        private Collider _proximityHitbox;
        private Collider _playerHitbox;
        private BloodEngine _blood;
        private GameSystem _gameSystem;


        private Weapon _weapon;
        private float _maxHealth;
        private float _stamina;

        private float _maxStamina;
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
        private Player _lastHitByPlayer;

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
        public float MaxHealth => _maxHealth;
        public float MaxStamina => _maxStamina;
        public int Stamina => (int)_stamina;
        public int VerticalFacing { get; set; }
        public int HorizontalFacing { get; set; }


        public int TeamIndex { get; set; }
        public bool IsArmed { get; set; } = true;
        public bool FlipGun { get; set; }
        public bool Disarmed { get; set; }
        public WeaponParameters WeaponParameters { get; set; }

        public Player(CharacterParameters characterParameters, WeaponParameters weaponParameters, int x, int y, int playerIndex) : base(x, y)
        {
            Parameters = characterParameters;
            WeaponParameters = weaponParameters;
            PlayerIndex = playerIndex;
            name = $"Player {PlayerIndex}";

            _entitiesInProximity = new List<Entity>();
            _pitsBelowPlayer = new List<Entity>();
        }

        public override void OnSpawn()
        {
            var metadata = ContextHelper.PlayerMetadata.FirstOrDefault(x => x.PlayerIndex == PlayerIndex);
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

            // Assign gun component
            EquipWeapon(WeaponParameters.Name);

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

            _gameSystem = scene.getSceneComponent<GameSystem>();
            SetWalkingState();
        }

        private void SetupRenderer(PlayerSprite playerSprite)
        {
            _renderer = addComponent(new PlayerRenderer(playerSprite, _weapon));
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

        private void SetupParameters()
        {
            Health = Parameters.MaxHealth;
            _maxHealth = Parameters.MaxHealth;
            _stamina = Parameters.MaxStamina;
            _maxStamina = Parameters.MaxStamina;
            Speed = Parameters.Speed;

            var playerMetadata = ContextHelper.PlayerMetadata.FirstOrDefault(x => x.PlayerIndex == PlayerIndex);
            if (playerMetadata != null)
            {
                EquipWeapon(playerMetadata.Weapon.Name);
            }
        }

        public override void Update()
        {
            ReadInputs();

            if (PlayerState == PlayerState.Idle) return;

            Move();
            SetFacing();

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
            if (_numSprintPressed == 2 && _controller.SprintPressed && _stamina > DodgeRollStaminaCost && (Acceleration.X != 0 || Acceleration.Y != 0))
            {
                SetRollingState();
                _isRollingRight = FacingAngle.X > 0 ? true : false;
                _numSprintPressed = 0;
                _stamina -= DodgeRollStaminaCost * _gameSystem.Settings.StaminaMultiplier;

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
                    _stamina -= 50 * _gameSystem.Settings.StaminaMultiplier * Time.deltaTime;
                    _isRegeneratingStamina = false;
                }
            }
            else
            {
                if (_stamina >= Parameters.MaxStamina) return;
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
            _weapon?.ToggleRunning(false);
            PlayerMobilityState = PlayerMobilityState.Walking;
        }

        private void SetRunningState()
        {
            _accelerationPlayerFactor = SprintAcceleration;
            _weapon?.ToggleRunning(true);
            PlayerMobilityState = PlayerMobilityState.Running;
        }

        public void EquipWeapon(string name)
        {
            if (_weapon != null)
            {
#if DEBUG
                DropGun();
#else
                UnEquipGun();
#endif
            }

            var gunParams = Guns.Get(name);
            if (gunParams != null)
            {
                _weapon = scene.addEntity(new Gun(this, gunParams));
                IsArmed = true;

                var meta = ContextHelper.PlayerMetadata.FirstOrDefault(p => p.PlayerIndex == PlayerIndex);
                if (meta != null)
                {
                    meta.Weapon = gunParams;
                }
            }
            else
            {
                var meleeParams = Melees.Get(name);
                if (meleeParams != null)
                {
                    _weapon = scene.addEntity(new Melee(this, meleeParams));
                    IsArmed = true;

                    var meta = ContextHelper.PlayerMetadata.FirstOrDefault(p => p.PlayerIndex == PlayerIndex);
                    if (meta != null)
                    {
                        meta.Weapon = meleeParams;
                    }
                }
            }
        }

        public void UnEquipGun()
        {
            IsArmed = false;
            _weapon.destroy();
            _weapon = null;
        }

        private void SwitchWeapon()
        {
#if DEBUG

            if (_weapon != null)
            {
                if (_weapon is Melee melee)
                {
                    var nextMelee = Melees.GetNextAfter(melee.Parameters.Name ?? "Stick").Name;
                    EquipWeapon(nextMelee);
                }
                else if (_weapon is Gun gun)
                {
                    var nextGun = Guns.GetNextAfter(gun.Parameters.Name ?? "M4").Name;
                    EquipWeapon(nextGun);
                }
            }
#endif
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
            PlayerState = PlayerState.Dying;

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

            _gameSystem.Publish(GameEvents.PlayerKilled, new PlayerKilledEventParameters
            {
                Killed = this
            });
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
            DropGun();
            if (resetVelocity)
            {
                Velocity = Vector2.Zero;
            }
            Acceleration = Vector2.Zero;
        }

        public void Attack()
        {
            if (Disarmed) return;

            if (_weapon != null)
                _weapon.Fire();
        }

        public void Reload()
        {
            if (_weapon != null && _weapon is Gun _gun)
                _gun.ReloadMagazine();
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

        private void JoinGame()
        {
            SetupComponents();
            SetupParameters();
            //SetupDebug();

            addComponent(new RegenEffect());

            _gameSystem.RegisterPlayer(this);
            updateOrder = 0;

            PlayerState = PlayerState.Normal;
            ContextHelper.PlayerMetadata.First(x => x.PlayerIndex == PlayerIndex).IsInitialized = true;
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
            };
            _lastHitByPlayer = bullet.Owner;
            Damage(directionalDamage);
        }

        public void Damage(DirectionalDamage dd)
        {
            var scaledDamage = dd.Damage * _gameSystem.Settings.DamageMultiplier;
            var scaledKnockback = dd.Knockback * _gameSystem.Settings.KnockbackMultiplier;

            Health -= scaledDamage;
            Velocity += dd.Direction * scaledKnockback * Time.deltaTime;

            _blood.Sprinkle(scaledDamage, dd.Direction);

            if (Health <= 0 && PlayerState != PlayerState.Dying && PlayerState != PlayerState.Dead)
            {
                PlayerState = PlayerState.Dying;
                DropDead();
                DisablePlayer(resetVelocity: false);

                _gameSystem.Publish(GameEvents.PlayerKilled, new PlayerKilledEventParameters
                {
                    Killed = this,
                    Killer = _lastHitByPlayer
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

        public void SetParameters(CharacterParameters nextCharacter)
        {
            Parameters = nextCharacter;
            SetupParameters();
            ChangePlayerSprite(nextCharacter.PlayerSprite);
            var meta = ContextHelper.PlayerMetadata.FirstOrDefault(p => p.PlayerIndex == PlayerIndex);
            if (meta != null)
            {
                meta.Character = nextCharacter;
            }
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
            if (_weapon != null)
            {
                Collectible throwedItem = null;
                if (_weapon is Gun gun)
                {
                    //Throw out a new gunz
                    throwedItem = scene.addEntity(new Collectible(transform.position.X, transform.position.Y, gun.Parameters.Name, true));

                }
                else if (_weapon is Melee melee)
                {
                    //Throw out a new gunz
                    throwedItem = scene.addEntity(new Collectible(transform.position.X, transform.position.Y, melee.Parameters.Name, true));
                }

                throwedItem.Velocity = new Vector2(
                        FacingAngle.X * ThrowSpeed,
                        FacingAngle.Y * ThrowSpeed)
                        + Velocity * 2f;
                UnEquipGun();
            }
        }

        public override void OnDespawn()
        {
            _weapon?.Destroy();
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
