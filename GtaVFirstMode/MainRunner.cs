using System;
using System.Windows.Forms;
using GTA;
using GTA.UI;
using System.Collections;
using GtaVFirstMode.utilites;
using System.Collections.Generic;
using System.Drawing;
using GTA.Math;

namespace GtaVFirstMode
{
    public class Main : Script
    {
        private PedCreationManagment pedCreationManagment;
        private PedsAndVehicleManagment pedsAndVehicleManagment;
        Hashtable tryingToEnterVehclesWithPeds = new Hashtable();
        Hashtable closestVehicleCache = new Hashtable();
        private EntityGrabber entityGrabber;
        int i = 0;
        int waitingTimeForSorting = 0;
        Ped player;


        public Main()
        {
            InitPlayer();
            VehicleUtilty.player = player;
            pedCreationManagment = new PedCreationManagment(player);
            pedsAndVehicleManagment = new PedsAndVehicleManagment(player);
            entityGrabber = new EntityGrabber(player);
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
                DrawVehicleSpeedStatsPlayerDriving();
                pedDestroyJackerOnceDamaged();
            }
            catch (Exception exc)
            {
                Notification.Show("An exception happened of type: " + exc.Message);
                LoggerUtil.logInfo("An exception happened while running the mod: " + exc.Message);
                LoggerUtil.logInfo(exc.Source);
                LoggerUtil.logInfo(exc.StackTrace);
            }
            
            
        }

        private void pedDestroyJackerOnceDamaged()
        {
            if (Game.Player.TargetedEntity != null)
            {
                entityGrabber.addEntity(new EntityWithLastPosition(Game.Player.TargetedEntity));
                
            }
            entityGrabber.forceEntitiesToPlayerPosition();
            UIUtils.showNotification("size is: " + entityGrabber.getSizeOfEntitiesHeld());
        }
        
        
        private void DrawVehicleSpeedStatsPlayerDriving()
        {
            Vehicle vehicle = player.CurrentVehicle;
            if (vehicle != null)
            {
                string speedStats = String.Format("wheelSpeed: {0}, accelration: {1}, clutch: {2}, currentGear: {3}, speed: {4}, bhealth: {5}",
                    (int)vehicle.WheelSpeed,
                    vehicle.Acceleration,
                    vehicle.Clutch,
                    vehicle.CurrentGear,
                    (int)vehicle.Speed,
                    vehicle.BodyHealth );
                UIUtils.showSubTitle(speedStats);
                if (Game.IsKeyPressed(Keys.W))
                {
                    //vehicle.ForwardSpeed = vehicle.Speed + 20f;
                    vehicle.ApplyForceRelative(new Vector3(0, 1f, 0));
                    
                } 
                else if (Game.IsKeyPressed(Keys.S))
                {
                    //vehicle.ApplyForceRelative(new Vector3(0, -1.5f, 0));
                    if (vehicle.Speed > 10 && vehicle.Acceleration > -1)
                    {
                        vehicle.ForwardSpeed = 1;    
                    }

                }

                if (vehicle.BodyHealth < 1000)
                {
                    vehicle.BodyHealth = 1000;
                }
                
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
            if (IfNoVehicleToDrive(ped))
            {
                stealAndAssignOnceGettingIntoVehicle(ped);
            }

        }

        private void stealAndAssignOnceGettingIntoVehicle(Ped ped)
        {
            
            StealVhiecleAndAssignIt(ped);
            assignPedToAlloedVehicleIfHeGettingInto(ped);
        }

        private void assignPedToAlloedVehicleIfHeGettingInto(Ped ped)
        {
            if (IsPedAlloedToAssignVehicle(ped))
            {
                LoggerUtil.logInfo(ped, "Ped is allowed to assign a vehicle...");
                Vehicle vehicle = (Vehicle)tryingToEnterVehclesWithPeds[ped];
                pedsAndVehicleManagment.addVehicaleAndAssignItToPed(ped, vehicle);
                closestVehicleCache.Remove(ped);
                if (vehicle.Driver != null)
                {
                    vehicle.Driver.Delete();
                    ped.SetIntoVehicle(vehicle, VehicleSeat.Driver);
                    LoggerUtil.logInfo(ped, "Driver should leave vehicle then flee from the ped!");

                }

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
            List<Vehicle> closestVehiclesToPed = getClosestVehiclesToPed(ped);
            LoggerUtil.logInfo(ped, "Starting stealing some vehicles!");
            StealClosestVehicle(ped, closestVehiclesToPed);
        }

        private List<Vehicle> getClosestVehiclesToPed(Ped ped)
        {
            if(waitingTimeForSorting > 200)
            {
                closestVehicleCache.Remove(ped);
                closestVehicleCache.Add(ped, VehicleUtilty.getClosestVehiclesToPed(ped));
            }
            return closestVehicleCache.Contains(ped) ? (List<Vehicle>) closestVehicleCache[ped] : new List<Vehicle>();
        }

        private bool IsPedAlloedToAssignVehicle(Ped ped)
        {
            return ped.IsGettingIntoVehicle && (Vehicle)tryingToEnterVehclesWithPeds[ped] != null && !player.Equals(ped.VehicleTryingToEnter.Driver);
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
            player.HealthFloat = 10000;
            Game.Player.SetRunSpeedMultThisFrame(30);
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
                tryingToEnterVehclesWithPeds.Clear();
                closestVehicleCache.Clear();
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
            else if (e.KeyCode == Keys.NumPad2)
            {
                VehicleUtilty.createForwardVehicle();
            }
        }

        private void StealClosestVehicle(Ped ped, List<Vehicle> closestVehiclesToPed)
        {
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
            Game.Player.WantedLevel = 0;
        }

        private void CreatePedAndAssignToVehicleIfThereis()
        {
            Ped ped = pedCreationManagment.createPedWithPlayerGroup();
            pedsAndVehicleManagment.assignPedToVeachleIfThereIs(ped);

        }
        
    }
}