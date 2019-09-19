using FredflixAndChell.Shared.GameObjects.Players;
using Nez;

namespace FredflixAndChell.Shared.Components.StatusEffects
{
    public abstract class StatusEffect : Component, IUpdatable
    {
        protected Player _affectedPlayer;
        private float _durationSeconds;
        private float _tickIntervalSeconds;
        private bool _shouldBeRemoved;

        protected Player _sourceOfStatus;

        private ITimer _timer;


        public StatusEffect(float durationSeconds = -1, float tickIntervalSeconds = 0, Player sourceOfStatus = null)
        {
            _durationSeconds = durationSeconds;
            _tickIntervalSeconds = tickIntervalSeconds;
            _sourceOfStatus = sourceOfStatus;
        }

        public override void onAddedToEntity()
        {
            base.onAddedToEntity();
            _affectedPlayer = (Player)entity;
            if (_durationSeconds > 0)
            {
                _timer = Core.schedule(_durationSeconds, _ => Remove());
            }

            OnEffectAdded(_affectedPlayer);
        }

        protected abstract void OnEffectAdded(Player player);
        protected abstract void OnEffectRemoved(Player player);
        protected abstract void Effect(Player player);
        public virtual void update()
        {
            if (_shouldBeRemoved)
            {
                OnEffectRemoved(_affectedPlayer);
                _affectedPlayer.removeComponent(this);
                return;
            }

            if (_tickIntervalSeconds > 0)
            {
                if (Time.checkEvery(_tickIntervalSeconds))
                {
                    Effect(_affectedPlayer);
                }
            }
            else
            {
                Effect(_affectedPlayer);
            }
        }

        public void Remove()
        {
            _shouldBeRemoved = true;
        }

        public void Reapply()
        {
            _timer?.reset();
        }
    }
}
