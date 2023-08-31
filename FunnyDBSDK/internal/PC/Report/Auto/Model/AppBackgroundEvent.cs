#if UNITY_STANDALONE || UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace SoFunny.FunnyDB.PC
{

    internal class AppBackgroundEvent : IAutoCollect
    {
        internal static int EnterForgroundInMills { private get; set; }

        private int duration;

        internal static void Track()
        {
            if (EnterForgroundInMills == 0)
            {
                return;
            }
            AppBackgroundEvent e = new AppBackgroundEvent();
            if (e.IsNeedReport())
            {
                e.duration = (Environment.TickCount - EnterForgroundInMills) / 1000;
                EnterForgroundInMills = 0;
                string reportInfoStr = JsonWriterUtils.ConvertDictionaryToJson(e.GetReport());
                FunnyDBPCInstance.Instance.ReportEvent(e.GetEventName(), reportInfoStr);
            }
        }

        public string GetEventName()
        {
            return Constants.REPORT_EVENT_BACKGROUND_NAME;
        }

        public Dictionary<string, object> GetReport()
        {
            Dictionary<string, object> appEndProperties = new Dictionary<string, object>();
            appEndProperties[Constants.KEY_APP_END_DURATION] = duration;
            return appEndProperties;
        }

        public bool IsNeedReport()
        {
            return true;
        }
    }
}
#endif