﻿using System;
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
            return World.CreateVehicle(VehicleHash.Adder, player.GetOffsetPosition(new Vector3(0, 7, 0)), player.Heading - 180);
        }
        public static void makePedToLeaveVehicle(Ped ped)
        {
            if (ped.IsInVehicle())
            {
                ped.Task.LeaveVehicle();
            }

        }

        public static Vehicle[] getClosestVehiclesToPed(Ped ped)
        {
            Vehicle[] closesVehicles = World.GetNearbyVehicles(ped.Position, 300);
            Array.Sort(closesVehicles, new Sorter(ped));
            return closesVehicles;
        }
    }
}