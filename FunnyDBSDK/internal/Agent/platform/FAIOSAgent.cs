#if Unity_IOS
using System.Runtime.InteropServices;
namespace SoFunny.FunnyDB.Bridge
{
    internal class FunnyDBIOSAgent : IFunnyDBAgent
    {

        [DllImport("__Internal", EntryPoint = "FunnyDB_InitializeSDK")]
        private static extern int initialize(string accessKeyId, string accessKeySecret, string endPoint);
        [DllImport("__Internal", EntryPoint = "FunnyDB_SetSDKStatus")]
        private static extern void setSDKStatus(int status);
        [DllImport("__Internal", EntryPoint = "FunnyDB_SetReportType")]
        private static extern void setSDKSendType(int sendType);
        [DllImport("__Internal", EntryPoint = "FunnyDB_SetUserID")]
        private static extern void setUserId(string userId);
        [DllImport("__Internal", EntryPoint = "FunnyDB_SetChannel")]
        private static extern void setChannel(string channel);
        [DllImport("__Internal", EntryPoint = "FunnyDB_SetDeviceID")]
        private static extern void setDeviceId(string deviceId);
        [DllImport("__Internal", EntryPoint = "FunnyDB_GetDeviceID")]
        private static extern string getDeviceId();
        [DllImport("__Internal", EntryPoint = "FunnyDB_SetReportInterval")]
        private static extern void setReportInterval(int interval);
        [DllImport("__Internal", EntryPoint = "FunnyDB_SetReportLimit")]
        private static extern void setReportLimit(int limit);
        [DllImport("__Internal", EntryPoint = "FunnyDB_EventUpdate")]
        private static extern void reportCustom(int customType, int operateType, string jsonStr);
        [DllImport("__Internal", EntryPoint = "FunnyDB_EventCustom")]
        private static extern void reportEvent(string eventName, string customPropert);
        [DllImport("__Internal", EntryPoint = "FunnyDB_Flush")]
        private static extern void flush();
        [DllImport("__Internal", EntryPoint = "FunnyDB_DebugLogEnable")]
        private static extern void enableDebug();
        public int Initialize(string accessKeyId, string accessKeySecret, string endPoint)
        {
            int flag = initialize(accessKeyId, accessKeySecret, endPoint);
            return flag;
        }

        public void SetSDKStatus(int status)
        {
            setSDKStatus(status);
        }

        public void SetSDKSendType(int sendType)
        {
            setSDKSendType(sendType);
        }

        public void SetUserId(string userId)
        {
            setUserId(userId);
        }

        public void SetChannel(string channel)
        {
            setChannel(channel);
        }

        public void SetDeviceId(string deviceId)
        {
            setDeviceId(deviceId);
        }

        public string GetDeviceId()
        {
            return getDeviceId();
        }

        public void ReportEvent(string eventName, string customProperty = "")
        {
            reportEvent(eventName, customProperty);
        }

        public void SetReportInterval(int interval)
        {
            setReportInterval(interval);
        }

        public void SetReportLimit(int limit)
        {
            setReportLimit(limit);
        }

        public void Flush()
        {
            flush();
        }

        public void ReportCustom(int customType, int operateType, string jsonStr)
        {
            reportCustom(customType, operateType, jsonStr);
        }

        public void EnableDebug()
        {
            enableDebug();
        }

        public void SetOAIDCertInfo(string data)
        {
            Logger.Log("Editor 不支持该方法");
        }

        public void SetOAIDCertAssetName(string name)
        {
            Logger.Log("Editor 不支持该方法");
        }

        public void ShowToast(string msg)
        {
            Logger.Log($"Toast Message - {msg}");
        }
    }
}
#endif