

using GTA;

namespace GtaVFirstMode.utilites
{

    class LoggerUtil
    {    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static bool logsEnabled = true;
        private LoggerUtil()
        {

        }

        public static void logInfo(string logMessage)
        {
            if(logsEnabled)
            {
                log.Info(logMessage);
            }
        }

        public static void logInfo(Ped ped, string logMessage)
        {
            if(logsEnabled)
            {
                log.InfoFormat("[Ped: {0}] {1}", ped.GetHashCode(), logMessage);
            }

        }
    }
}
