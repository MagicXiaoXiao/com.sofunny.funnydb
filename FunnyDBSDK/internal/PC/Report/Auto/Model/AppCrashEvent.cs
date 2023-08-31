#if UNITY_STANDALONE || UNITY_EDITOR
using System.Collections.Generic;
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
            string reportInfoStr = JsonWriterUtils.ConvertDictionaryToJson(crashEvent.GetReport());
            // 先入库，等合适的时机再上报
            FunnyDBPCInstance.Instance.ReportEvent(crashEvent.GetEventName(), reportInfoStr, sendType: ((int)DBSDK_SEND_TYPE_ENUM.DELAY));
        }

        public string GetEventName()
        {
            return Constants.REPORT_EVENT_CRASH_NAME;
        }

        public Dictionary<string, object> GetReport()
        {
            Dictionary<string, object> appCrashProperties = new Dictionary<string, object>();
            appCrashProperties[Constants.KEY_APP_CRASH_REASON] = appCrashReason;
            return appCrashProperties;
        }

        public bool IsNeedReport()
        {
            return true;
        }
    }
}
#endif