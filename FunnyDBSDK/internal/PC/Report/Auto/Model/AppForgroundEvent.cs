#if UNITY_STANDALONE || UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace SoFunny.FunnyDB.PC
{

    internal class AppForgroundEvent : IAutoCollect
    {
        internal static int EnterBackgroundInMills { private get;set;}
        private string startReason = Constants.VALUE_UNKNOWN;
        private int backgroundDuration;

        public static void Track()
        {
            if (EnterBackgroundInMills == 0)
            {
                return;
            }
            AppForgroundEvent e = new AppForgroundEvent();
            e.backgroundDuration = (Environment.TickCount - EnterBackgroundInMills) / 1000;
            if (!e.IsNeedReport())
            {
                return;
            }
            string reportInfoStr = JsonWriterUtils.ConvertDictionaryToJson(e.GetReport());
            FunnyDBPCInstance.Instance.ReportEvent(e.GetEventName(), reportInfoStr);
        }

        public string GetEventName()
        {
            return Constants.REPORT_EVENT_FOREGROUND_NAME;
        }

        public Dictionary<string, object> GetReport()
        {
            Dictionary<string, object> appForgroundProperties = new Dictionary<string, object>();
            appForgroundProperties[Constants.KEY_APP_START_REASON] = startReason;
            appForgroundProperties[Constants.KEY_APP_START_BACKGROUND_DURATION] = backgroundDuration;
            return appForgroundProperties;
        }

        public bool IsNeedReport()
        {
            return true;
        }
    }
}
#endif