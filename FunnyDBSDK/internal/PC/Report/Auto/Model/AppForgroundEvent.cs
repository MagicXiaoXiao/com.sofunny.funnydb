
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

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
            FunnyDBPCInstance.instance.ReportEvent(e.GetEventName(), e.GetReport());
        }

        public string GetEventName()
        {
            return Constants.REPORT_EVENT_FOREGROUND_NAME;
        }

        public string GetReport()
        {
            Dictionary<string, object> appForgroundProperties = new Dictionary<string, object>();
            appForgroundProperties[Constants.KEY_APP_START_REASON] = startReason;
            appForgroundProperties[Constants.KEY_APP_START_BACKGROUND_DURATION] = backgroundDuration;
            return JsonConvert.SerializeObject(appForgroundProperties);
        }

        public bool IsNeedReport()
        {
            return true;
        }
    }
}