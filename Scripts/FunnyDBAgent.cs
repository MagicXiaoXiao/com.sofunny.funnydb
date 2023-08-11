using System;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// FunnyDB Agent
/// </summary>
namespace SoFunny.FunnyDB {
    internal sealed class FunnyDBAgent {
        private const string mFunnyVersion = "0.1.0";
        private static bool mIsInit = false;
        private const string mLogTag = "FunnyDBLog";

        /// <summary> 
        /// FunnyDB Initialize
        /// </summary>
        internal static void Initialize(string accessKeyId, string accessKeySecret, string endPoint) {

            if (string.IsNullOrEmpty(accessKeyId)) {
                return;
            }
            if (string.IsNullOrEmpty(accessKeySecret)) {
                return;
            }

            if (!mIsInit) {
                _initialize(accessKeyId, accessKeySecret, endPoint);
                mIsInit = true;
            }
            else {
                Logger.LogError("can't initialize more than once");
            }

        }

        /// <summary>
        /// Set SDK Status
        /// </summary>
        /// <param name="status"></param>
        internal static void SetSDKStatus(int status) {
            if (!mIsInit) {
                return;
            }
            _setSDKStatus(status);
        }

        /// <summary>
        /// Set SDK SendType
        /// </summary>
        /// <param name="status"></param>
        internal static void SetSDKSendType(int sendType) {
            if (!mIsInit) {
                return;
            }
            _setSDKSendType(sendType);
        }

        /// <summary>
        /// Set UserId
        /// </summary>
        /// <param name="userId"></param>
        internal static void SetUserId(string userId) {
            _setUserId(userId);
        }

        /// <summary>
        /// Set Channel
        /// </summary>
        /// <param name="channel"></param>
        internal static void SetChannel(string channel) {
            _setChannel(channel);
        }

        /// <summary>
        /// Set DeviceId
        /// </summary>
        /// <param name="deviceId"></param>
        internal static void SetDeviceId(string deviceId) {
            _setDeviceId(deviceId);
        }

        internal static string GetDeviceId() {
            return _getDeviceId();
        }

        /// <summary>
        /// Report Interval
        /// </summary>
        /// <param name="interval">interval</param> // ms
        internal static void SetReportInterval(int interval) {
            if (!mIsInit) {
                return;
            }
            _setReportInterval(interval);
        }

        /// <summary>
        /// Report Limit
        /// </summary>
        /// <param name="frameRate">limit</param>
        internal static void SetReportLimit(int limit) {
            if (!mIsInit) {
                return;
            }
            _setReportLimit(limit);
        }

        internal static void ReportCustom(int customType, int operateType, string jsonStr) {
            if (!mIsInit) {
                return;
            }
            _reportCustom(customType, operateType, jsonStr);
        }

        /// <summary>
        /// Report Event
        /// </summary>
        /// <param name="eventName">event Name</param>
        /// <param name="customProperty">custom Property</param>
        internal static void ReportEvent(string eventName, string customProperty = "") {
            if (!mIsInit) {
                return;
            }
            _reportEvent(eventName, customProperty);
        }

        /// </summary>
        /// Flush
        /// </summary>
        internal static void Flush() {
            if (!mIsInit) {
                return;
            }
            _flush();
        }

        internal static void EnableDebug() {
            _enableDebug();
        }

        internal static void SetOAIDFileData(string data) {
#if UNITY_ANDROID
            setOAIDCertInfo(data);
#endif
        }

        internal static void SetOAIDFileName(string name) {
#if UNITY_ANDROID
            setOAIDCertAssetName(name);
#endif
        }

        internal static void ShowToast(string msg) {
#if UNITY_ANDROID
            showToast(msg);
#endif
        }

#if (UNITY_EDITOR || UNITY_STANDALONE)

        private static void _initialize(string accessKeyId, string accessKeySecret, string endPoint) {
        FunnyDBEditor.Instance().Initialize(accessKeyId, accessKeySecret, endPoint);
    }

    private static void setOAIDCertInfo(string data) {
        Logger.Log("Editor 不支持该方法");
    }

    private static void setOAIDCertAssetName(string name) {
        Logger.Log("Editor 不支持该方法");
    }

    private static void _setSDKStatus(int status) {
        FunnyDBEditor.Instance().SetSDKStatus(status);
    }

    private static void _setSDKSendType(int sendType) {
        FunnyDBEditor.Instance().SetSDKSendType(sendType);
    }

    private static void _setUserId(string userId) {
        FunnyDBEditor.Instance().SetUserId(userId);
    }

    private static void _setChannel(string channel) {
        FunnyDBEditor.Instance().SetChannel(channel);
    }
    
    private static void _setDeviceId(string deviceId) {
        FunnyDBEditor.Instance().SetDeviceId(deviceId);
    }

    private static string _getDeviceId() {
        return FunnyDBEditor.Instance().GetDeviceId();
    }

    private static void _reportEvent(string eventName, string customProperty = "") {
        FunnyDBEditor.Instance().ReportEvent(eventName, customProperty);
    }

    private static void _setReportInterval(int pingNum) {
        FunnyDBEditor.Instance().SetReportInterval(pingNum);
    }

    private static void _setReportLimit(int limit) {
        FunnyDBEditor.Instance().SetReportLimit(limit);
    }

    private static void _flush() {
        FunnyDBEditor.Instance().Flush();
    }

    private static void _reportCustom(int customType, int operateType, string jsonStr) {
        FunnyDBEditor.Instance().ReportCustom(customType, operateType, jsonStr);
    }

    private static void _enableDebug() {
        FunnyDBEditor.Instance().EnableDebug();
    }

    private static void showToast(string msg) {
        Logger.Log($"Toast Message - {msg}");
    }
#elif UNITY_ANDROID
    // Get FunnyBridge
    private static readonly string FUNNY_BRIDGE_CLASS = "com.sofunny.eventAnalyzer.FunnyBridge";
    private static AndroidJavaObject mFunnyBridge;
    internal static AndroidJavaObject FunnyBridge {
        get {
            if (mFunnyBridge == null) {
                mFunnyBridge = new AndroidJavaObject(FUNNY_BRIDGE_CLASS);
            }
            return mFunnyBridge;
        }
    }
    private static AndroidJavaObject mFunnyInstance;
    internal static AndroidJavaObject FunnyInstance {
        get {
            if (mFunnyInstance == null) {
                mFunnyInstance = FunnyBridge.CallStatic<AndroidJavaObject>("getInstance");
            }
            return mFunnyInstance;
        }
    }
    // Get Unity Activity
    private static readonly string UNTIFY_CLASS = "com.unity3d.player.UnityPlayer";
    private static AndroidJavaClass mUnityClass;
    internal static AndroidJavaClass UnityClass {
        get {
            if (null == mUnityClass) {
                mUnityClass = new AndroidJavaClass(UNTIFY_CLASS);
            }
            return mUnityClass;
        }
    }
    private static AndroidJavaObject mUnityActivity;
    internal static AndroidJavaObject UnityActivity {
        get {
            if (null == mUnityActivity) {
                mUnityActivity = UnityClass.GetStatic<AndroidJavaObject>("currentActivity");
            }
            return mUnityActivity;
        }
    }

    private static int _initialize(string accessKeyId, string accessKeySecret, string endPoint) {
        int flag = 0;
        try {
            flag = FunnyInstance.Call<int>("initialize", UnityActivity, accessKeyId, accessKeySecret, endPoint);
        } catch (Exception e) {
            flag = -1;
            Logger.LogError("called initialize error: " + e);
        }
        return flag;
    }

    private static void _setSDKStatus(int status) {
        try {
            FunnyInstance.Call("setSDKStatus", status);
        } catch (Exception e) {
             Logger.LogError("called setSDKStatus error: " + e);
        }
    }

    private static void _setSDKSendType(int sendType) {
        try {
            FunnyInstance.Call("setSDKSendType", sendType);
        } catch (Exception e) {
             Logger.LogError("called setSDKSendType error: " + e);
        }
    }

    private static void _setUserId(string userId) {
        try {
            FunnyInstance.Call("setUserId", userId);
        } catch (Exception e) {
             Logger.LogError("called setUserId error: " + e);
        }
    }

    private static void _setChannel(string channel) {
        try {
            FunnyInstance.Call("setChannel", channel);
        } catch (Exception e) {
            Logger.LogError("called setChannel error: " + e);
        }
    }

    private static void _setDeviceId(string deviceId) {
        try {
            FunnyInstance.Call("setDeviceId", UnityActivity, deviceId);
        } catch (Exception e) {
            Logger.LogError("called setDeviceId error: " + e);
        }
    }

    private static string _getDeviceId() {
        try {
            return FunnyInstance.Call<string>("getDeviceId", UnityActivity);
        } catch (Exception e) {
            Logger.LogError("called setDeviceId error: " + e);
            return string.Empty;
        }
    }

    private static void _reportEvent(string eventName, string customProperty = "") {
        try {
            FunnyInstance.Call("reportEvent", eventName, customProperty);
        } catch (Exception e) {
            Logger.LogError("called reportEvent error: " + e);
        }
    }

    private static void _setReportInterval(int interval) {
        try {
            FunnyInstance.Call("setReportInterval", interval);
        } catch (Exception e) {
            Logger.LogError("called setReportInterval error: " + e);
        }
    }

    private static void _setReportLimit(int limit) {
        try {
            FunnyInstance.Call("setReportLimit", limit);
        } catch (Exception e) {
            Logger.LogError("called setReportLimit error: " + e);
        }
    }

    private static void _flush() {
        try {
            FunnyInstance.Call("flush");
        } catch (Exception e) {
            Logger.LogError("called report error: " + e);
        }
    }

    private static void _reportCustom(int customType, int operateType, string jsonStr) {
        try {
            FunnyInstance.Call("reportCustom", customType, operateType, jsonStr);
        } catch (Exception e) {
            Logger.LogError("called reportCustom error: " + e);
        }
    }

    private static void _enableDebug() {
        try {
            FunnyInstance.Call("enableDebug");
        } catch (Exception e) {
            Logger.LogError("called enableDebug error: " + e);
        }
    }

    internal static void getUniqueIdentify() {
        FunnyInstance.Call("getUniqueIdentify", 1);
        FunnyInstance.Call("getUniqueIdentify", 2);
        FunnyInstance.Call("getUniqueIdentify", 3);
        FunnyInstance.Call("getUniqueIdentify", 4);
     }

    private static void setOAIDCertInfo(string certInfo) {
        FunnyInstance.Call("setOAIDCerInfo", certInfo);
    }

    private static void setOAIDCertAssetName(string certAssetName) {
        FunnyInstance.Call("setOAIDCerAssetName", certAssetName);
    }

    private static void showToast(string msg) {
        FunnyInstance.Call("showToast", UnityActivity, msg);
    }
    

#elif (UNITY_IOS || UNITY_IPHONE)
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
    private static int _initialize(string accessKeyId, string accessKeySecret, string endPoint) {
        int flag = initialize(accessKeyId, accessKeySecret, endPoint);
        return flag;
    }

    private static void _setSDKStatus(int status) {
        setSDKStatus(status);
    }

    private static void _setSDKSendType(int sendType) {
        setSDKSendType(sendType);
    }

    private static void _setUserId(string userId) {
        setUserId(userId);
    }

    private static void _setChannel(string channel) {
        setChannel(channel);
    }

    private static void _setDeviceId(string deviceId) {
        setDeviceId(deviceId);
    }

    private static string _getDeviceId() {
        return getDeviceId();
    }

    private static void _reportEvent(string eventName, string customProperty = "") {
        reportEvent(eventName, customProperty);
    }

    private static void _setReportInterval(int interval) {
        setReportInterval(interval);
    }

    private static void _setReportLimit(int limit) {
        setReportLimit(limit);
    }

    private static void _flush() {
        flush();
    }

    private static void _reportCustom(int customType, int operateType, string jsonStr) {
        reportCustom(customType, operateType, jsonStr);
    }

    private static void _enableDebug() {
        enableDebug();
    }

#endif
    }
}
