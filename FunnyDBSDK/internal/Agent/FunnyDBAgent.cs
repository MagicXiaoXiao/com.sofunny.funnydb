using SoFunny.FunnyDB.PC;
using UnityEngine;
using static SoFunny.FunnyDB.PC.EnumConstants;

/// <summary>
/// FunnyDB Agent
/// </summary>
namespace SoFunny.FunnyDB.Bridge
{
      internal sealed partial class FunnyDBAgent {
        private const string sFunnyVersion = "0.1.0";
        private static bool sIsInit = false;
        
        private static readonly object lockObject = new object();
#if UNITY_ANDROID && !UNITY_EDITOR
        private static IFunnyDBAgent _iFunnyDBAgent = new FunnyDBAndroidAgent();
#elif UNITY_IOS && !UNITY_EDITOR
        private static IFunnyDBAgent _iFunnyDBAgent = new FunnyDBIOSAgent();
#elif UNITY_STANDALONE || UNITY_EDITOR
        private static IFunnyDBAgent _iFunnyDBAgent = new FunnyDBPCAgent();
#endif
        /// <summary> 
        /// FunnyDB Initialize
        /// </summary>
        internal static void Initialize(FunnyDBConfig config) {
            lock (lockObject)
            {
#if ENABLE_FUNNYDB_DEBUG || ENABLE_FUNNYDB_DEBUG_LOTS_LOGS
                EnableDebug();
                Logger.LogVerbose("EnableDebug");
#endif
                if (sIsInit)
                {
                    Logger.LogError("FunnyDB was Inited, You can't initialize more than once");
                    return;
                }
                if (string.IsNullOrEmpty(config.keyID))
                {
                    Logger.LogError("access key id was empty");
                    return;
                }
                if (string.IsNullOrEmpty(config.keySecret))
                {
                    Logger.LogError("access secret id was empty");
                    return;
                }

                // 设置设备号
                if (!string.IsNullOrEmpty(config.deviceID))
                {
                    _iFunnyDBAgent.SetDeviceId(config.deviceID);
                }
                // 设置渠道
                if (!string.IsNullOrEmpty(config.channel))
                {
                    _iFunnyDBAgent.SetChannel(config.channel);
                }

                // Android 设置 OAID
                if (config.oaidEnable)
                {
                    switch (config.oaidType)
                    {
                        case FDB_OAID_TYPE.CertData:
                            SetOAIDFileData(config.oaidData);
                            break;
                        case FDB_OAID_TYPE.CertFileName:
                            SetOAIDFileName(config.oaidData);
                            break;
                        default: break;
                    }
                }

                // 调用初始化方法
                _iFunnyDBAgent.Initialize(config.keyID, config.keySecret, config.endPoint);
                sIsInit = true;
            }
        }

        /// <summary>
        /// Set SDK Status
        /// </summary>
        /// <param name="status"></param>
        internal static void SetSDKStatus(int status) {
            if (!sIsInit) {
                return;
            }
            _iFunnyDBAgent.SetSDKStatus(status);
        }

        /// <summary>
        /// Set SDK SendType
        /// </summary>
        /// <param name="status"></param>
        internal static void SetSDKSendType(int sendType) {
            if (!sIsInit) {
                return;
            }
            _iFunnyDBAgent.SetSDKSendType(sendType);
        }

        /// <summary>
        /// Set UserId
        /// </summary>
        /// <param name="userId"></param>
        internal static void SetUserId(string userId) {
            _iFunnyDBAgent.SetUserId(userId);
        }

        /// <summary>
        /// Set Channel
        /// </summary>
        /// <param name="channel"></param>
        internal static void SetChannel(string channel) {
            _iFunnyDBAgent.SetChannel(channel);
        }

        /// <summary>
        /// Set DeviceId
        /// </summary>
        /// <param name="deviceId"></param>
        internal static void SetDeviceId(string deviceId) {
            _iFunnyDBAgent.SetDeviceId(deviceId);
        }

        internal static string GetDeviceId() {
            return _iFunnyDBAgent.GetDeviceId();
        }

        /// <summary>
        /// Report Interval
        /// </summary>
        /// <param name="interval">interval</param> // ms
        internal static void SetReportInterval(int interval) {
            if (!sIsInit) {
                return;
            }
            _iFunnyDBAgent.SetReportInterval(interval);
        }

        /// <summary>
        /// Report Limit
        /// </summary>
        /// <param name="frameRate">limit</param>
        internal static void SetReportLimit(int limit) {
            if (!sIsInit) {
                return;
            }
            _iFunnyDBAgent.SetReportLimit(limit);
        }

        internal static void ReportCustom(int customType, int operateType, string jsonStr) {
            if (!sIsInit) {
                return;
            }
            _iFunnyDBAgent.ReportCustom(customType, operateType, jsonStr);
        }

        /// <summary>
        /// Report Event
        /// </summary>
        /// <param name="eventName">event Name</param>
        /// <param name="customProperty">custom Property</param>
        internal static void ReportEvent(string eventName, string customProperty = "") {
            if (!sIsInit) {
                return;
            }
            _iFunnyDBAgent.ReportEvent(eventName, customProperty);
        }

        /// </summary>
        /// Flush
        /// </summary>
        internal static void Flush() {
            if (!sIsInit) {
                return;
            }
            _iFunnyDBAgent.Flush();
        }

        internal static void EnableDebug() {
            _iFunnyDBAgent.EnableDebug();
        }

        internal static void SetOAIDFileData(string data) {
            _iFunnyDBAgent.SetOAIDCertInfo(data);
        }

        internal static void SetOAIDFileName(string name) {
            _iFunnyDBAgent.SetOAIDCertAssetName(name);
        }

        internal static void ShowToast(string msg) {
            _iFunnyDBAgent.ShowToast(msg);
        }
    }
}
