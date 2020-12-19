using System.Collections;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using GtaVFirstMode.utilites;

namespace GtaVFirstMode
{
    public class EntityGrabber
    {
        private Ped player;
        private int DISTANCE_RANGE_FOR_CLOSEST = 12;
        private Hashtable entitySet;

        public EntityGrabber(Ped player)
        {
            this.player = player;
            entitySet =new Hashtable();
        }

        public void addEntity(EntityWithLastPosition entityWithLastPosition)
        {
          

            if (!entitySet.Contains(entityWithLastPosition))
            {
                LoggerUtil.logInfo("Added an item with hash code in entity: " + entityWithLastPosition.GetHashCode());
                entitySet.Add(entityWithLastPosition, 1);
            }
        }

        public void forceEntitiesToPlayerPosition()
        {
            List<EntityWithLastPosition> toBeRemovedFromSet = new List<EntityWithLastPosition>();
            foreach (EntityWithLastPosition entityWithLastPosition in entitySet.Keys)
            {
                applyForceToEntityIntoPlayerDirection(entityWithLastPosition.getEntity());
                removeFromSetIfCloseEnoughToPlayer(entityWithLastPosition, toBeRemovedFromSet);
                if (entityWithLastPosition.isFrozen())
                {
                    LoggerUtil.logInfo("Entity is frozen there fore it will be removed :)");
                    toBeRemovedFromSet.Add(entityWithLastPosition);
                }
                
            }

            foreach (var entityWithLastPosition in toBeRemovedFromSet)
            {
                entitySet.Remove(entityWithLastPosition);
            }

        }

        private void removeFromSetIfCloseEnoughToPlayer(EntityWithLastPosition entityWithLastPosition,
            List<EntityWithLastPosition> toBeRemovedFromSet)
        {
            if (World.GetDistance(player.Position, entityWithLastPosition.getEntity().Position) <
                DISTANCE_RANGE_FOR_CLOSEST)
            {
                entityWithLastPosition.getEntity().Speed = 0;
                toBeRemovedFromSet.Add(entityWithLastPosition);
                LoggerUtil.logInfo("Entity is close enough to palyer so it will be removed :)");
            }
        }

        private void applyForceToEntityIntoPlayerDirection(Entity entity)
        {
            Vector3 targetPostion = entity.Position;
            Vector3 playerPosition = player.Position;
            float power = 1f;
            float x = getPowerDirection(playerPosition.X , targetPostion.X, power);
            float y =  getPowerDirection(playerPosition.Y , targetPostion.Y, power);
            float z =  getPowerDirection(playerPosition.Z , targetPostion.Z, power);
                
            entity.ApplyForce(new Vector3(x, y, z), Vector3.Zero, ForceType.MaxForceRot);
      
        }
        
        private float getPowerDirection(float playerAxisValue, float targetAxisValue, float desiredPower)
        {
            var subResult = playerAxisValue - targetAxisValue;
            if ((int)subResult > 0)
                return desiredPower;
            if ((int)subResult < 0)
                return -desiredPower;
            return  0;
        }

        public int getSizeOfEntitiesHeld()
        {
            return entitySet.Count;
        }
    }
}