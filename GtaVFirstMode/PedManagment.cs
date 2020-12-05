using GTA;
using GTA.Math;
using GtaVFirstMode.utilites;
using System;
using System.Collections;

namespace GtaVFirstMode
{
    
    class PedCreationManagment
    {
        private Ped player;
        private ArrayList peds;

        public PedCreationManagment(Ped player)
        {
            this.player = player;
            peds = new ArrayList();
        }

        public Ped createPedWithPlayerGroup()
        {
            Ped ped = World.CreatePed(PedHash.Security01SMM, player.GetOffsetPosition(new Vector3(0, 7, 0)));
            peds.Add(ped);
            player.PedGroup.Add(ped, false);
            setPedCharatersitcs(ped);
            LoggerUtil.logInfo("Created ped with hashcode of: " + ped.GetHashCode());
            return ped;
        }

        private static void setPedCharatersitcs(Ped ped)
        {
            ped.NeverLeavesGroup = true;
            ped.Weapons.Give(WeaponHash.APPistol, 1000, true, true);
            ped.DrivingStyle = DrivingStyle.AvoidTrafficExtremely;
            ped.CanBeDraggedOutOfVehicle = false;
        }

        public ArrayList getPeds()
        {
            return peds;
        }

        public void deleteAllPeds()
        {
            foreach (Ped ped in peds)
            {
                ped.Delete();
            }
            peds.Clear();
        }
    }
}
