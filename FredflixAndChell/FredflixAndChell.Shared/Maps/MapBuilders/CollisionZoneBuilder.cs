using Microsoft.Xna.Framework;
using Nez;
using Nez.Tiled;
using static FredflixAndChell.Shared.Assets.Constants;

namespace FredflixAndChell.Shared.Maps.MapBuilders
{
    public static class CollisionZoneBuilder
    {
        public static void BuildCollisionZones(this Map map, TiledObjectGroup objectGroup)
        {
            foreach (var collisionObject in objectGroup.objectsWithName(TiledObjects.Collision))
            {
                var collidable = map.scene.createEntity("collidable" + collisionObject.id,
                    new Vector2(collisionObject.x + collisionObject.width / 2,
                        collisionObject.y + collisionObject.height / 2));
                collidable.tag = Tags.Obstacle;
                var hitbox = collidable.addComponent(new BoxCollider(collisionObject.width, collisionObject.height));
                Flags.setFlagExclusive(ref hitbox.physicsLayer, Layers.MapObstacles);
            }
        }

    }
}
