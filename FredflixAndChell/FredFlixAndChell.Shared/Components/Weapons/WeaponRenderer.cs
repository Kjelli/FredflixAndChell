using FredflixAndChell.Shared.GameObjects.Players;
using FredflixAndChell.Shared.GameObjects.Weapons;
using Microsoft.Xna.Framework;
using Nez;

namespace FredflixAndChell.Shared.Components.Weapons
{
    public class WeaponRenderer : Component, IUpdatable
    {
        private Weapon _gun;
        private float _renderOffset;

        public Player Player { get; set; }
        public bool IsPlayerRunning { get; set; }
        public Color PlayerSkinColor { get; set; }

        public WeaponRenderer(Player player)
        {
            Player = player;
            PlayerSkinColor = player.Parameters.SkinColor;
        }

        public void ToggleRunningDisplacement(bool isRunning)
        {
            IsPlayerRunning = isRunning;
        }

        public virtual void Fire()
        {
        }

        public virtual void update()
        {
        }

        //public override void onAddedToEntity()
        //{
        //    base.onAddedToEntity();
        //    entity.setScale(_gun.Parameters.Scale);
        //    setUpdateOrder(1);

        //    var handColorizer = AssetLoader.GetEffect("weapon_hand_color");
        //    handColorizer.Parameters["hand_color"].SetValue(_playerSkinColor.ToVector4());
        //    handColorizer.Parameters["hand_border_color"].SetValue(_playerSkinColor.subtract(new Color(0.1f, 0.1f, 0.1f, 0.0f)).ToVector4());

        //    _animation = entity.addComponent(SetupAnimations(_melee.Parameters.Sprite));
        //    _animation.renderLayer = Layers.Player;
        //    _animation.material = new Material(BlendState.NonPremultiplied, handColorizer);

        //    var shadow = entity.addComponent(new SpriteMime(_animation));
        //    shadow.color = new Color(0, 0, 0, 80);
        //    shadow.material = Material.stencilRead(Stencils.EntityShadowStencil);
        //    shadow.renderLayer = Layers.Shadow;
        //    shadow.localOffset = new Vector2(1, 2);

        //    // Assign silhouette component when gun is visually blocked
        //    var silhouette = entity.addComponent(new SpriteMime(_animation));
        //    silhouette.color = new Color(0, 0, 0, 80);
        //    silhouette.material = Material.stencilRead(Stencils.HiddenEntityStencil);
        //    silhouette.renderLayer = Layers.Foreground;
        //    silhouette.localOffset = new Vector2(0, 0);

        //    _animation.play(MeleeAnimations.Held_Idle);
        //}
    }
}
