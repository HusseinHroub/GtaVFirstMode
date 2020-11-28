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
        }
        public Vehicle getEmptyVehicle()
        {
            foreach (Vehicle vehicle in vehicles)
            {
                if (isVehicleConsideredEmpty(vehicle))
                {
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
                if (ped.IsInVehicle())
                {
                    ped.Task.EnterVehicle(vehicle, VehicleSeat.Driver);
                    VehicleUtilty.DrivePedBehindPlayer(ped, vehicle);
                }
                else if (ped.VehicleTryingToEnter == null)
                {
                    VehicleUtilty.DrivePedBehindPlayer(ped, vehicle);
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
            vehicles.Add(currentVehicle);
            assignPedToVehicle(ped, currentVehicle);
        }

        public void removeVehicaleIfTakenByPlayer()
        {
            Vehicle vehicle = player.CurrentVehicle;
            if (vehicle != null && vehiclesWithPeds.Contains(vehicle))
            {
                removeRelationIfHaveOwner(vehicle);
            }
        }
        private void removeRelationIfHaveOwner(Vehicle vehicle)
        {
            Ped ownerOfThisVehicle = (Ped)vehiclesWithPeds[vehicle];
            if (ownerOfThisVehicle != null)
            {
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
