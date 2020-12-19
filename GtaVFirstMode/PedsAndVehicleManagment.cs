using GTA;
using GtaVFirstMode.utilites;
using System.Collections;

namespace GtaVFirstMode
{
    class PedsAndVehicleManagment
    {
        private Ped player;
        private ArrayList vehicles = new ArrayList();
        private Hashtable vehiclesWithPeds = new Hashtable();

        public PedsAndVehicleManagment(Ped player)
        {
            this.player = player;
        }
        public void CreateVehicale()
        {
            vehicles.Add(VehicleUtilty.createVehicle());
        }
        public bool assignPedToVeachleIfThereIs(Ped ped)
        {
            LoggerUtil.logInfo(ped, "Start assigning ped to vehicle: ");
            Vehicle vehicle = getEmptyVehicle();
            if (vehicle != null)
            {
                assignPedToVehicle(ped, vehicle);
                return true;
            }
            else
            {
                return false;
            }
        }
        public void assignPedToVehicle(Ped ped, Vehicle vehicle)
        {
            vehiclesWithPeds.Add(vehicle, ped);
            vehiclesWithPeds.Add(ped, vehicle);
            LoggerUtil.logInfo(ped, "Succesfully assigned ped to vehicle with hascode: " + vehicle.GetHashCode());
        }
        public Vehicle getEmptyVehicle()
        {
            foreach (Vehicle vehicle in vehicles)
            {
                if (isVehicleConsideredEmpty(vehicle))
                {
                    LoggerUtil.logInfo("Found empty vehicle with hashcode: " + vehicle.GetHashCode());
                    return vehicle;
                }
            }
            return null;
        }

        private bool isVehicleConsideredEmpty(Vehicle vehicle)
        {
            return !vehicle.Equals(player.VehicleTryingToEnter) && !player.Equals(vehicle.Driver) && !vehiclesWithPeds.Contains(vehicle);
        }

        public void deleteAllVehicales()
        {
            foreach (Vehicle vehicle in vehicles)
            {
                vehicle.Delete();
            }
            vehicles.Clear();
            vehiclesWithPeds.Clear();
        }
        public bool PedDriveIfHaveVehicle(Ped ped)
        {
            if (vehiclesWithPeds.Contains(ped))
            {
                Vehicle vehicle = (Vehicle)vehiclesWithPeds[ped];
                LoggerUtil.logInfo(ped, "Ped own vehicle: " + vehicle.GetHashCode() + " and will drive.");
                if (ped.IsInVehicle())
                {
                    LoggerUtil.logInfo(ped, "Ped is inVehicle, so EnterVehicle task is invoked, and drive to is invoked");
                    ped.Task.EnterVehicle(vehicle, VehicleSeat.Driver, -1, 30);
                    VehicleUtilty.DrivePedBehindPlayer(ped, vehicle);
                }
                else if (!vehicle.Equals(ped.VehicleTryingToEnter))
                {
                    LoggerUtil.logInfo(ped, "Ped trying to enter vehicle is null, making him to enter vehicle");
                    ped.Task.EnterVehicle(vehicle, VehicleSeat.Driver, -1, 30);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public ArrayList getVehicles()
        {
            return vehicles;
        }

        public Hashtable getVehiclesWithPeds()
        {
            return vehiclesWithPeds;
        }

        public void addVehicaleAndAssignItToPed(Ped ped, Vehicle currentVehicle)
        {
            if(currentVehicle != null)
            {
                LoggerUtil.logInfo(ped, "Addeing vehicle to ped: " + currentVehicle.GetHashCode());
                vehicles.Add(currentVehicle);
                assignPedToVehicle(ped, currentVehicle);
            }
            
        }

        public void removeVehicaleIfTakenByPlayer()
        {
            Vehicle vehicle = player.CurrentVehicle;
            if (vehicle != null && vehiclesWithPeds.Contains(vehicle))
            {
                LoggerUtil.logInfo("This vehicle: " + vehicle.GetHashCode() + " is now taken by player");
                removeRelationIfHaveOwner(vehicle);
            }
        }
        private void removeRelationIfHaveOwner(Vehicle vehicle)
        {
            Ped ownerOfThisVehicle = (Ped)vehiclesWithPeds[vehicle];
            if (ownerOfThisVehicle != null)
            {
                LoggerUtil.logInfo(ownerOfThisVehicle, "This vehicle: " + vehicle.GetHashCode() + " is removed from ped");
                vehiclesWithPeds.Remove(ownerOfThisVehicle);
                vehiclesWithPeds.Remove(vehicle);
            }
        }
        public bool isRelatedVehicle(Vehicle vehicle)
        {
            foreach (Vehicle vMyWorld in vehicles)
            {
                if (vehicle.Equals(vMyWorld))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
