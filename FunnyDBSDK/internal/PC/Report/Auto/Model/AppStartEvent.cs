#if UNITY_STANDALONE || UNITY_EDITOR
using System.Collections.Generic;

namespace SoFunny.FunnyDB.PC
{
    internal class AppStartEvent : IAutoCollect
    {
        private string startReason = Constants.VALUE_UNKNOWN;

        public Dictionary<string, object> GetReport()
        {
            Dictionary<string, object> appStartProperties = new Dictionary<string, object>();
            appStartProperties[Constants.KEY_APP_START_REASON] = startReason;
            return appStartProperties;
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
#endif