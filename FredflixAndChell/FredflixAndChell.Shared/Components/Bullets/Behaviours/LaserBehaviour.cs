using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FredflixAndChell.Shared.GameObjects.Bullets;
using Nez.Tweens;
using Microsoft.Xna.Framework;
using Nez.Sprites;
using Nez;
using FredflixAndChell.Shared.GameObjects.Players;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.Components.Bullets.Behaviours
{
    public class LaserBehaviour : BulletBehaviour
    {
        public LaserBehaviour(Bullet bullet) : base(bullet)
        {
            _collider = _bullet.addComponent(new BoxCollider(16, 3));
            _mover = _bullet.addComponent(new ProjectileMover());

            Flags.setFlagExclusive(ref _collider.collidesWithLayers, Layers.MapObstacles);
            Flags.setFlag(ref _collider.collidesWithLayers, Layers.Player);
            Flags.setFlagExclusive(ref _collider.physicsLayer, Layers.Bullet);

            _collider.onAddedToEntity();
            _mover.onAddedToEntity();
        }

        public override void OnImpact(Player player)
        {
            if (player != _bullet.Owner)
            {
                DamagePlayer(player);
            }
        }

        public override void OnNonPlayerImpact(CollisionResult collision)
        {
            // Do nothing
        }
        public override void OnFired()
        {
            _bullet.Velocity = Vector2.Zero;
            _bullet.Owner.SlownessFactor = 1f;
            _bullet.position = _bullet.Owner.position;

            var direction = _bullet.Owner.FacingAngle;
            var start = _bullet.Owner.position;
            var end = start + direction * 1000f;

            RaycastHit[] hits = new RaycastHit[256];

            _collider.enabled = false;
            var linecast = Physics.linecastAll(start + new Vector2(0, 3), end, hits, _bullet.Owner.PlayerIndex);
            _collider.enabled = true;

            var closestObstacleHit = new RaycastHit { fraction = 1.0f};
            var potentialPlayerHits = new List<RaycastHit>();
            foreach (var hit in hits)
            {
                if (hit.collider == null) continue;

                if (hit.collider.entity.tag == Tags.Obstacle && hit.fraction < closestObstacleHit.fraction)
                {
                    closestObstacleHit = hit;
                }
                else if (hit.collider.entity.tag == Tags.Player)
                {
                    potentialPlayerHits.Add(hit);
                }
            }

            var distance = closestObstacleHit.point - start;
            var length = distance.Length();

            _bullet.setScale(new Vector2(length / 16f + 0.1f, _bullet.Parameters.Scale));
            _bullet.position += distance / 2 + new Vector2(0, -4);


            foreach (var hit in potentialPlayerHits)
            {
                var player = hit.collider.entity as Player;
                if (hit.fraction < closestObstacleHit.fraction && player != _bullet.Owner)
                {
                    Console.WriteLine($"Player {_bullet.Owner.PlayerIndex} shot Player {player.PlayerIndex}");
                    OnImpact(player);
                }
            }
        }
        public override void update()
        {
            // DO NADA
        }
    }
}
