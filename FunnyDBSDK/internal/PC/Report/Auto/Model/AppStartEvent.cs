using System.Collections.Generic;
using Newtonsoft.Json;

namespace SoFunny.FunnyDB.PC
{
    internal class AppStartEvent : IAutoCollect
    {
        private string startReason = Constants.VALUE_UNKNOWN;

        public string GetReport()
        {
            Dictionary<string, string> appStartProperties = new Dictionary<string, string>();
            appStartProperties[Constants.KEY_APP_START_REASON] = startReason;
            return JsonConvert.SerializeObject(appStartProperties);
        }

        public string GetEventName()
        {
            return Constants.REPORT_EVENT_START_NAME;
        }

        public bool IsNeedReport()
        {
            return true;
        }

    }
}
