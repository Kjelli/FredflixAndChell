using FredflixAndChell.Shared.Components.Cameras;
using FredflixAndChell.Shared.Components.PlayerComponents;
using FredflixAndChell.Shared.Components.Players;
using FredflixAndChell.Shared.GameObjects.Collectibles;
using FredflixAndChell.Shared.GameObjects.Players.Characters;
using FredflixAndChell.Shared.GameObjects.Weapons;
using FredflixAndChell.Shared.Particles;
using FredflixAndChell.Shared.Systems;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Tweens;
using System;
using System.Collections.Generic;
using static FredflixAndChell.Shared.Assets.Constants;
using static FredflixAndChell.Shared.Components.HUD.DebugHud;

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

        private readonly CharacterParameters _params;

        private PlayerState _playerState;

        private Mover _mover;
        private PlayerCollisionHandler _playerCollisionHandler;
        private PlayerRenderer _renderer;
        private CameraTracker _cameraTracker;
        private PlayerController _controller;
        private Collider _proximityHitbox;
        private Collider _playerHitbox;
        private BloodEngine _blood;

        private Gun _gun;

        private float _health;
        private float _maxHealth;
        private float _stamina;
        private float _maxStamina;
        private float _speed = 50f;
        private float _accelerationMultiplier;
        private bool _isRegeneratingStamina = false;
        private readonly float _walkAcceleration = 0.05f;
        private readonly float _sprintAcceleration = 0.10f;

        private List<Entity> _entitiesInProximity;
        private bool _isRolling;
        private bool _isRollingRight;
        private int _numSprintPressed = 0;
        private bool _isWithinDodgeRollGracePeriod;
        private float _gracePeriod;

        public PlayerState PlayerState => _playerState;
        public CharacterParameters Parameters => _params;
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
            scene.getSceneComponent<GameSystem>().RegisterPlayer(this);
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
            EquipGun("Goggles");

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
            _cameraTracker = addComponent(new CameraTracker(() => _playerState != PlayerState.Dead));

            //Particles
            //TODO: Disse to linjene (eller komponetner) blir brukt, bare for stek
            //_bloodParticles = _particlesEntity.addComponent(new ParticleEngine(ParticleDesigner.flame));

            _blood = addComponent(new BloodEngine());
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
        }

        private void ReadInputs()
        {
            
            if (_controller == null || !_controller.InputEnabled) return;

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
            if (_controller.SprintPressed)
                ToggleDodgeRoll();
            if (_controller.DebugModePressed)
                Core.debugRenderEnabled = !Core.debugRenderEnabled;

            HandleDodgeRollGracePeriod();
            ToggleSprint();
            ToggleStaminaRegeneration();
            PerformDodgeRoll();

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
            if (_isRolling) return;
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
            if (_isRegeneratingStamina) return;
            if (_numSprintPressed == 2 && _controller.SprintPressed)
            {
                _isRolling = true;
                _isRollingRight = FacingAngle.X > 0 ? true : false;
            }

            if (_isRolling)
            {
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
                _isRolling = false;
                localRotation = 0;
                _numSprintPressed = 0;
                _stamina -= 50;
            }
        }

        private void ToggleStaminaRegeneration()
        {
            if (!_isRegeneratingStamina)
                return;

            _stamina += 25 * Time.deltaTime;
            if (_stamina >= 100)
            {
                _stamina = 100;
                _isRegeneratingStamina = false;
            }
        }

        private void ToggleSprint()
        {
            if (_controller.SprintDown && !_isRegeneratingStamina)
            {
                _accelerationMultiplier = _sprintAcceleration;
                _stamina -= 50 * Time.deltaTime;
                _gun?.ToggleRunning(true);
            }
            else
            {
                _accelerationMultiplier = _walkAcceleration;
                _gun?.ToggleRunning(false);
            }

            if (_stamina <= 0)
            {
                _stamina = 0;
                _isRegeneratingStamina = true;
                _accelerationMultiplier = _walkAcceleration;
                _gun?.ToggleRunning(false);
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
            var deltaTime = Time.deltaTime;

            Acceleration *= _speed * deltaTime;
            Velocity = (0.95f * Velocity + _accelerationMultiplier * Acceleration);

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
            _health = 0;
            DisablePlayer();
            DisableHitbox();
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
            InteractWithNearestEntity();
        }

        private void InteractWithNearestEntity()
        {
            _playerCollisionHandler.InteractWithNearestEntity();
        }

        public void Damage(int damage, Vector2 damageDirection)
        {
            _health -= damage;
            _blood.Sprinkle(damage, damageDirection);
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
    }

    public enum FacingCode
    {
        UP = 1,
        RIGHT = 2,
        DOWN = 3,
        LEFT = 4
    }
}
