using System;
using System.Collections;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using GtaVFirstMode.utilites;
using log4net.Repository.Hierarchy;

namespace GtaVFirstMode
{
    public class EntityGrabber
    {
        private Ped player;
        private int DISTANCE_RANGE_FOR_CLOSEST = 12;
        Dictionary<int, EntityWithLastPosition> dict;

        public EntityGrabber(Ped player)
        {
            this.player = player;
            dict  = new Dictionary<int, EntityWithLastPosition>();
        }

        public void addEntity(EntityWithLastPosition entityWithLastPosition)
        {
            
            if (!dict.ContainsKey(entityWithLastPosition.getEntity().Handle))
            {
                dict.Add(entityWithLastPosition.getEntity().Handle, entityWithLastPosition);
            }
        }

        public void forceEntitiesToPlayerPosition()
        {
            List<EntityWithLastPosition> toBeRemovedFromSet = new List<EntityWithLastPosition>();
            foreach (EntityWithLastPosition entityWithLastPosition in dict.Values)
            {
                applyForceToEntityIntoPlayerDirection(entityWithLastPosition.getEntity());
                removeFromSetIfCloseEnoughToPlayer(entityWithLastPosition, toBeRemovedFromSet);
                if (entityWithLastPosition.isFrozen())
                {
                    toBeRemovedFromSet.Add(entityWithLastPosition);
                }
            }

            foreach (var entityWithLastPosition in toBeRemovedFromSet)
            {
                dict.Remove(entityWithLastPosition.getEntity().Handle);
            }
        }

        private void removeFromSetIfCloseEnoughToPlayer(EntityWithLastPosition entityWithLastPosition,
            List<EntityWithLastPosition> toBeRemovedFromSet)
        {
            if (World.GetDistance(player.Position, entityWithLastPosition.getEntity().Position) <
                DISTANCE_RANGE_FOR_CLOSEST)
            {
                entityWithLastPosition.getEntity().Speed = 0;
                entityWithLastPosition.getEntity().Velocity = Vector3.Zero;

                toBeRemovedFromSet.Add(entityWithLastPosition);
            }
        }

        private void applyForceToEntityIntoPlayerDirection(Entity entity)
        {
            Vector3 targetToPlayerVector = player.Position - entity.Position;
            targetToPlayerVector.Normalize();
            targetToPlayerVector = targetToPlayerVector * 100;
            //entity.ApplyForce(targetToPlayerVector, Vector3.Zero, ForceType.MinForce2);
            entity.Velocity = targetToPlayerVector;

        }

        public int getSizeOfEntitiesHeld()
        {
            return dict.Count;
        }
    }
}