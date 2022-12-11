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
        Dictionary<int, EntityWithLastPosition> entitesToApplyForcePower;
        Dictionary<int, ToBeRemovedEntity> entitiesThatWillExpire;

        public EntityGrabber(Ped player)
        {
            this.player = player;
            entitesToApplyForcePower  = new Dictionary<int, EntityWithLastPosition>();
            entitiesThatWillExpire = new Dictionary<int, ToBeRemovedEntity>();
        }

        public void addEntity(EntityWithLastPosition entityWithLastPosition)
        {
            
            if (!entitesToApplyForcePower.ContainsKey(entityWithLastPosition.getEntity().Handle))
            {
                entitesToApplyForcePower.Add(entityWithLastPosition.getEntity().Handle, entityWithLastPosition);
            }
        }

        public void forceEntitiesToPlayerPosition()
        {
            List<EntityWithLastPosition> toBeRemovedFromSet = new List<EntityWithLastPosition>();
            foreach (EntityWithLastPosition entityWithLastPosition in entitesToApplyForcePower.Values)
            {
                applyForceToEntityIntoPlayerDirection(entityWithLastPosition.getEntity());
                if (enoughForcePower(entityWithLastPosition))
                {
                    toBeRemovedFromSet.Add(entityWithLastPosition);
                    entityWithLastPosition.getEntity().Speed = 0;
                    entityWithLastPosition.getEntity().Velocity = Vector3.Zero;
                    if (!entitiesThatWillExpire.ContainsKey(entityWithLastPosition.getEntity().Handle))
                    {
                        entitiesThatWillExpire.Add(entityWithLastPosition.getEntity().Handle, new ToBeRemovedEntity(entityWithLastPosition.getEntity()));
                        LoggerUtil.logInfo("entitiesThatWillExpire dict size: " + entitiesThatWillExpire.Count);
                    }
                }
                if (entityWithLastPosition.isFrozen())
                {
                    toBeRemovedFromSet.Add(entityWithLastPosition);
                }
            }

            foreach (var entityWithLastPosition in toBeRemovedFromSet)
            {
                entitesToApplyForcePower.Remove(entityWithLastPosition.getEntity().Handle);
            }

            removeExpiredEntities();
        }

        private void removeExpiredEntities()
        {
            List<ToBeRemovedEntity> toBeRemovedIfExpired = new List<ToBeRemovedEntity>();
            foreach (ToBeRemovedEntity removedEntity in entitiesThatWillExpire.Values)
            {
                if (removedEntity.isExpired())
                {
                    LoggerUtil.logInfo("Entitty with handle: " + removedEntity.getEntity().Handle + " is expired");
                    toBeRemovedIfExpired.Add(removedEntity);
                }
                else
                {
                    removedEntity.getEntity().Speed = 0;
                    removedEntity.getEntity().Velocity = Vector3.Zero;
                    removedEntity.getEntity().ApplyForce(new Vector3(0, 0, 0));
                }
            }
            foreach (var entity in toBeRemovedIfExpired)
            {
                entitiesThatWillExpire.Remove(entity.getEntity().Handle);
                LoggerUtil.logInfo("entitiesThatWillExpire dict size: " + entitiesThatWillExpire.Count);
            }
            
        }

        private bool enoughForcePower(EntityWithLastPosition entityWithLastPosition)
        {
            if (World.GetDistance(player.Position, entityWithLastPosition.getEntity().Position) < DISTANCE_RANGE_FOR_CLOSEST)
            {
                return true;
            }
            return false;

        }

        private void applyForceToEntityIntoPlayerDirection(Entity entity)
        {
            Vector3 targetToPlayerVector = player.Position - entity.Position;
            targetToPlayerVector.Normalize();
            targetToPlayerVector = targetToPlayerVector * 2;
            entity.ApplyForce(targetToPlayerVector, Vector3.Zero, ForceType.MaxForceRot);
            //entity.Velocity = targetToPlayerVector;

        }

        public int getSizeOfEntitiesHeld()
        {
            return entitesToApplyForcePower.Count;
        }
    }
}