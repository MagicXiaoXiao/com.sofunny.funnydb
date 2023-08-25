namespace SoFunny.FunnyDB.PC
{
    internal class AutoEventCollectManager
    {

        private static readonly AutoEventCollectManager instance = new AutoEventCollectManager();

        internal static AutoEventCollectManager Instance => instance;

        internal CrashCollectHandler _crashCollectHandler = null;
        internal ApplictionStateHandler _applictionStateHandler = null;

        private bool IsInit = false;

        private AutoEventCollectManager()
        {
        }

        internal void ReportAutoCollectEvents()
        {
            AppInstallEvent appInstallEvent = new AppInstallEvent();
            if (appInstallEvent.IsNeedReport())
            {
                FunnyDBPCInstance.instance.ReportEvent(appInstallEvent.GetEventName(), appInstallEvent.GetReport());
            }
        }

        internal void Init()
        {
            if (IsInit)
            {
                return;
            }
            IsInit = true;
            AppStartEvent startEvent = new AppStartEvent();
            if (startEvent.IsNeedReport())
            {
                FunnyDBPCInstance.instance.ReportEvent(startEvent.GetEventName(), startEvent.GetReport());
            }
            ReportAutoCollectEvents();
            _crashCollectHandler =  new CrashCollectHandler();
            _applictionStateHandler = new ApplictionStateHandler();

            _crashCollectHandler.Init();
            _applictionStateHandler.Init();
        }
    }
}