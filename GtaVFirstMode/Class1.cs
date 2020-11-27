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
        //test
        ArrayList peds = new ArrayList();
        ArrayList vehicles = new ArrayList();
        Hashtable vehiclesWithPeds = new Hashtable();
        int i = 0;
        Ped player;
        public Main()
        {
            InitPlayer();
            KeyDown += onKeyDown;
            Tick += tick;
        }

        private void tick(object sender, EventArgs e)
        {
            ++i;
            //AttackJackTarget();
            if (i > 1)
            {
                i = 0;
                assingDriveToTaskToPeds();
            }

        }

        private void assingDriveToTaskToPeds()
        {
            foreach (Ped ped in peds)
            {

                if (ped.LastVehicle != null)
                {
                    ped.Task.EnterVehicle(ped.LastVehicle, VehicleSeat.Driver);
                    ped.Task.DriveTo(ped.LastVehicle, player.GetOffsetPosition(new Vector3(0, -5, 0)), 2, 250);
                }
            }
        }

        private void AttackJackTarget()
        {
            if (player.JackTarget != null)
            {
                foreach (Ped ped in peds)
                {
                    ped.Task.FightAgainst(player.JackTarget);
                }
            }
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
            else if (e.KeyCode == Keys.NumPad9)
            {
                clearHashTable();
                deleteAllPeds();
                deleteAllCars();
            }
            else if (e.KeyCode == Keys.NumPad6)
            {
                assingDriveToTaskToPeds();
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
            foreach (Vehicle vehicle in vehicles)
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
                //ped.Task.PerformSequence(GetVeheccleTasks(ped, vehicle));
                ped.Task.DriveTo(vehicle, player.GetOffsetPosition(new Vector3(0, 10, 0)), 5, 20);
            }
        }

        private TaskSequence GetVeheccleTasks(Ped ped, Vehicle vehicle)
        {
            TaskSequence taskSequence = new TaskSequence();
            //taskSequence.AddTask.GoTo(player.GetOffsetPosition(new Vector3(0, 20, 0)));
            //taskSequence.AddTask.GoTo(player.GetOffsetPosition(new Vector3(0, 0, 0)));

            //taskSequence.AddTask.DriveTo(vehicle, player.GetOffsetPosition(new Vector3(0, 80, 0)), 10, 20);
            taskSequence.AddTask.DriveTo(vehicle, player.GetOffsetPosition(new Vector3(0, 0, 0)), 10, 20);
            return taskSequence;
        }

        private Vehicle getEmptyCar()
        {
            foreach (Vehicle vehicle in vehicles)
            {
                if (!vehiclesWithPeds.Contains(vehicle))
                {
                    return vehicle;
                }
            }
            return null;
        }

        private void deleteAllPeds()
        {
            foreach (Ped ped in peds)
            {
                ped.Delete();
            }
            peds.Clear();
        }
    }
}