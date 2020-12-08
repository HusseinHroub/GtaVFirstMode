using System;
using System.Windows.Forms;
using GTA;
using GTA.UI;
using System.Collections;
using GtaVFirstMode.utilites;
using System.Collections.Generic;
using System.Drawing;
using GTA.Native;
using GTA.Math;

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
            try
            {
                i++;
                waitingTimeForSorting++;
                if (i > 1)
                {
                    i = 0;
                    pedsAndVehicleManagment.removeVehicaleIfTakenByPlayer();
                    PedsDriveOnPlayerDrive();
                    //RemoveAllNotRelatedVehicles();
                }

                DrawPedHashCodeOnTop();
            }
            catch (Exception exc)
            {
                Notification.Show("An exception happened of type: " + exc.Message);
                Notification.Show("An exception happened of source: " + exc.Source);
                LoggerUtil.logInfo("An exception happened while running the mod: " + exc.Message);
                LoggerUtil.logInfo(exc.Source);
                LoggerUtil.logInfo(exc.StackTrace);
            }
            
            
        }

        private void DrawPedHashCodeOnTop()
        {
            if(LoggerUtil.logsEnabled)
            {
                foreach (Ped ped in pedCreationManagment.getPeds())
                {
                    DrawPedIdOnPed(ped);
                }
            }            
        }

        private static void DrawPedIdOnPed(Ped ped)
        {
            if (ped.IsOnScreen)
            {
                PointF pointF = GTA.UI.Screen.WorldToScreen(ped.AbovePosition);
                if(pointF.X != 0 && pointF.Y != 0)
                {
                    TextElement textElemntu = new TextElement("" + ped.GetHashCode(), pointF , 0.4f, Color.DarkRed);
                    textElemntu.Enabled = true;
                    textElemntu.Draw();
                }
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
                    LoggerUtil.logInfo(ped, "Ped is allowed drive, searching for vehicle to drive.");
                    ForcePedToDriveAVehicle(ped);
                }
                else
                {
                    VehicleUtilty.makePedToLeaveVehicle(ped);
                }
            }

            if (waitingTimeForSorting > 200)
            {
                waitingTimeForSorting = 0;
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
            LoggerUtil.logInfo(ped, "Starting stealing some vehicles!");
            StealClosestVehicle(ped);
            if (IsPedAlloedToAssignVehicle(ped))
            {
                LoggerUtil.logInfo(ped, "Ped is allowed to assign a vehicle...");
                pedsAndVehicleManagment.addVehicaleAndAssignItToPed(ped, (Vehicle) tryingToEnterVehclesWithPeds[ped]);
                if (ped.VehicleTryingToEnter.Driver != null)
                {
                    ped.VehicleTryingToEnter.Driver.Task.FleeFrom(ped);
                    LoggerUtil.logInfo(ped, "Driver should flee from the ped!");

                }

            }
        }

        private bool IsPedAlloedToAssignVehicle(Ped ped)
        {
            return ped.IsGettingIntoVehicle && ped.VehicleTryingToEnter != null && !player.Equals(ped.VehicleTryingToEnter.Driver);
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
                var pos = new PointF(0f, 0f);
                TextElement textElemntu = new TextElement("Hello", pos, 5f, Color.White);
                textElemntu.Enabled = true;
            }
            else if (e.KeyCode == Keys.NumPad5)
            {
                pedsAndVehicleManagment.CreateVehicale();
            }
            else if(e.KeyCode == Keys.NumPad4)
            {
                VehicleUtilty.createVehicle().LockStatus = VehicleLockStatus.CanBeBrokenInto;
            }
            else if(e.KeyCode == Keys.NumPad1)
            {
                Ped[] peds = World.GetNearbyPeds(player, 200);
                foreach(Ped ped in peds)
                {
                    ped.Delete();
                }
            }
        }

        private void StealClosestVehicle(Ped ped)
        {
            List<Vehicle> closestVehiclesToPed = VehicleUtilty.getClosestVehiclesToPed(ped);
            foreach (Vehicle vehicle in closestVehiclesToPed)
            {
                if (isPedAllowedToStealVehicle(ped, vehicle))
                {
                    if (!vehicle.Equals(ped.VehicleTryingToEnter))
                    {
                        LoggerUtil.logInfo(ped, "was allowed to steal vehciel: " + vehicle.GetHashCode());
                        AddVehicleToPedTryingToEnterVehicles(ped, vehicle);
                        ped.Task.EnterVehicle(vehicle, VehicleSeat.Driver, -1, 30, EnterVehicleFlags.AllowJacking);
                        LoggerUtil.logInfo(ped, "Is entering vehicle: " + vehicle.GetHashCode());


                    }
                    break;
                        
                }
            }

        }

        private void AddVehicleToPedTryingToEnterVehicles(Ped ped, Vehicle vehicle)
        {
            if (tryingToEnterVehclesWithPeds.ContainsKey(ped))
            {
                tryingToEnterVehclesWithPeds.Remove(tryingToEnterVehclesWithPeds[ped]);
                tryingToEnterVehclesWithPeds.Remove(ped);
                LoggerUtil.logInfo(ped, "removed vehicle from tryingToEnterVehclesWithPeds");
            }
            tryingToEnterVehclesWithPeds.Add(ped, vehicle);
            tryingToEnterVehclesWithPeds.Add(vehicle, ped);
            LoggerUtil.logInfo(ped, "Added this vehicle to tryingToEnterVehclesWithPeds: " + vehicle.GetHashCode());
        }

        private bool isPedAllowedToStealVehicle(Ped ped, Vehicle vehicle)
        {
            return vehicle != null &&
                !player.Equals(vehicle.Driver) &&
                !pedsAndVehicleManagment.getVehiclesWithPeds().Contains(vehicle) &&
                (!tryingToEnterVehclesWithPeds.Contains(vehicle) || vehicle.Equals(tryingToEnterVehclesWithPeds[ped]));
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