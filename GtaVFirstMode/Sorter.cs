using GTA;
using System.Collections.Generic;

namespace GtaVFirstMode
{
    class Sorter : IComparer<Vehicle>
    {
        private Ped ped;
        public Sorter(Ped ped)
        {
            this.ped = ped;
        }
        public int Compare(Vehicle x, Vehicle y)
        {
            float distanceFromX = World.GetDistance(ped.Position, x.Position);
            float distanceFromY = World.GetDistance(ped.Position, y.Position);
            return Comparer<float>.Default.Compare(distanceFromX, distanceFromY);
        }
    }
}
