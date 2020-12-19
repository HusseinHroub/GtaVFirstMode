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
                entityWithLastPosition.updateLastPosition();
                applyForceToEntityIntoPlayerDirection(entityWithLastPosition.getEntity());
                removeFromSetIfCloseEnoughToPlayer(entityWithLastPosition, toBeRemovedFromSet);
                if (entityWithLastPosition.isFrozen())
                {
                    toBeRemovedFromSet.Add(entityWithLastPosition);
                }
                
            }

            foreach (var entityWithLastPosition in toBeRemovedFromSet)
            {
                LoggerUtil.logInfo("Hashcode of entity trynna to remove: "+ entityWithLastPosition.GetHashCode());
                LoggerUtil.logInfo("Size before removing: "+ getSizeOfEntitiesHeld());
                entitySet.Remove(entityWithLastPosition);
                LoggerUtil.logInfo("set size after removing: " + getSizeOfEntitiesHeld());
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
        
        private float getPowerDirection(float playerDirction, float targetDirection, float desiredPower)
        {
            var subResult = playerDirction - targetDirection;
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