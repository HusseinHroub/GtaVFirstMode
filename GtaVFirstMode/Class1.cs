using System;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using GTA.Math;
using System.Collections;

namespace TestMode
{
    public class Main : Script
    {
        ArrayList peds = new ArrayList();
        ArrayList vehicles = new ArrayList();
        Hashtable vehiclesWithPeds = new Hashtable();
        Ped player;
        public Main()
        {
            InitPlayer();
            KeyDown += onKeyDown;
            Tick += tick;
        }

        private void tick(object sender, EventArgs e)
        {
                PedsDriveOnPlayerDrive();
            
        }

        private void PedsDriveOnPlayerDrive()
        {
            foreach (Ped ped in peds)
            {
                
                if (player.VehicleTryingToEnter != null || player.IsInVehicle())
                {
                    PedDriveIfHaveVehicle(ped);
                }
                else
                {
                    PedLeaveVehicle(ped);
                }
            }
        }

        private static void PedLeaveVehicle(Ped ped)
        {
            if (ped.IsInVehicle())
            {
                ped.Task.LeaveVehicle();
            }
        }

        private void PedDriveIfHaveVehicle(Ped ped)
        {
            if (vehiclesWithPeds.Contains(ped))
            {
                Vehicle vehicle = (Vehicle)vehiclesWithPeds[ped];
                DrivePedBehindPlayer(ped, vehicle);
            }
        }

        private void DrivePedBehindPlayer(Ped ped, Vehicle vehicle)
        {
            ped.Task.DriveTo(vehicle, player.GetOffsetPosition(new Vector3(0, -5, 0)), 2, 250);
        }

        private void InitPlayer()
        {
            player = Game.Player.Character;
            player.PedGroup.Formation = Formation.Line;
        }

        private void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.NumPad8)
            {
                InitSetup();
                CreatePed();
            }
            else if(e.KeyCode == Keys.NumPad9)
            {
                clearHashTable();
                deleteAllPeds();
                deleteAllCars();
            }
            else if (e.KeyCode == Keys.NumPad5)
            {
                CreateCar();
            }
        }

        private void clearHashTable()
        {
            vehiclesWithPeds.Clear();
        }

        private void CreateCar()
        {
            vehicles.Add(World.CreateVehicle(VehicleHash.Adder, player.GetOffsetPosition(new Vector3(0, 7, 0))));
        }

        private void deleteAllCars()
        {
            foreach(Vehicle vehicle in vehicles)
            {
                vehicle.Delete();
            }
            vehicles.Clear();
        }
        private void InitSetup()
        {
            Game.Player.SetRunSpeedMultThisFrame(30);
            Game.Player.WantedLevel = 0;
        }

        private void CreatePed()
        {
            Ped ped = World.CreatePed(PedHash.Security01SMM, player.GetOffsetPosition(new Vector3(0, 7, 0)));
            peds.Add(ped);
            player.PedGroup.Add(ped, false);
            ped.NeverLeavesGroup = true;
            ped.Weapons.Give(WeaponHash.APPistol, 1000, true, true);
            ped.DrivingStyle = DrivingStyle.AvoidTrafficExtremely;
            assignPedToVeachleIfThereIs(ped);

        }

        private void assignPedToVeachleIfThereIs(Ped ped)
        {
            Vehicle vehicle = getEmptyCar();
            if (vehicle != null)
            {
                vehiclesWithPeds.Add(vehicle, ped);
                vehiclesWithPeds.Add(ped, vehicle);
            }
        }
        private Vehicle getEmptyCar()
        {
            foreach(Vehicle vehicle in vehicles)
            {
               if(!vehiclesWithPeds.Contains(vehicle))
                {
                    return vehicle;
                }
            }
            return null;
        }

        private void deleteAllPeds()
        {
            foreach(Ped ped in peds)
            {
                ped.Delete();
            }
            peds.Clear();
        }
    }
}