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
                LoggerUtil.logInfo("Adding EntityWithLastPosition.Handle " + entityWithLastPosition.getEntity().Handle);
                dict.Add(entityWithLastPosition.getEntity().Handle, entityWithLastPosition);
                LoggerUtil.logInfo("Size: " + dict.Count);
            }
        }

        public void forceEntitiesToPlayerPosition()
        {
            LoggerUtil.logInfo("Size of entites in grabber: " + getSizeOfEntitiesHeld());
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
                LoggerUtil.logInfo("Removed: ");
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
            Vector3 targetPostion = entity.Position;
            Vector3 playerPosition = player.Position;
            float power = 1f;
            float x = getPowerDirection(playerPosition.X, targetPostion.X, power);
            float y = getPowerDirection(playerPosition.Y, targetPostion.Y, power);
            float z = getPowerDirection(playerPosition.Z, targetPostion.Z, power);
            if (z < 0 && entity.HeightAboveGround < 1.6)
            {
                z = 0;
            }

            //LoggerUtil.logInfo("Z power direction for this entity: " + z);
            entity.ApplyForce(new Vector3(x, y, z), Vector3.Zero, ForceType.MaxForceRot);
        }

        private float getPowerDirection(float playerAxisValue, float targetAxisValue, float desiredPower)
        {
            var powerValue = playerAxisValue - targetAxisValue;
            if (powerValue > desiredPower)
                powerValue = desiredPower;
            else if (powerValue < -desiredPower)
                powerValue = -desiredPower;
            return powerValue;
        }

        public int getSizeOfEntitiesHeld()
        {
            return dict.Count;
        }
    }
}