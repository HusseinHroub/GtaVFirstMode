using GTA;
using GTA.Math;
using GtaVFirstMode.utilites;

namespace GtaVFirstMode
{
    public class ToBeRemovedEntity
    {
        private readonly Entity entity;
        private int expiryTime = 200;

        public ToBeRemovedEntity(Entity entity)
        {
            this.entity = entity;
        }

        public bool isExpired()
        {
            if (expiryTime-- == 0)
            {
                return true;
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
    }
}