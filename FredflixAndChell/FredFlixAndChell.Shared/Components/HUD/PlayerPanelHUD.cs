using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Components.Players;
using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.Utilities.Graphics;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Textures;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.Components.HUD
{
    public class PlayerPanelHUD : Entity
    {
        private const float BarStretchToFit = 0.16f;
        private const float ScalingSpeed = 8f;

        private Player _player;
        private PlayerState _previousPlayerState;

        private ScalableSprite _panel;
        private ScalableSprite _head;
        private ScalableSprite _healthBar;
        private ScalableSprite _healthBarFade;
        private ScalableSprite _staminaBar;
        private ScalableSprite _staminaBarFade;


        private int _barTextureWidth = 25;
        private float _uiScale = 4f;
        private float _healthBarScale = 1.0f;
        private float _healthBarFadeScale = 1.0f;
        private float _staminaBarScale = 1.0f;
        private float _staminaBarFadeScale = 1.0f;

        private bool _firstFrame = true;

        public PlayerPanelHUD(Player player, int screenX, int screenY)
        {
            _player = player;

            var hudTexture = AssetLoader.GetTexture("UI/HUD");
            var hudFrontTexture = new Subtexture(hudTexture, 0, 0, 48, 16);
            var bar = new Subtexture(hudTexture, 48, 0, _barTextureWidth, 3);

            _panel = new ScalableSprite(hudFrontTexture);
            _panel.renderLayer = Layers.HUD;
            _panel.layerDepth = 1f;
            _panel.localOffset = new Vector2(screenX + 48, screenY);
            _panel.origin = new Vector2(0, 0);

            _healthBar = new ScalableSprite(bar);
            _healthBar.color = Color.Green;
            _healthBar.localOffset = _panel.localOffset + new Vector2(80, 28);
            _healthBar.renderLayer = Layers.HUD;
            _healthBar.origin = new Vector2(0, 0);

            _healthBarFade = new ScalableSprite(bar);
            _healthBarFade.color = Color.OrangeRed;
            _healthBarFade.localOffset = _panel.localOffset + new Vector2(80, 28);
            _healthBarFade.renderLayer = Layers.HUD;
            _healthBarFade.layerDepth = 0.5f;
            _healthBarFade.origin = new Vector2(0, 0);

            _staminaBar = new ScalableSprite(bar);
            _staminaBar.color = Color.Yellow;
            _staminaBar.localOffset = _panel.localOffset + new Vector2(80, 44);
            _staminaBar.renderLayer = Layers.HUD;
            _staminaBar.origin = new Vector2(0, 0);

            _staminaBarFade = new ScalableSprite(bar);
            _staminaBarFade.color = Color.Gold;
            _staminaBarFade.localOffset = _panel.localOffset + new Vector2(80, 44);
            _staminaBarFade.renderLayer = Layers.HUD;
            _staminaBarFade.layerDepth = 0.5f;
            _staminaBarFade.origin = new Vector2(0, 0);

            _head = new ScalableSprite(bar);
            _head.renderLayer = Layers.HUD;
            _head.layerDepth = 1f;
            _head.localOffset = _panel.localOffset + new Vector2(40, 50);
            _head.origin = new Vector2(0, 0);

            _panel.SetScale(new Vector2(_uiScale, _uiScale));
            _head.SetScale(new Vector2(_uiScale, _uiScale));
            _healthBar.SetScale(new Vector2((_uiScale + BarStretchToFit) * _healthBarScale, _uiScale));
            _healthBarFade.SetScale(new Vector2((_uiScale + BarStretchToFit) * _healthBarFadeScale, _uiScale));
            _staminaBar.SetScale(new Vector2((_uiScale + BarStretchToFit) * _staminaBarScale, _uiScale));
            _staminaBarFade.SetScale(new Vector2((_uiScale + BarStretchToFit) * _staminaBarFadeScale, _uiScale));

            addComponent(_panel);
            addComponent(_healthBar);
            addComponent(_healthBarFade);
            addComponent(_staminaBar);
            addComponent(_staminaBarFade);
            addComponent(_head);
        }

        public override void onAddedToScene()
        {

        }

        public override void update()
        {
            base.update();
            if (_player.PlayerState == PlayerState.Idle) return;
            if (_firstFrame)
            {
                _firstFrame = false;
                var head = _player.getComponent<PlayerRenderer>().Head;

                if (head != null)
                {
                    _head.subtexture = head.subtexture;
                    _head.flipX = head.flipX;
                }
            }

            _healthBarScale = Mathf.lerp(_healthBarScale, _player.Health * 1.0f / _player.MaxHealth, Time.deltaTime * ScalingSpeed);
            _healthBar.SetScale(new Vector2((_uiScale + BarStretchToFit) * _healthBarScale, _uiScale));

            _healthBarFadeScale = Mathf.lerp(_healthBarFadeScale, _player.Health * 1.0f / _player.MaxHealth, Time.deltaTime * ScalingSpeed / 4);
            _healthBarFade.SetScale(new Vector2((_uiScale + BarStretchToFit) * _healthBarFadeScale, _uiScale));

            _staminaBarScale = Mathf.lerp(_staminaBarScale, _player.Stamina * 1.0f / _player.MaxStamina, Time.deltaTime * ScalingSpeed);
            _staminaBar.SetScale(new Vector2((_uiScale + BarStretchToFit) * _staminaBarScale, _uiScale));

            _staminaBarFadeScale = Mathf.lerp(_staminaBarFadeScale, _player.Stamina * 1.0f / _player.MaxStamina, Time.deltaTime * ScalingSpeed / 4);
            _staminaBarFade.SetScale(new Vector2((_uiScale + BarStretchToFit) * _staminaBarFadeScale, _uiScale));

            if (_player.PlayerState != _previousPlayerState)
            {
                if (_player.PlayerState == PlayerState.Dead || _player.PlayerState == PlayerState.Dying)
                {
                    _head.tweenColorTo(new Color(0.1f, 0.1f, 0.1f), 0.4f).start();
                }

                if (_player.PlayerState == PlayerState.Normal)
                {
                    _head.tweenColorTo(Color.White, 0.4f).start();
                }
            }
            _previousPlayerState = _player.PlayerState;
        }
    }

}
