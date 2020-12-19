using GTA;
using GTA.Math;
using GtaVFirstMode.utilites;

namespace GtaVFirstMode
{
    public class EntityWithLastPosition
    {
        private readonly Entity entity;
        private Vector3 lastKnownPosition;
        private int attemptsToReCheck = 200;

        public EntityWithLastPosition(Entity entity)
        {
            this.entity = entity;
            updateLastPosition();
        }

        public bool isFrozen()
        {
            if (attemptsToReCheck-- < 0)
            {
                attemptsToReCheck = 200;
                bool isFrozen = lastKnownPosition.Equals(entity.Position);
                updateLastPosition();
                LoggerUtil.logInfo("is frzeon: " + isFrozen);
                return isFrozen;
            }
            return false;
        }

        public Entity getEntity()
        {
            return entity;
        }

        public override int GetHashCode()
        {
            return entity.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return entity.Equals(obj);
        }

        public void updateLastPosition()
        {
            this.lastKnownPosition = entity.Position;
        }
    }
}