#if UNITY_STANDALONE || UNITY_EDITOR
using System.Collections.Generic;

namespace SoFunny.FunnyDB.PC
{

    internal class AppInstallEvent : IAutoCollect
    {
        public string GetEventName()
        {
            return Constants.REPORT_EVENT_INSTALL_NAME;
        }

        public Dictionary<string, object> GetReport()
        {
            Dictionary<string, object> appInstallProperties = new Dictionary<string, object>();
            long installTimeInMillSecs = FunnyDBPCInstance.Instance.CalibratedTime.GetInMills();
            appInstallProperties[Constants.KEY_INSTALL_TIME] = installTimeInMillSecs;
            PlayerPfsUtils.Save(Constants.SP_KEY_FIRST_INSTALL_TIME, installTimeInMillSecs.ToString());
            return appInstallProperties;
        }

        public bool IsNeedReport()
        {
            return string.IsNullOrEmpty(PlayerPfsUtils.Get<string>(Constants.SP_KEY_FIRST_INSTALL_TIME));
        }
    }
}
#endif