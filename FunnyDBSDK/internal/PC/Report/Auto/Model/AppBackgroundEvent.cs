
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

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
                e.duration = Environment.TickCount - EnterForgroundInMills;
                EnterForgroundInMills = 0;
                FunnyDBPCInstance.instance.ReportEvent(e.GetEventName(), e.GetReport());
            }
        }

        public string GetEventName()
        {
            return Constants.REPORT_EVENT_BACKGROUND_NAME;
        }

        public string GetReport()
        {
            Dictionary<string, object> appEndProperties = new Dictionary<string, object>();
            appEndProperties[Constants.KEY_APP_END_DURATION] = duration;
            return JsonConvert.SerializeObject(appEndProperties);
        }

        public bool IsNeedReport()
        {
            return true;
        }
    }
}