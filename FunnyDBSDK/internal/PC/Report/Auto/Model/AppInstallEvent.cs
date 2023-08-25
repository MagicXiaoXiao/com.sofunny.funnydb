using System.Collections.Generic;
using Newtonsoft.Json;

namespace SoFunny.FunnyDB.PC
{

    internal class AppInstallEvent : IAutoCollect
    {
        public string GetEventName()
        {
            return Constants.REPORT_EVENT_INSTALL_NAME;
        }

        public string GetReport()
        {
            Dictionary<string, object> appInstallProperties = new Dictionary<string, object>();
            long installTimeInMillSecs = FunnyDBPCInstance.instance.CalibratedTime.GetInMills();
            appInstallProperties[Constants.KEY_INSTALL_TIME] = installTimeInMillSecs;
            PlayerPfsUtils.Save(Constants.SP_KEY_FIRST_INSTALL_TIME, installTimeInMillSecs.ToString());
            return JsonConvert.SerializeObject(appInstallProperties);
        }

        public bool IsNeedReport()
        {
            return string.IsNullOrEmpty(PlayerPfsUtils.Get<string>(Constants.SP_KEY_FIRST_INSTALL_TIME));
        }
    }
}
