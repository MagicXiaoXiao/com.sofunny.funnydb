#if UNITY_STANDALONE || UNITY_EDITOR
using SoFunny.FunnyDB.PC;
using UnityEngine;
using static SoFunny.FunnyDB.PC.Constants;

namespace SoFunny.FunnyDB.Bridge
{
    internal class FunnyDBPCAgent: IFunnyDBAgent
    {
        internal FunnyDBPCAgent()
        {
            new GameObject("FunnyDBPCInstance", typeof(FunnyDBPCInstance));
        }

        public int Initialize(string accessKeyId, string accessKeySecret, string endPoint)
        {
            FunnyDBPCInstance.Instance.Initialize(accessKeyId, accessKeySecret, endPoint, (int)ReportChannel.ChannelTypePrj);
            return 1;
        }

        public void SetOAIDCertInfo(string data)
        {
            Logger.Log("Editor 不支持该方法");
        }

        public void SetOAIDCertAssetName(string name)
        {
            Logger.Log("Editor 不支持该方法");
        }

        public void SetSDKStatus(int status)
        {
            FunnyDBPCInstance.Instance.SetSDKStatus(status);
        }

        public void SetSDKSendType(int sendType)
        {
            FunnyDBPCInstance.Instance.SetSDKSendType(sendType);
        }

        public void SetUserId(string userId)
        {
            FunnyDBPCInstance.Instance.SetUserId(userId);
        }

        public void SetChannel(string channel)
        {
            FunnyDBPCInstance.SetChannel(channel);
        }

        public void SetDeviceId(string deviceId)
        {
            FunnyDBPCInstance.SetDeviceId(deviceId);
        }

        public string GetDeviceId()
        {
            return FunnyDBPCInstance.GetDeviceId();
        }

        public void ReportEvent(string eventName, string customProperty = "")
        {
            FunnyDBPCInstance.Instance.ReportEvent(eventName, customProperty);
        }

        public void SetReportInterval(int pingNum)
        {
            FunnyDBPCInstance.Instance.SetReportInterval(pingNum);
        }

        public void SetReportLimit(int limit)
        {
            FunnyDBPCInstance.Instance.SetReportLimit(limit);
        }

        public void Flush()
        {
            FunnyDBPCInstance.Instance.Flush();
        }

        public void ReportCustom(int customType, int operateType, string jsonStr)
        {
            FunnyDBPCInstance.Instance.ReportCustom(customType, operateType, jsonStr);
        }

        public void EnableDebug()
        {
            
        }

        public void ShowToast(string msg)
        {
            Logger.LogVerbose($"Toast Message - {msg}");
        }
    }
}
#endif