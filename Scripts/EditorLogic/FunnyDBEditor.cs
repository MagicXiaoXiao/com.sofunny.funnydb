#if (UNITY_EDITOR || UNITY_STANDALONE)

using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
/// <summary>
/// Unity Editor Logic(Simple)
/// </summary>
namespace SoFunny.FunnyDB {
    public sealed class FunnyDBEditor {
        private static FunnyDBEditor instance;
        public static FunnyDBEditor Instance() {
            if (instance == null) {
                instance = new FunnyDBEditor();
            }
            return instance;
        }
        private System.Random random = new System.Random();
        private bool mIsInit = false; // 是否初始化
        private bool mIsOpen = true; // 功能开关

        public void Initialize (string accessKeyId, string accessKeySecret, string endPoint) {
            if (mIsInit) {
                return;
            }
            Common.Init(accessKeyId, accessKeySecret, endPoint);
            mIsInit = true;
            Debug.Log("FunnyDB Init true");
            ReportEvent("#device_login", "");
        }

        // InValid in Editorss
        public void SetSDKStatus(int status) {
            Debug.Log("Editor 下不支持当前行为");
        }

        // InValid in Editor
        public void SetSDKSendType(int sendType) {
            Debug.Log("Editor 下不支持当前行为");
        }

        public void SetUserId(string userId) {
            if (!mIsOpen) {
                return;
            }
            Logger.Log(string.Format("SetUserId: {0}", userId));
            DevicesInfo.UserId = userId;
            // auto setUser
            Hashtable properties = new Hashtable();
            string userCustom = JsonConvert.SerializeObject(properties);
            ReportCustom((int)DBSDK_CUSTOM_TYPE_ENUM.USER, (int)DBSDK_OPERATE_TYPE_ENUM.SET, userCustom);
        }

        public void SetChannel(string channel) {
            if (!mIsOpen) {
                return;
            }
            Logger.Log(string.Format("SetChannel: {0}", channel));
            DevicesInfo.Channel = channel;
        }

        public void SetDeviceId(string deviceId) {
            if (mIsInit) { return; }
            if (string.IsNullOrEmpty(deviceId)) { return; }


            Logger.Log(string.Format("SetDeviceId: {0}", deviceId));
            DevicesInfo.DeviceId = deviceId;
            // auto setDevice
            //Hashtable properties = new Hashtable();
            //string deviceCustom = JsonConvert.SerializeObject(properties);
            //ReportCustom((int)DBSDK_CUSTOM_TYPE_ENUM.DEVICE, (int)DBSDK_OPERATE_TYPE_ENUM.SET, deviceCustom);
        }

        public string GetDeviceId() {
            return DevicesInfo.DeviceId;
        }

        // InValid in Editor
        public void SetReportInterval(int reportInterval) {
            Debug.Log("Editor 下不支持当前行为");
        }

        // InValid in Editor
        public void SetReportLimit(int reportSizeLimit) {
            Debug.Log("Editor 下不支持当前行为");
        }

        public void ReportEvent(string eventName, string customProperty) {
            if (!mIsInit) {
                return;
            }
            if (!mIsOpen) {
                return;
            }
            Logger.Log(string.Format("ReportEvent eventName: {0} customProperty: {1}", eventName, customProperty));
            try {
                Hashtable eventObj = new Hashtable();
                eventObj.Add(Constants.KEY_TIME, GetTimeStamp());
                eventObj.Add(Constants.KEY_LOG_ID, random.Next() + "");
                string networkType = "NONE";
                switch (Application.internetReachability) {
                    case NetworkReachability.ReachableViaCarrierDataNetwork:
                        networkType = "4G";
                        break;
                    case NetworkReachability.ReachableViaLocalAreaNetwork:
                        networkType = "WIFI";
                        break;
                }
                eventObj.Add(Constants.KEY_NETWORK, networkType);
                eventObj.Add(Constants.KEY_EVENT, eventName);

                if (!string.IsNullOrEmpty(customProperty)) {
                    try {
                        Hashtable customeObj = JsonConvert.DeserializeObject<Hashtable>(customProperty);
                        IDictionaryEnumerator enumerator = customeObj.GetEnumerator();
                        while (enumerator.MoveNext()) {
                            eventObj.Add(enumerator.Key, enumerator.Value);
                        }

                    }
                    catch (Exception e) {
                        Logger.LogError(string.Format("ReportEvent customProperty error: {0}", e.Message));
                    }
                }
                
                eventObj.Add(Constants.KEY_DEVICE_ID, DevicesInfo.DeviceId);
                eventObj.Add(Constants.KEY_USER_ID, DevicesInfo.UserId);
                eventObj.Add(Constants.KEY_CHANNEL, DevicesInfo.Channel);
                eventObj.Add(Constants.KEY_SDK_TYPE, DevicesInfo.SdkType);
                eventObj.Add(Constants.KEY_SDK_VERSION, DevicesInfo.SdkVersion);
                eventObj.Add(Constants.KEY_DEVICE_MODEL, DevicesInfo.DeviceModel);
                eventObj.Add(Constants.KEY_MANUFACTURER, DevicesInfo.Manufacturer);
                eventObj.Add(Constants.KEY_SCREEN_HEIGHT, DevicesInfo.ScreenHeight);
                eventObj.Add(Constants.KEY_SCREEN_WIDTH, DevicesInfo.ScreenWidth);
                eventObj.Add(Constants.KEY_OS, DevicesInfo.Os);
                eventObj.Add(Constants.KEY_OS_PLATFORM, DevicesInfo.OsPlatform);
                eventObj.Add(Constants.KEY_OS_VERSION, DevicesInfo.OsVersion);
                eventObj.Add(Constants.KEY_CARRIER, DevicesInfo.Carrier);
                eventObj.Add("#system_language", "unknown");
                eventObj.Add("#zone_offset", 0);

                Hashtable mEvent = new Hashtable();
                mEvent.Add(Constants.KEY_TYPE, Constants.VALUE_EVENT);
                mEvent.Add(Constants.KEY_DATA, eventObj);
                Report(mEvent);
            } catch (Exception e) {
                Logger.LogError(string.Format("ReportEvent error: {0}", e.Message));
            }
        }

        public void ReportCustom(int customType, int operateType, string customStr) {
            if (!mIsInit) {
                return;
            }
            if (!mIsOpen) {
                return;
            }
            Logger.Log(string.Format("ReportCustom customType: {0} customStr: {1}", customType, customStr));
            try {
                Hashtable customeObj = new Hashtable();
                switch (operateType) {
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
                customeObj.Add(Constants.KEY_PROPERTY, JsonConvert.DeserializeObject<Hashtable>(customStr));
                customeObj.Add(Constants.KEY_TIME, GetTimeStamp());
                customeObj.Add(Constants.KEY_LOG_ID, random.Next() + "");
                Hashtable mUserCustom = new Hashtable();
                switch (customType) {
                    case (int)DBSDK_CUSTOM_TYPE_ENUM.USER:
                        if (DevicesInfo.UserId == null) {
                            return;
                        }
                        customeObj.Add(Constants.KEY_IDENTIFY, DevicesInfo.UserId);
                        mUserCustom.Add(Constants.KEY_TYPE, Constants.VALUE_USER_MUTATION);
                        break;
                    case (int)DBSDK_CUSTOM_TYPE_ENUM.DEVICE:
                        if (DevicesInfo.DeviceId == null) {
                            return;
                        }
                        customeObj.Add(Constants.KEY_IDENTIFY, DevicesInfo.DeviceId);
                        mUserCustom.Add(Constants.KEY_TYPE, Constants.VALUE_DEVICE_MUTATION);
                        break;
                    default:
                        return;
                }
                mUserCustom.Add(Constants.KEY_DATA, customeObj);
                Report(mUserCustom);
            } catch (Exception e) {
                Logger.LogError(string.Format("ReportCustom error: {0}", e.Message));
            }
        }

        // InValid in Editor
        public void Flush() {
            Debug.Log("Editor 下不支持当前行为");
        }

        // InValid in Editor
        public void EnableDebug() {
            mIsOpen = true;
        }
        
        private void Report(Hashtable evenObj) {
            try {
                ArrayList messageArr = new ArrayList();
                messageArr.Add(evenObj);
                Hashtable sendObj = new Hashtable();
                sendObj.Add(Constants.KEY_MESSAGES, messageArr);
                string sendData = JsonConvert.SerializeObject(sendObj);
                
                Logger.Log(string.Format("sendData: {0}", sendData));

                IngestSignature ingestSignature = new IngestSignature(Common.AccessKeyId);
                ingestSignature.Nonce = random.Next() + "";
                ingestSignature.Timestamp = GetTimeStamp() + "";
                ingestSignature.Body = sendData;
                string sign = EncryptUtils.GetEncryptSign(Common.AccessKeySecret, ingestSignature.getToEncryptContent());
                EventUpload.PostIngest(ingestSignature, sendData, sign);
            } catch (Exception e) {
                Logger.LogError(string.Format("Report error: {0}", e.Message));
            }
        }

        private long GetTimeStamp() {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds) - 8 * 60 * 60 * 1000;
        }
    }
}

#endif