﻿using System;
using System.Windows.Forms;
using GTA;
using System.Collections;
using GtaVFirstMode.utilites;

namespace GtaVFirstMode
{
    public class Main : Script
    {
        private PedCreationManagment pedCreationManagment;
        private PedsAndVehicleManagment pedsAndVehicleManagment;
        Hashtable tryingToEnterVehclesWithPeds = new Hashtable();
        int i = 0;
        int waitingTimeForSorting = 0;
        Ped player;
        public Main()
        {
            InitPlayer();
            VehicleUtilty.player = player;
            pedCreationManagment = new PedCreationManagment(player);
            pedsAndVehicleManagment = new PedsAndVehicleManagment(player);
            KeyDown += onKeyDown;
            Tick += tick;
        }

        private void tick(object sender, EventArgs e)
        {
            i++;
            waitingTimeForSorting++;
            if ( i > 1)
            {
                i = 0;
                pedsAndVehicleManagment.removeVehicaleIfTakenByPlayer();
                PedsDriveOnPlayerDrive();
                //RemoveAllNotRelatedVehicles();
            }


        }
        private void RemoveAllNotRelatedVehicles()
        {
            foreach (Vehicle v in World.GetAllVehicles())
            {
                if (!pedsAndVehicleManagment.isRelatedVehicle(v))
                {
                    v.Delete();

                }
            }
        }

        private void PedsDriveOnPlayerDrive()
        {
            foreach (Ped ped in pedCreationManagment.getPeds())
            {
                if(IfAllowedToDrive(ped))
                {
                    ForcePedToDriveAVehicle(ped);
                }
                else
                {
                    VehicleUtilty.makePedToLeaveVehicle(ped);
                }

                if (waitingTimeForSorting > 200)
                {
                    waitingTimeForSorting = 0;
                }
            }
        }

        private void ForcePedToDriveAVehicle(Ped ped)
        {
            if (IfNoVehicleToDrive(ped) && waitingTimeForSorting > 200)
            {
                StealVhiecleAndAssignIt(ped);
            }
        }

        private bool IfNoVehicleToDrive(Ped ped)
        {
            return !pedsAndVehicleManagment.PedDriveIfHaveVehicle(ped) &&
                 !pedsAndVehicleManagment.assignPedToVeachleIfThereIs(ped);
        }

        private bool IfAllowedToDrive(Ped ped)
        {
            return player.VehicleTryingToEnter != null ||
                player.IsInVehicle() ||
                (distanceToPlayer(ped) > 25 && distanceStraight(ped) > 25);
        }

        private void StealVhiecleAndAssignIt(Ped ped)
        {
            StealClosestVehicle(ped);
            if (ped.IsInVehicle())
            {
                pedsAndVehicleManagment.addVehicaleAndAssignItToPed(ped, ped.CurrentVehicle);
                
            }
        }
        
        private float distanceStraight(Ped ped)
        {
            return World.GetDistance(ped.Position, player.Position);
        }

        private float distanceToPlayer(Ped ped)
        {
            return World.CalculateTravelDistance(ped.Position, player.Position);
        }

        private void InitPlayer()
        {
            player = Game.Player.Character;
            player.PedGroup.Formation = Formation.Default;
        }

        private void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.NumPad8)
            {
                InitSetup();
                CreatePedAndAssignToVehicleIfThereis();
            }
            else if (e.KeyCode == Keys.NumPad9)
            {
                pedCreationManagment.deleteAllPeds();
                pedsAndVehicleManagment.deleteAllVehicales();
            }
            else if(e.KeyCode == Keys.NumPad6)
            {
                foreach(Ped ped in pedCreationManagment.getPeds())
                {
                    StealClosestVehicle(ped);
                }
            }
            else if (e.KeyCode == Keys.NumPad5)
            {
                pedsAndVehicleManagment.CreateVehicale();
            }
        }

        //this is performance killed cuz of sorting.
        private void StealClosestVehicle(Ped ped)
        {
            Vehicle[] closestVehiclesToPed = VehicleUtilty.getClosestVehiclesToPed(ped);
            foreach (Vehicle vehicle in closestVehiclesToPed)
            {
                if (isPedAllowedToStealVehicle(ped, vehicle))
                {
                    AddVehicleToPedTryingToEnterVehicles(ped, vehicle);
                    ped.Task.EnterVehicle(vehicle, VehicleSeat.Driver, -1, 30, EnterVehicleFlags.AllowJacking);
                    break;
                }
            }

        }

        private void AddVehicleToPedTryingToEnterVehicles(Ped ped, Vehicle vehicle)
        {
            if (tryingToEnterVehclesWithPeds.ContainsKey(ped))
            {
                tryingToEnterVehclesWithPeds.Remove(ped);
            }
            tryingToEnterVehclesWithPeds.Add(ped, vehicle);
        }

        private bool isPedAllowedToStealVehicle(Ped ped, Vehicle vehicle)
        {
            return vehicle != null &&
                !player.Equals(vehicle.Driver) &&
                !pedsAndVehicleManagment.getVehiclesWithPeds().Contains(vehicle) &&
                !vehicle.Equals(tryingToEnterVehclesWithPeds[ped]);
        }

        private void InitSetup()
        {
            Game.Player.SetRunSpeedMultThisFrame(30);
            Game.Player.WantedLevel = 0;
        }

        private void CreatePedAndAssignToVehicleIfThereis()
        {
            Ped ped = pedCreationManagment.createPedWithPlayerGroup();
            pedsAndVehicleManagment.assignPedToVeachleIfThereIs(ped);

        }
        
    }
}