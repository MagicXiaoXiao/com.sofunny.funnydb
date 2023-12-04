#if UNITY_ANDROID
using System;
using System.Threading;
using UnityEngine;

namespace SoFunny.FunnyDB.Bridge
{
    internal class AndroidNotificationMessage : AndroidJavaProxy
    {
        private readonly SynchronizationContext OriginalContext;

        public AndroidNotificationMessage() : base("java 层接口映射路径") //TODO: 需填写 java 映射接口路径
        {
            OriginalContext = SynchronizationContext.Current;
        }


        internal void Post(string identifier)
        {
            OriginalContext.Post(_ =>
            {
                FunnyDBNotificationCenter.Default.Post(identifier);
            }, null);

        }

        internal void Post(string identifier, string jsonValue)
        {
            OriginalContext.Post(_ =>
            {
                FunnyDBNotificationCenter.Default.Post(identifier, FunnyDBNotificationValue.Create(jsonValue));
            }, null);

        }


    }

    internal class FunnyDBAndroidAgent : IFunnyDBAgent
    {

        internal FunnyDBAndroidAgent()
        {
            // 下发通知对象到 Android 层
            FunnyBridge.CallStatic("registerNotification", new AndroidNotificationMessage());

            // 监听 funnydb.init.complete 通知
            // 该通知接收数据 JSON 格式为：{ "code":0, "message":"初始化成功" }
            FunnyDBNotificationCenter.Default.AddObserver(this, "funnydb.init.complete", (value) =>
            {
                int code = value.TryGetValue<int>("code"); // 0 = 成功，其他统一为失败
                string message = value.TryGetValue<string>("message"); // 结果消息

                FunnyDBSDK.AttackInitEvent(code == 0, message);
            });
        }

        // Get FunnyBri
        private static readonly string FUNNY_BRIDGE_CLASS = "com.sofunny.eventAnalyzer.FunnyBridge";
        private static AndroidJavaObject mFunnyBridge;
        private static long _sOnSubSystemInitInTimeMills = 0L;
        internal static AndroidJavaObject FunnyBridge
        {
            get
            {
                if (mFunnyBridge == null)
                {
                    mFunnyBridge = new AndroidJavaObject(FUNNY_BRIDGE_CLASS);
                }
                return mFunnyBridge;
            }
        }
        private static AndroidJavaObject mFunnyInstance;
        internal static AndroidJavaObject FunnyInstance
        {
            get
            {
                if (mFunnyInstance == null)
                {
                    mFunnyInstance = FunnyBridge.CallStatic<AndroidJavaObject>("getInstance");
                }
                return mFunnyInstance;
            }
        }
        // Get Unity Activity
        private static readonly string UNTIFY_CLASS = "com.unity3d.player.UnityPlayer";
        private static AndroidJavaClass mUnityClass;
        internal static AndroidJavaClass UnityClass
        {
            get
            {
                if (null == mUnityClass)
                {
                    mUnityClass = new AndroidJavaClass(UNTIFY_CLASS);
                }
                return mUnityClass;
            }
        }
        private static AndroidJavaObject mUnityActivity;
        internal static AndroidJavaObject UnityActivity
        {
            get
            {
                if (null == mUnityActivity)
                {
                    mUnityActivity = UnityClass.GetStatic<AndroidJavaObject>("currentActivity");
                }
                return mUnityActivity;
            }
        }

        public int Initialize(string accessKeyId, string accessKeySecret, string endPoint)
        {
            int flag = 0;
            try
            {
                flag = FunnyInstance.Call<int>("initialize", UnityActivity, accessKeyId, accessKeySecret, endPoint, _sOnSubSystemInitInTimeMills);
            }
            catch (Exception e)
            {
                flag = -1;
                Logger.LogError("called initialize error: " + e);
            }
            return flag;
        }

        public void SetSDKStatus(int status)
        {
            try
            {
                FunnyInstance.Call("setSDKStatus", status);
            }
            catch (Exception e)
            {
                Logger.LogError("called setSDKStatus error: " + e);
            }
        }

        public void SetSDKSendType(int sendType)
        {
            try
            {
                FunnyInstance.Call("setSDKSendType", sendType);
            }
            catch (Exception e)
            {
                Logger.LogError("called setSDKSendType error: " + e);
            }
        }

        public void SetUserId(string userId)
        {
            try
            {
                FunnyInstance.Call("setUserId", userId);
            }
            catch (Exception e)
            {
                Logger.LogError("called setUserId error: " + e);
            }
        }

        public void SetChannel(string channel)
        {
            try
            {
                FunnyInstance.Call("setChannel", channel);
            }
            catch (Exception e)
            {
                Logger.LogError("called setChannel error: " + e);
            }
        }

        public void SetDeviceId(string deviceId)
        {
            try
            {
                FunnyInstance.Call("setDeviceId", UnityActivity, deviceId);
            }
            catch (Exception e)
            {
                Logger.LogError("called setDeviceId error: " + e);
            }
        }

        public string GetDeviceId()
        {
            try
            {
                return FunnyInstance.Call<string>("getDeviceId", UnityActivity);
            }
            catch (Exception e)
            {
                Logger.LogError("called setDeviceId error: " + e);
                return string.Empty;
            }
        }

        public void ReportEvent(string eventName, string customProperty = "")
        {
            try
            {
                FunnyInstance.Call("reportEvent", eventName, customProperty);
            }
            catch (Exception e)
            {
                Logger.LogError("called reportEvent error: " + e);
            }
        }

        public void SetReportInterval(int interval)
        {
            try
            {
                FunnyInstance.Call("setReportInterval", interval);
            }
            catch (Exception e)
            {
                Logger.LogError("called setReportInterval error: " + e);
            }
        }

        public void SetReportLimit(int limit)
        {
            try
            {
                FunnyInstance.Call("setReportLimit", limit);
            }
            catch (Exception e)
            {
                Logger.LogError("called setReportLimit error: " + e);
            }
        }

        public void Flush()
        {
            try
            {
                FunnyInstance.Call("flush");
            }
            catch (Exception e)
            {
                Logger.LogError("called report error: " + e);
            }
        }

        public void ReportCustom(int customType, int operateType, string jsonStr)
        {
            try
            {
                FunnyInstance.Call("reportCustom", customType, operateType, jsonStr);
            }
            catch (Exception e)
            {
                Logger.LogError("called reportCustom error: " + e);
            }
        }

        public void EnableDebug()
        {
            try
            {
                FunnyInstance.Call("enableDebug");
            }
            catch (Exception e)
            {
                Logger.LogError("called enableDebug error: " + e);
            }
        }

        public void getUniqueIdentify()
        {
            FunnyInstance.Call("getUniqueIdentify", 1);
            FunnyInstance.Call("getUniqueIdentify", 2);
            FunnyInstance.Call("getUniqueIdentify", 3);
            FunnyInstance.Call("getUniqueIdentify", 4);
        }

        public void SetOAIDCertInfo(string certInfo)
        {
            FunnyInstance.Call("setOAIDCerInfo", certInfo);
        }

        public void SetOAIDCertAssetName(string certAssetName)
        {
            FunnyInstance.Call("setOAIDCerAssetName", certAssetName);
        }

        public void ShowToast(string msg)
        {
            FunnyInstance.Call("showToast", UnityActivity, msg);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void OnSubSystemInit()
        {
            AndroidJavaClass javaClass = new AndroidJavaClass("android.os.SystemClock");
            _sOnSubSystemInitInTimeMills = javaClass.CallStatic<long>("elapsedRealtime");
        }

    }
}
#endif