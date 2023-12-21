#if UNITY_STANDALONE || UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        internal static FunnyDBPCInstance Instance;        
        internal readonly Hashtable AccessKeyHashTable = new Hashtable();
        internal ICalibratedTime CalibratedTime = null;

        private void Awake()
        {
            Instance = this;
            _originalContext = SynchronizationContext.Current;
            _mainThreadId = Thread.CurrentThread.ManagedThreadId;
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
                GCSuppressProcess(null);
                AutoReportTimer.Instance.DoCheckDataSource();
            }
        }

        internal void Initialize(string accessKeyId, string accessKeySecret, string endPoint, int reportChannelType = (int)Constants.ReportChannel.ChannelTypePrj)
        {
            _JsonStringWriter = new StringWriter();
            _jsonWriter = new JsonTextWriter(_JsonStringWriter);

            AccessKeyHashTable[reportChannelType] = new AccessInfo(accessKeyId, accessKeySecret, endPoint);
            if (!_isInit)
            {
                // Do first Init
                _isInit = true;
                CalibratedTime = new CalibratedTimeWithNTP();

                AutoEventCollectManager.Instance.Init();
                AutoReportTimer.Instance.Init();

            }
            DataSource.Init(accessKeyId);
            Logger.LogVerbose("FunnyDB Init true");
            if (reportChannelType == ((int)Constants.ReportChannel.ChannelTypePrj))
            {
                ReportEvent("#device_login", null, reportChannelType);
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
            if (Thread.CurrentThread.ManagedThreadId == _mainThreadId)
            {
                ReportEventInternal(eventName, customProperty, reportChannelType, sendType);
                return;
            }
            _originalContext.Post(_ =>
            {
                ReportEventInternal(eventName, customProperty, reportChannelType, sendType);
            }, null);
        }


        internal void ReportCustom(int customType, int operateType, string customStr)
        {
            if (Thread.CurrentThread.ManagedThreadId == _mainThreadId)
            {
                ReportCustomInternal(customType, operateType, customStr);
                return;
            }
            _originalContext.Post(_ =>
            {
                ReportCustomInternal(customType, operateType, customStr);
            }, null);
        }

        /// <summary>
        /// Flush 只上报游戏的缓存事件
        /// </summary>
        internal void Flush()
        {
            if (Thread.CurrentThread.ManagedThreadId == _mainThreadId)
            {
                FlushInternal();
                return;
            }
            _originalContext.Post(_ =>
            {
                FlushInternal();
            }, null);
        }

        internal void EnableDebug()
        {
            //FunnyDBLog.IsDebug = true;
        }

        internal void Report(string evenObj, int reportChannelType, int sendType)
        {
            if (Thread.CurrentThread.ManagedThreadId == _mainThreadId)
            {
                ReportInternal(evenObj, reportChannelType, sendType);
                return;
            }
            _originalContext.Post(_ =>
            {
                ReportInternal(evenObj, reportChannelType, sendType);
            }, null);
        }

    }

    internal sealed partial class FunnyDBPCInstance : MonoBehaviour
    {
        private SynchronizationContext _originalContext;
        private int _mainThreadId = int.MinValue;
        private int _curSendType = (int)DBSDK_SEND_TYPE_ENUM.NOW;
        private int _lastReportInterval = ReportSettings.ReportInterval;
        private WaitForSeconds _waitForSeconds = null;
        private readonly System.Random _random = new System.Random();
        private static bool _isInit = false; // 是否初始化
        private StringWriter _JsonStringWriter = null;
        private JsonWriter _jsonWriter = null;
        /// <summary>
        /// 字符串 GC 抑制数组
        /// </summary>
        private List<string> _strsSuppressList = new List<string>();
        private const int RELEASE_CNT = 20;

        /// <summary>
        /// 处理字符串 GC 抑制过程控制
        /// </summary>
        /// <param name="suppressStr"></param>
        private void GCSuppressProcess(string suppressStr)
        {
            if (string.IsNullOrEmpty(suppressStr))
            {
                _strsSuppressList.Clear();
                return;
            }
            _strsSuppressList.Add(suppressStr);
            if (_strsSuppressList.Count >= RELEASE_CNT)
            {
                _strsSuppressList.Clear();
            }
        }

        /// <summary>
        /// 去掉左右两边的花括号，直接进行拼接
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string HandleCustomJson(string input)
        {

            if (input == null || input.Length <= 2)
            {
                return null;
            }

            int leftBraceIndex = input.IndexOf('{');
            if (leftBraceIndex == -1)
            {
                return null;
            }

            int lastRightBraceIndex = input.LastIndexOf('}');
            if (lastRightBraceIndex == -1)
            {
                return null;
            }

            char[] chars = input.ToCharArray();
            return new string(chars, leftBraceIndex + 1, lastRightBraceIndex - 1);
        }

        // JsonWriter 没有正常 Close 的情况下都要重置
        private void ResetJsonWriter()
        {
            _JsonStringWriter = new StringWriter();
            _jsonWriter = new JsonTextWriter(_JsonStringWriter);
        }

        private void FlushInternal()
        {
            if (!ReportSettings.CanSend())
            {
                return;
            }
            AutoReportTimer.Instance.DoCheckDataSource(true);
        }

        /// <summary>
        /// 上报事件基础方法
        /// </summary>
        /// <param name="evenObj"></param>
        /// <param name="reportChannelType"></param>
        /// <param name="isReportNow">是否立即上报，默认为 true</param>
        private void ReportInternal(string evenObj, int reportChannelType, int sendType)
        {
            try
            {
                int finalSendType = sendType == Constants.FUNNY_DB_SEND_TYPE_NONE ? _curSendType : sendType;

                AccessInfo accessInfo = (AccessInfo)AccessKeyHashTable[reportChannelType];

                if (finalSendType == ((int)DBSDK_SEND_TYPE_ENUM.DELAY))
                {
                    DataSource.Create(evenObj, accessInfo.AccessKeyId);
                }
                else if (finalSendType == ((int)DBSDK_SEND_TYPE_ENUM.NOW))
                {
                    if (ReportSettings.CanSend())
                    {
                        _jsonWriter.WriteStartObject();

                        _jsonWriter.WritePropertyName(Constants.KEY_MESSAGES);
                        _jsonWriter.WriteStartArray();
                        _jsonWriter.WriteRawValue(evenObj);
                        _jsonWriter.WriteEndArray();

                        _jsonWriter.WriteEndObject();

                        string sendData = _JsonStringWriter.ToString();
                        _JsonStringWriter.GetStringBuilder().Clear();
                        GCSuppressProcess(sendData);
                        Logger.LogVerbose("ReportEvent Str: " + sendData);
                        IngestSignature ingestSignature = new IngestSignature(accessInfo)
                        {
                            Nonce = _random.Next().ToString(),
                            Timestamp = CalibratedTime.GetInMills().ToString(),
                            Body = sendData,
                        };
                        string sign = EncryptUtils.GetEncryptSign(accessInfo.AccessSecret, ingestSignature.GetToEncryptContent());
                        ingestSignature.Sign = sign;
                        ingestSignature.OriginEvent = evenObj;
                        EventUpload.PostIngest(ingestSignature);
                    }
                    else if (ReportSettings.CanToDB())
                    {
                        DataSource.Create(evenObj, accessInfo.AccessKeyId);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError(string.Format("Report error: {0}", e.Message));
                ResetJsonWriter();
            }
        }

        private void ReportCustomInternal(int customType, int operateType, string customStr)
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
                _jsonWriter.WriteStartObject();

                if (customType == ((int)DBSDK_CUSTOM_TYPE_ENUM.USER))
                {
                    if (string.IsNullOrEmpty(DevicesInfo.UserId))
                    {
                        ResetJsonWriter();
                        return;
                    }
                    JsonWriterUtils.Write(_jsonWriter, Constants.KEY_TYPE, Constants.VALUE_USER_MUTATION);
                }
                else if (customType == (int)DBSDK_CUSTOM_TYPE_ENUM.DEVICE)
                {
                    if (string.IsNullOrEmpty(DevicesInfo.DeviceId))
                    {
                        ResetJsonWriter();
                        return;
                    }
                    JsonWriterUtils.Write(_jsonWriter, Constants.KEY_TYPE, Constants.VALUE_DEVICE_MUTATION);
                }

                _jsonWriter.WritePropertyName(Constants.KEY_DATA);

                _jsonWriter.WriteStartObject();
                switch (operateType)
                {
                    case (int)DBSDK_OPERATE_TYPE_ENUM.SET:
                        JsonWriterUtils.Write(_jsonWriter, Constants.KEY_OPERATE, Constants.VALUE_SET);
                        break;
                    case (int)DBSDK_OPERATE_TYPE_ENUM.ADD:
                        JsonWriterUtils.Write(_jsonWriter, Constants.KEY_OPERATE, Constants.VALUE_ADD);
                        break;
                    case (int)DBSDK_OPERATE_TYPE_ENUM.SET_ONCE:
                        JsonWriterUtils.Write(_jsonWriter, Constants.KEY_OPERATE, Constants.VALUE_SET_ONCE);
                        break;
                }

                JsonWriterUtils.Write(_jsonWriter, Constants.KEY_TIME, CalibratedTime.GetInMills());
                JsonWriterUtils.Write(_jsonWriter, Constants.KEY_LOG_ID, _random.Next().ToString());

                if (customType == ((int)DBSDK_CUSTOM_TYPE_ENUM.USER))
                {
                    JsonWriterUtils.Write(_jsonWriter, Constants.KEY_IDENTIFY, DevicesInfo.UserId);
                }
                else if (customType == (int)DBSDK_CUSTOM_TYPE_ENUM.DEVICE)
                {
                    JsonWriterUtils.Write(_jsonWriter, Constants.KEY_IDENTIFY, DevicesInfo.DeviceId);
                }

                _jsonWriter.WritePropertyName(Constants.KEY_PROPERTY);
                _jsonWriter.WriteStartObject();

                string handlerCustomPro = HandleCustomJson(customStr);
                if (!string.IsNullOrEmpty(handlerCustomPro))
                {
                    _jsonWriter.WriteRaw(handlerCustomPro);
                    _jsonWriter.WriteRaw(",");
                }

                // MOCK Data
                JsonWriterUtils.Write(_jsonWriter, Constants.KEY_DEVICE_ID, DevicesInfo.DeviceId);

                if (customType == (int)DBSDK_CUSTOM_TYPE_ENUM.DEVICE)
                {
                    JsonWriterUtils.Write(_jsonWriter, Constants.KEY_DEVICE_ID, DevicesInfo.DeviceId);
                    JsonWriterUtils.Write(_jsonWriter, Constants.KEY_DEVICE_MODEL, DevicesInfo.DeviceModel);
                    JsonWriterUtils.Write(_jsonWriter, Constants.KEY_DEVICE_NAME, DevicesInfo.DeviceName);
                    JsonWriterUtils.Write(_jsonWriter, Constants.KEY_MANUFACTURER, DevicesInfo.Manufacturer);
                    JsonWriterUtils.Write(_jsonWriter, Constants.KEY_RAM_CAPACITY, DevicesInfo.RamCapacity);
                    JsonWriterUtils.Write(_jsonWriter, Constants.KEY_CPU_MODEL, DevicesInfo.CPUModel);
                    JsonWriterUtils.Write(_jsonWriter, Constants.KEY_CPU_CORE_COUNT, DevicesInfo.CPUCoreCnt);
                    JsonWriterUtils.Write(_jsonWriter, Constants.KEY_CPU_FREQUENCY, DevicesInfo.CpuFrequency);
                    JsonWriterUtils.Write(_jsonWriter, Constants.KEY_SCREEN_WIDTH, DevicesInfo.ScreenWidth);
                    JsonWriterUtils.Write(_jsonWriter, Constants.KEY_SCREEN_HEIGHT, DevicesInfo.ScreenHeight);
                }

                _jsonWriter.WriteEndObject();
                _jsonWriter.WriteEndObject();
                _jsonWriter.WriteEndObject();

                string reportJson = _JsonStringWriter.ToString();
                _JsonStringWriter.GetStringBuilder().Clear();
                GCSuppressProcess(reportJson);
                ReportInternal(reportJson, (int)Constants.ReportChannel.ChannelTypePrj, Constants.FUNNY_DB_SEND_TYPE_NONE);
            }
            catch (Exception e)
            {
                Logger.LogError(string.Format("ReportCustom error: {0}", e.Message));
                ResetJsonWriter();
            }
        }

        private void ReportEventInternal(string eventName, string customProperty, int reportChannelType = (int)Constants.ReportChannel.ChannelTypePrj, int sendType = Constants.FUNNY_DB_SEND_TYPE_NONE)
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
                _jsonWriter.WriteStartObject();
                JsonWriterUtils.Write(_jsonWriter, Constants.KEY_TYPE, Constants.VALUE_EVENT);

                // 开始写 KEY_DATA
                _jsonWriter.WritePropertyName(Constants.KEY_DATA);

                _jsonWriter.WriteStartObject();
                JsonWriterUtils.Write(_jsonWriter, Constants.KEY_EVENT, eventName);

                string handlerCustomPro = HandleCustomJson(customProperty);
                if (!string.IsNullOrEmpty(handlerCustomPro))
                {
                    _jsonWriter.WriteRaw(",");
                    _jsonWriter.WriteRaw(handlerCustomPro);
                }

                JsonWriterUtils.Write(_jsonWriter, Constants.KEY_TIME, CalibratedTime.GetInMills());
                JsonWriterUtils.Write(_jsonWriter, Constants.KEY_LOG_ID, _random.Next().ToString());

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
                JsonWriterUtils.Write(_jsonWriter, Constants.KEY_DEVICE_ID, DevicesInfo.DeviceId);
                JsonWriterUtils.Write(_jsonWriter, Constants.KEY_OS_VERSION, DevicesInfo.OsVersion);
                JsonWriterUtils.Write(_jsonWriter, Constants.KEY_NETWORK, networkType);
                JsonWriterUtils.Write(_jsonWriter, Constants.KEY_CARRIER, DevicesInfo.Carrier);
                JsonWriterUtils.Write(_jsonWriter, Constants.KEY_SYSTEM_LANGUAGE, DevicesInfo.Language);
                JsonWriterUtils.Write(_jsonWriter, Constants.KEY_ZONE_OFFSET, DevicesInfo.TimeZone);
                #endregion

                #region 其它维度
                JsonWriterUtils.Write(_jsonWriter, Constants.KEY_USER_ID, DevicesInfo.UserId);
                JsonWriterUtils.Write(_jsonWriter, Constants.KEY_CHANNEL, DevicesInfo.Channel);
                #endregion

                #region SDK 维度
                JsonWriterUtils.Write(_jsonWriter, Constants.KEY_SDK_TYPE, DevicesInfo.SdkType);
                JsonWriterUtils.Write(_jsonWriter, Constants.KEY_SDK_VERSION, DevicesInfo.SdkVersion);
                #endregion

                #region 设备常用固定维度
                JsonWriterUtils.Write(_jsonWriter, Constants.KEY_DEVICE_MODEL, DevicesInfo.DeviceModel);
                JsonWriterUtils.Write(_jsonWriter, Constants.KEY_DEVICE_NAME, DevicesInfo.DeviceName);
                JsonWriterUtils.Write(_jsonWriter, Constants.KEY_MANUFACTURER, DevicesInfo.Manufacturer);
                JsonWriterUtils.Write(_jsonWriter, Constants.KEY_SCREEN_HEIGHT, DevicesInfo.ScreenHeight);
                JsonWriterUtils.Write(_jsonWriter, Constants.KEY_SCREEN_WIDTH, DevicesInfo.ScreenWidth);
                JsonWriterUtils.Write(_jsonWriter, Constants.KEY_OS_PLATFORM, DevicesInfo.OsPlatform);
                #endregion

                if (eventName == Constants.REPORT_EVENT_INSTALL_NAME
                    || eventName == Constants.REPORT_EVENT_START_NAME
                    || eventName == Constants.REPORT_EVENT_DEVICE_LOGIN_NAME)
                {
                    #region 设备不常用固定维度
                    JsonWriterUtils.Write(_jsonWriter, Constants.KEY_RAM_CAPACITY, DevicesInfo.RamCapacity);
                    JsonWriterUtils.Write(_jsonWriter, Constants.KEY_CPU_MODEL, DevicesInfo.CPUModel);
                    JsonWriterUtils.Write(_jsonWriter, Constants.KEY_CPU_CORE_COUNT, DevicesInfo.CPUCoreCnt);
                    JsonWriterUtils.Write(_jsonWriter, Constants.KEY_CPU_FREQUENCY, DevicesInfo.CpuFrequency);
                    #endregion
                }
                _jsonWriter.WriteEndObject();
                _jsonWriter.WriteEndObject();

                string finalStr = _JsonStringWriter.ToString();
                Logger.LogVerbose("ReportEvent Str: " + finalStr);
                _JsonStringWriter.GetStringBuilder().Clear();
                GCSuppressProcess(finalStr);
                Report(finalStr, reportChannelType, sendType);
            }
            catch (Exception e)
            {
                Logger.LogError(string.Format("ReportEvent error: {0}", e.Message));
                ResetJsonWriter();
            }
        }
    }

}
#endif