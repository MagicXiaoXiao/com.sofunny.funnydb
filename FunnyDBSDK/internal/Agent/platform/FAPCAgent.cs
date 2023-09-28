#if UNITY_STANDALONE || UNITY_EDITOR
using SoFunny.FunnyDB.PC;
using UnityEngine;
using static SoFunny.FunnyDB.PC.Constants;

namespace SoFunny.FunnyDB.Bridge
{
    internal class FunnyDBPCAgent: IFunnyDBAgent
    {
        public int Initialize(string accessKeyId, string accessKeySecret, string endPoint)
        {
            new GameObject("FunnyDBPCInstance", typeof(FunnyDBPCInstance));
            FunnyDBPCInstance.instance.Initialize(accessKeyId, accessKeySecret, endPoint, (int)ReportChannel.ChannelTypePrj);
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
            FunnyDBPCInstance.instance.SetSDKStatus(status);
        }

        public void SetSDKSendType(int sendType)
        {
            FunnyDBPCInstance.instance.SetSDKSendType(sendType);
        }

        public void SetUserId(string userId)
        {
            FunnyDBPCInstance.instance.SetUserId(userId);
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
            FunnyDBPCInstance.instance.ReportEvent(eventName, customProperty);
        }

        public void SetReportInterval(int pingNum)
        {
            FunnyDBPCInstance.instance.SetReportInterval(pingNum);
        }

        public void SetReportLimit(int limit)
        {
            FunnyDBPCInstance.instance.SetReportLimit(limit);
        }

        public void Flush()
        {
            FunnyDBPCInstance.instance.Flush();
        }

        public void ReportCustom(int customType, int operateType, string jsonStr)
        {
            FunnyDBPCInstance.instance.ReportCustom(customType, operateType, jsonStr);
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