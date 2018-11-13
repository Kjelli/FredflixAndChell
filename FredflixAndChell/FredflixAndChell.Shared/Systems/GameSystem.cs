using System.Collections.Generic;
using Nez;
using FredflixAndChell.Shared.GameObjects.Players;

namespace FredflixAndChell.Shared.Systems
{
    public class GameSystem : EntitySystem
    {
        private Dictionary<int, Player> _playerStandings;

        public Dictionary<int, Player> PlayerStandings => _playerStandings;
        public GameSystem(Matcher matcher) : base(matcher)
        {
            _playerStandings = new Dictionary<int, Player>();
        }

        protected override void process(List<Entity> entities)
        {
            _playerStandings.Clear();
            foreach(var playerEntity in entities)
            {
                var player = playerEntity.getComponent<Player>();
                _playerStandings[player.PlayerIndex] = player;
            }
        }
    }
}
