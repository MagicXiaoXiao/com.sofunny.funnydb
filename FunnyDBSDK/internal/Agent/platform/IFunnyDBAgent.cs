namespace SoFunny.FunnyDB
{
    internal interface IFunnyDBAgent
    {
        void SetOAIDCertAssetName(string name);
        void SetOAIDCertInfo(string data);
        void ShowToast(string msg);
        void EnableDebug();
        void Flush();
        string GetDeviceId();
        int Initialize(string keyID, string keySecret, string endPoint);
        void ReportCustom(int customType, int operateType, string jsonStr);
        void ReportEvent(string eventName, string customProperty);
        void SetChannel(string channel);
        void SetDeviceId(string deviceID);
        void SetReportInterval(int interval);
        void SetReportLimit(int limit);
        void SetSDKSendType(int sendType);
        void SetSDKStatus(int status);
        void SetUserId(string userId);
    }
}