using System.Collections.Generic;
using Newtonsoft.Json;
using static SoFunny.FunnyDB.PC.EnumConstants;

namespace SoFunny.FunnyDB.PC
{

    internal class AppCrashEvent : IAutoCollect
    {
        private string appCrashReason;

        public static void Track(string reportReason)
        {
            AppCrashEvent crashEvent = new AppCrashEvent();
            if (!crashEvent.IsNeedReport())
            {
                return;
            }
            crashEvent.appCrashReason = reportReason;
            // 先入库，等合适的时机再上报
            FunnyDBPCInstance.instance.ReportEvent(crashEvent.GetEventName(), crashEvent.GetReport(), sendType: ((int)DBSDK_SEND_TYPE_ENUM.DELAY));
        }

        public string GetEventName()
        {
            return Constants.REPORT_EVENT_CRASH_NAME;
        }

        public string GetReport()
        {
            Dictionary<string, string> appCrashProperties = new Dictionary<string, string>();
            appCrashProperties[Constants.KEY_APP_CRASH_REASON] = appCrashReason;
            return JsonConvert.SerializeObject(appCrashProperties);
        }

        public bool IsNeedReport()
        {
            return true;
        }
    }
}
