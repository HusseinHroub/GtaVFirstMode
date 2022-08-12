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
            Vector3 targetPostion = entity.Position;
            Vector3 playerPosition = player.Position;


            LoggerUtil.logInfo("playerPosition.X: " + playerPosition.X + ", targetPostion.X: " + targetPostion.X);
            LoggerUtil.logInfo("playerPosition.Y: " + playerPosition.Y + ", targetPostion.Y: " + targetPostion.Y);

            float power = 1f;
            float x = getPowerDirection(playerPosition.X, targetPostion.X, power);
            float y = getPowerDirection(playerPosition.Y, targetPostion.Y, power);
            float z = getPowerDirection(playerPosition.Z, targetPostion.Z, power);
            if (z < 0 && entity.HeightAboveGround < 1.6)
            {
                z = 0;
            }

            //entity.Velocity = new Vector3(x, y, z);
            entity.ApplyForce(new Vector3(x, y, z), Vector3.Zero, ForceType.MaxForceRot2);
            LoggerUtil.logInfo("============");

            /*LoggerUtil.logInfo("x: " + x);
            LoggerUtil.logInfo("y: " + y);
            LoggerUtil.logInfo("z: " + z);
            
          
            LoggerUtil.logInfo("Speed: " + entity.Speed);
            
            LoggerUtil.logInfo("Position: " + entity.Position);
            LoggerUtil.logInfo("LeftPosition: " + entity.LeftPosition);
            LoggerUtil.logInfo("RightPosition: " + entity.RightPosition);
            LoggerUtil.logInfo("FrontPosition: " + entity.FrontPosition);
            LoggerUtil.logInfo("RearPosition: " + entity.RearPosition);*/

        }

        private float getPowerDirection(float playerAxisValue, float targetAxisValue, float desiredPower)
        {
            var diff = playerAxisValue - targetAxisValue;
            if (diff > 0)
            {
                if (desiredPower > diff)
                    return diff;
                else
                    return desiredPower;
            }
            else if (diff < 0)
                if (desiredPower < diff)
                    return diff;
                else
                    return -desiredPower;
            else if (diff == 0)
            {
                return 0;
            }

            return diff;

        }

        public int getSizeOfEntitiesHeld()
        {
            return dict.Count;
        }
    }
}