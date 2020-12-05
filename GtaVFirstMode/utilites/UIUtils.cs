using System;


namespace GtaVFirstMode.utilites
{
    class UIUtils
    {
        private UIUtils()
        {

        }

        public static void showSubTitle(String message)
        {
            GTA.UI.Screen.ShowSubtitle(message, 9000);
        }

        public static void showNotification(String message)
        {
            GTA.UI.Notification.Show(message);
        }
    }
}
