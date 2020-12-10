
using System.Collections.Generic;
using GTA;
using GTA.Math;

namespace GtaVFirstMode.utilites
{
    class VehicleUtilty
    {
        public static Ped player;
        
        private VehicleUtilty()
        {
            
        }

        public static void DrivePedBehindPlayer(Ped ped, Vehicle vehicle)
        {
            ped.Task.DriveTo(vehicle, player.GetOffsetPosition(new Vector3(0, -2, 0)), 2, 70);
        }

        public static Vehicle createVehicle()
        {
            Vehicle vehicle = World.CreateVehicle(VehicleHash.Adder, player.GetOffsetPosition(new Vector3(0, 7, 0)), player.Heading - 180);
            LoggerUtil.logInfo("Created vehcile that has hashCode: " + vehicle.GetHashCode());
            return vehicle;
        }
        
        public static Vehicle createForwardVehicle()
        {
            Vehicle vehicle = World.CreateVehicle(VehicleHash.Adder, player.GetOffsetPosition(new Vector3(0, 7, 0)), player.Heading);
            float speed = 10000;
            vehicle.Speed = speed;
            vehicle.ForwardSpeed = speed;
            vehicle.HeliBladesSpeed = speed;
            
            LoggerUtil.logInfo("Created forward vehicle!");
            vehicle.MarkAsNoLongerNeeded();
            return vehicle;
        }
        public static void makePedToLeaveVehicle(Ped ped)
        {
            if (ped.IsInVehicle())
            {
                LoggerUtil.logInfo(ped, "ped shall leave vehicle") ;
                ped.Task.LeaveVehicle();
            }

        }

        public static List<Vehicle> getClosestVehiclesToPed(Ped ped)
        {
            List<Vehicle> closesVehicles = new List<Vehicle>(World.GetNearbyVehicles(ped.Position, 300));
            if(ped.VehicleTryingToEnter != null)
            {
                closesVehicles.Add(ped.VehicleTryingToEnter);
            }
            closesVehicles.Sort(new Sorter(ped));
            LoggerUtil.logInfo(ped, "Got closest vehicle to peds, the number of closest vehicle is: " + closesVehicles.Count);
            return closesVehicles;
        }
    }
}
