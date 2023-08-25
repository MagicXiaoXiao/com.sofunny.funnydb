using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;
using UnityEngine;
using static SoFunny.FunnyDB.PC.EnumConstants;
/// <summary>
/// Unity Editor Logic(Simple)
/// </summary>
namespace SoFunny.FunnyDB.PC
{
    internal sealed partial class FunnyDBPCInstance : MonoBehaviour
    {
        internal static FunnyDBPCInstance instance;        
        internal readonly Hashtable AccessKeyHashTable = new Hashtable();
        internal ICalibratedTime CalibratedTime = null;

        private void Awake()
        {
            instance = this;
            originalContext = SynchronizationContext.Current;
            mainThreadId = Thread.CurrentThread.ManagedThreadId;
            Logger.Log("FunnyDBPC Instance Awake");
            DontDestroyOnLoad(gameObject);
            _waitForSeconds = new WaitForSeconds(_lastReportInterval);
        }

        private void Start()
        {
            StartCoroutine(LoopReportTimer());
        }

        private IEnumerator LoopReportTimer()
        {
            while (true)
            {
                if(_lastReportInterval != ReportSettings.ReportInterval)
                {
                    _lastReportInterval = ReportSettings.ReportInterval;
                    _waitForSeconds = new WaitForSeconds(_lastReportInterval);
                    Logger.Log("Report Interval changed, Take Effect Now! ");
                }
                yield return _waitForSeconds;
                Logger.LogVerbose("Loop Timer: " + Time.time);
                AutoReportTimer.Instance.DoCheckDataSource();
            }
        }

        internal void Initialize(string accessKeyId, string accessKeySecret, string endPoint, int reportChannelType = (int)Constants.ReportChannel.ChannelTypePrj)
        {

            AccessKeyHashTable[reportChannelType] = new AccessInfo(accessKeyId, accessKeySecret, endPoint);
            if (!_isInit)
            {
                // Do first Init
                _isInit = true;
                CalibratedTime = new CalibratedTimeWithNTP();
                DataSource.Init(accessKeyId);
                AutoEventCollectManager.Instance.Init();
                AutoReportTimer.Instance.Init();
       
            }
            Logger.LogVerbose("FunnyDB Init true");
            if (reportChannelType == ((int)Constants.ReportChannel.ChannelTypePrj))
            {
                ReportEvent("#device_login", "", reportChannelType);
            }
        }

        internal void SetSDKStatus(int status)
        {
            if (!_isInit)
            {
                return;
            }
            Logger.Log("setSDKStatus: " + status);
            if (status > ((int)DBSDK_STATUS_ENUM.ONLY_COLLECT) || status < ((int)DBSDK_STATUS_ENUM.DEFAULT))
            {
                Logger.LogWarning("status illegal");
                return;
            }
            ReportSettings.SdkStatus = status;
        }

        internal void SetSDKSendType(int sendType)
        {
            if (!_isInit)
            {
                return;
            }
            Logger.Log("setSDKSendType: " + sendType);
            if (sendType > ((int)DBSDK_SEND_TYPE_ENUM.DELAY) || sendType < ((int)DBSDK_SEND_TYPE_ENUM.NOW))
            {
                Logger.LogWarning("sendType illegal");
                return;
            }
            _curSendType = sendType;
        }

        internal void SetUserId(string userId)
        {
            Logger.Log(string.Format("SetUserId: {0}", userId));
            DevicesInfo.UserId = userId;
            // auto setUser
            Dictionary<string, object> properties = new Dictionary<string, object>();
            string userCustom = JsonConvert.SerializeObject(properties);
            ReportCustom((int)DBSDK_CUSTOM_TYPE_ENUM.USER, (int)DBSDK_OPERATE_TYPE_ENUM.SET, userCustom);
        }

        internal static void SetChannel(string channel)
        {
            Logger.Log(string.Format("SetChannel: {0}", channel));
            DevicesInfo.Channel = channel;
        }

        internal static void SetDeviceId(string deviceId)
        {
            if (_isInit) { return; }
            if (string.IsNullOrEmpty(deviceId)) { return; }


            Logger.Log(string.Format("SetDeviceId: {0}", deviceId));
            DevicesInfo.DeviceId = deviceId;
            // auto setDevice
            //Hashtable properties = new Hashtable();
            //string deviceCustom = JsonConvert.SerializeObject(properties);
            //ReportCustom((int)DBSDK_CUSTOM_TYPE_ENUM.DEVICE, (int)DBSDK_OPERATE_TYPE_ENUM.SET, deviceCustom);
        }

        internal static string GetDeviceId()
        {
            return DevicesInfo.DeviceId;
        }

        internal void SetReportInterval(int reportInterval)
        {
            if (!_isInit)
            {
                return;
            }
            Logger.Log("setReportInterval: " + reportInterval);
            if (reportInterval < 1000)
            {
                Logger.LogWarning("reportInterval can't < 1000");
                return;
            }
            ReportSettings.ReportInterval = reportInterval / 1000;
        }

        internal void SetReportLimit(int reportSizeLimit)
        {
            if (!_isInit)
            {
                return;
            }

            Logger.Log("setReportLimit: " + reportSizeLimit);
            if (reportSizeLimit < 1)
            {
                Logger.LogWarning("reportSizeLimit can't < 1");
                return;
            }
            if (reportSizeLimit > Constants.DEFAULT_REPORT_SIZE_MAX_LIMIT)
            {
                reportSizeLimit = Constants.DEFAULT_REPORT_SIZE_MAX_LIMIT;
                Logger.LogWarning("reportSizeLimit can't > 50, use 50");
            }
            ReportSettings.ReportSizeLimit = reportSizeLimit;
        }

        internal void ReportEvent(string eventName, string customProperty, int reportChannelType = (int)Constants.ReportChannel.ChannelTypePrj, int sendType = Constants.FUNNY_DB_SEND_TYPE_NONE)
        {
            if (!_isInit)
            {
                return;
            }
            if (!ReportSettings.CanCollect())
            {
                Logger.Log("cur status can not collect");
                return;
            }
            Logger.LogVerbose(string.Format("ReportEvent eventName: {0} customProperty: {1}", eventName, customProperty));
            try
            {
                Dictionary<string, object> eventObj = new Dictionary<string, object>()
                {
                    { Constants.KEY_TIME, CalibratedTime.GetInMills() },
                    { Constants.KEY_LOG_ID, _random.Next() + "" }
                };
                string networkType = "NONE";
                switch (Application.internetReachability)
                {
                    case NetworkReachability.ReachableViaCarrierDataNetwork:
                        networkType = "4G";
                        break;
                    case NetworkReachability.ReachableViaLocalAreaNetwork:
                        networkType = "WIFI";
                        break;
                }

                #region 设备可变维度
                eventObj.Add(Constants.KEY_DEVICE_ID, DevicesInfo.DeviceId);
                eventObj.Add(Constants.KEY_OS_VERSION, DevicesInfo.OsVersion);
                eventObj.Add(Constants.KEY_NETWORK, networkType);
                eventObj.Add(Constants.KEY_CARRIER, DevicesInfo.Carrier);
                eventObj.Add(Constants.KEY_SYSTEM_LANGUAGE, DevicesInfo.Language);
                eventObj.Add(Constants.KEY_ZONE_OFFSET, DevicesInfo.TimeZone);
                #endregion

                eventObj.Add(Constants.KEY_EVENT, eventName);

                if (!string.IsNullOrEmpty(customProperty))
                {
                    try
                    {
                        Dictionary<string, object> customeObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(customProperty);
                        IDictionaryEnumerator enumerator = customeObj.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            eventObj.Add(enumerator.Key.ToString(), enumerator.Value);
                        }

                    }
                    catch (Exception e)
                    {
                        Logger.LogError(string.Format("ReportEvent customProperty error: {0}", e.Message));
                    }
                }
                // TODO 维度信息应该用一个类维护起来，需要用的时候拷贝到新字典，维护性较好
                #region 其它维度
                eventObj.Add(Constants.KEY_USER_ID, DevicesInfo.UserId);
                eventObj.Add(Constants.KEY_CHANNEL, DevicesInfo.Channel);
                #endregion

                #region SDK 维度
                eventObj.Add(Constants.KEY_SDK_TYPE, DevicesInfo.SdkType);
                eventObj.Add(Constants.KEY_SDK_VERSION, DevicesInfo.SdkVersion);
                #endregion

                #region 设备常用固定维度
                eventObj.Add(Constants.KEY_DEVICE_MODEL, DevicesInfo.DeviceModel);
                eventObj.Add(Constants.KEY_DEVICE_NAME, DevicesInfo.DeviceName);
                eventObj.Add(Constants.KEY_MANUFACTURER, DevicesInfo.Manufacturer);
                eventObj.Add(Constants.KEY_SCREEN_HEIGHT, DevicesInfo.ScreenHeight);
                eventObj.Add(Constants.KEY_SCREEN_WIDTH, DevicesInfo.ScreenWidth);
                //eventObj.Add(Constants.KEY_OS, DevicesInfo.Os);
                eventObj.Add(Constants.KEY_OS_PLATFORM, DevicesInfo.OsPlatform);
                #endregion

                if (eventName == Constants.REPORT_EVENT_INSTALL_NAME
                    || eventName == Constants.REPORT_EVENT_START_NAME
                    || eventName == Constants.REPORT_EVENT_DEVICE_LOGIN_NAME)
                {
                    #region 设备不常用固定维度
                    eventObj.Add(Constants.KEY_RAM_CAPACITY, DevicesInfo.RamCapacity);
                    eventObj.Add(Constants.KEY_CPU_MODEL, DevicesInfo.CPUModel);
                    eventObj.Add(Constants.KEY_CPU_CORE_COUNT, DevicesInfo.CPUCoreCnt);
                    eventObj.Add(Constants.KEY_CPU_FREQUENCY, DevicesInfo.CpuFrequency);
                    #endregion
                }

                Dictionary<string, object> mEvent = new Dictionary<string, object>()
                {
                    { Constants.KEY_TYPE, Constants.VALUE_EVENT },
                    { Constants.KEY_DATA, eventObj }
                };
                Report(mEvent, reportChannelType, sendType);
            }
            catch (Exception e)
            {
                Logger.LogError(string.Format("ReportEvent error: {0}", e.Message));
            }
        }

        internal void ReportCustom(int customType, int operateType, string customStr)
        {
            if (!_isInit)
            {
                return;
            }

            if (!ReportSettings.CanCollect())
            {
                Logger.Log("cur status can not collect");
                return;
            }

            Logger.Log(string.Format("ReportCustom customType: {0} customStr: {1}", customType, customStr));
            try
            {
                Dictionary<string, object> customeObj = new Dictionary<string, object>();
                switch (operateType)
                {
                    case (int)DBSDK_OPERATE_TYPE_ENUM.SET:
                        customeObj.Add(Constants.KEY_OPERATE, Constants.VALUE_SET);
                        break;
                    case (int)DBSDK_OPERATE_TYPE_ENUM.ADD:
                        customeObj.Add(Constants.KEY_OPERATE, Constants.VALUE_ADD);
                        break;
                    case (int)DBSDK_OPERATE_TYPE_ENUM.SET_ONCE:
                        customeObj.Add(Constants.KEY_OPERATE, Constants.VALUE_SET_ONCE);
                        break;
                }
                Dictionary<string, object> properties = JsonConvert.DeserializeObject<Dictionary<string, object>>(customStr);
                customeObj.Add(Constants.KEY_TIME, CalibratedTime.GetInMills());
                customeObj.Add(Constants.KEY_LOG_ID, _random.Next() + "");
                Dictionary<string, object> mUserCustom = new Dictionary<string, object>();

                switch (customType)
                {
                    case (int)DBSDK_CUSTOM_TYPE_ENUM.USER:
                        if (DevicesInfo.UserId == null)
                        {
                            return;
                        }
                        customeObj.Add(Constants.KEY_IDENTIFY, DevicesInfo.UserId);
                        mUserCustom.Add(Constants.KEY_TYPE, Constants.VALUE_USER_MUTATION);
                        break;
                    case (int)DBSDK_CUSTOM_TYPE_ENUM.DEVICE:
                        if (DevicesInfo.DeviceId == null)
                        {
                            return;
                        }

                        properties.Add(Constants.KEY_DEVICE_ID, DevicesInfo.DeviceId);
                        properties.Add(Constants.KEY_DEVICE_MODEL, DevicesInfo.DeviceModel);
                        properties.Add(Constants.KEY_DEVICE_NAME, DevicesInfo.DeviceName);
                        properties.Add(Constants.KEY_MANUFACTURER, DevicesInfo.Manufacturer);
                        properties.Add(Constants.KEY_RAM_CAPACITY, DevicesInfo.RamCapacity);
                        properties.Add(Constants.KEY_CPU_MODEL, DevicesInfo.CPUModel);
                        properties.Add(Constants.KEY_CPU_CORE_COUNT, DevicesInfo.CPUCoreCnt);
                        properties.Add(Constants.KEY_CPU_FREQUENCY, DevicesInfo.CpuFrequency);
                        properties.Add(Constants.KEY_SCREEN_WIDTH, DevicesInfo.ScreenWidth);
                        properties.Add(Constants.KEY_SCREEN_HEIGHT, DevicesInfo.ScreenHeight);
                        customeObj.Add(Constants.KEY_IDENTIFY, DevicesInfo.DeviceId);
                        mUserCustom.Add(Constants.KEY_TYPE, Constants.VALUE_DEVICE_MUTATION);
                        break;
                    default:
                        return;
                }
                customeObj.Add(Constants.KEY_PROPERTY, properties);
                mUserCustom.Add(Constants.KEY_DATA, customeObj);
                Report(mUserCustom, (int)Constants.ReportChannel.ChannelTypePrj, Constants.FUNNY_DB_SEND_TYPE_NONE);
            }
            catch (Exception e)
            {
                Logger.LogError(string.Format("ReportCustom error: {0}", e.Message));
            }
        }

        /// <summary>
        /// Flush 只上报游戏的缓存事件
        /// </summary>
        internal void Flush()
        {
            if (!ReportSettings.CanSend())
            {
                return;
            }
            AutoReportTimer.Instance.DoCheckDataSource(true);
        }

        internal void EnableDebug()
        {
            //FunnyDBLog.IsDebug = true;
        }
        /// <summary>
        /// 上报事件基础方法
        /// </summary>
        /// <param name="evenObj"></param>
        /// <param name="reportChannelType"></param>
        /// <param name="isReportNow">是否立即上报，默认为 true</param>
        internal void Report(Dictionary<string, object> evenObj, int reportChannelType, int sendType)
        {
            try
            {
                int finalSendType = sendType == Constants.FUNNY_DB_SEND_TYPE_NONE ? _curSendType : sendType;

                AccessInfo accessInfo = (AccessInfo)AccessKeyHashTable[reportChannelType];

                if (finalSendType == ((int)DBSDK_SEND_TYPE_ENUM.DELAY))
                {
                    StartCoroutinWithAciton(() =>
                    {
                        DataSource.Create(JsonConvert.SerializeObject(evenObj), accessInfo.AccessKeyId);
                    });
                }
                else if (finalSendType == ((int)DBSDK_SEND_TYPE_ENUM.NOW))
                {
                    if (ReportSettings.CanSend())
                    {
                        List<Dictionary<string, object>> messageArr = new List<Dictionary<string, object>>()
                        {
                            evenObj
                        };
                        Dictionary<string, object> sendObj = new Dictionary<string, object>()
                        {
                            { Constants.KEY_MESSAGES, messageArr }
                        };

                        string sendData = JsonConvert.SerializeObject(sendObj);

                        IngestSignature ingestSignature = new IngestSignature(accessInfo)
                        {
                            Nonce = _random.Next() + "",
                            Timestamp = CalibratedTime.GetInMills() + "",
                            Body = sendData,
                        };
                        string sign = EncryptUtils.GetEncryptSign(accessInfo.AccessSecret, ingestSignature.GetToEncryptContent());
                        ingestSignature.Sign = sign;
                        ingestSignature.OriginEvents = messageArr;
                        EventUpload.PostIngest(ingestSignature);
                    }
                    else if (ReportSettings.CanToDB())
                    {
                        StartCoroutinWithAciton(() =>
                        {
                            DataSource.Create(JsonConvert.SerializeObject(evenObj), accessInfo.AccessKeyId);
                        });
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError(string.Format("Report error: {0}", e.Message));
            }
        }
    }

    internal sealed partial class FunnyDBPCInstance : MonoBehaviour
    {
        private SynchronizationContext originalContext;
        private int mainThreadId = int.MinValue;
        private int _curSendType = (int)DBSDK_SEND_TYPE_ENUM.NOW;
        private int _lastReportInterval = ReportSettings.ReportInterval;
        private WaitForSeconds _waitForSeconds = null;
        private readonly System.Random _random = new System.Random();
        private static bool _isInit = false; // 是否初始化

        internal void StartCoroutinWithAciton(Action action)
        {
            if (Thread.CurrentThread.ManagedThreadId == mainThreadId)
            {
                StartCoroutine(StartCorutineInner(action));
                return;
            }
            originalContext.Post(_ =>
            {
                StartCoroutine(StartCorutineInner(action));
            }, null);
        }

        private IEnumerator StartCorutineInner(Action action)
        {
            action();
            yield return null;
        }

    }

}