using System;
using SoFunny.FunnyDB.Bridge;
using static SoFunny.FunnyDB.PC.EnumConstants;

/// <summary>
/// FunnyDB SDK
/// </summary>
namespace SoFunny.FunnyDB
{

    public static class FunnyDBSDK
    {

        /// <summary>
        /// 初始化完成事件(bool = 是否成功，string = 消息信息)
        /// </summary>
        public static event Action<bool, string> OnInitCompleted;

        /// <summary>
        /// 初始化方法，需要传入 FunnyFBConfig
        /// </summary>
        /// <param name="config"></param>
        public static void Initialize(FunnyDBConfig config)
        {
            // 调用初始化方法
            FunnyDBAgent.Initialize(config);

#if UNITY_IOS || UNITY_STANDALONE || UNITY_EDITOR
            AttackInitEvent(true, "初始化成功");
#endif
        }

        internal static void AttackInitEvent(bool isSuccess, string message)
        {
            OnInitCompleted?.Invoke(isSuccess, message);
        }

        /// <summary>
        /// FunnyDB 初始化方法
        /// </summary>
        /// <param name="keyID"></param>
        /// <param name="keySecret"></param>
        /// <param name="endPoint"></param>
        [Obsolete("请使用 FunnyDBSDK.Initialize(FunnyDBConfig config) 方法")]
        public static void Initialize(string keyID, string keySecret) {
            FunnyDBAgent.Initialize(new FunnyDBConfig(keyID, keySecret));
        }

        /// <summary>
        /// FunnyDB 初始化 (设置 endPoint)
        /// </summary>
        /// <param name="keyID"></param>
        /// <param name="keySecret"></param>
        /// <param name="endPoint"></param>
        [Obsolete("请使用 FunnyDBSDK.Initialize(FunnyDBConfig config) 方法")]
        public static void Initialize(string keyID, string keySecret, string endPoint) {
            FunnyDBConfig config = new FunnyDBConfig(keyID, keySecret);
            config.SetEndPoint(endPoint);
            FunnyDBAgent.Initialize(config);
        }

        /// <summary>
        /// 设置 SDK 状态
        /// </summary>
        /// <param name="status"></param>
        public static void SetSDKStatus(DBSDK_STATUS_ENUM status) {
            FunnyDBAgent.SetSDKStatus((int)status);
        }

        /// <summary>
        /// 设置 SDK 上报类型
        /// </summary>
        /// <param name="sendType"></param>
        public static void SetSDKSendType(DBSDK_SEND_TYPE_ENUM sendType) {
            FunnyDBAgent.SetSDKSendType((int)sendType);
        }

        /// <summary>
        /// 设置用户标识（必接，可任意阶段调用）
        /// </summary>
        /// <param name="userId"></param>
        public static void SetUserId(string userId) {
            FunnyDBAgent.SetUserId(userId);
        }

        /// <summary>
        /// 清除用户标识
        /// </summary>
        public static void ClearUserId() {
            FunnyDBAgent.SetUserId("");
        }

        /// <summary>
        /// 设置渠道信息（可任意阶段调用）
        /// </summary>
        /// <param name="channel"></param>
        [Obsolete("请使用 FunnyConfig 类进行设置", true)]
        public static void SetChannel(string channel) {
            FunnyDBAgent.SetChannel(channel);
        }

        /// <summary>
        /// 设置设备 Id （需在初始化前调用）
        /// </summary>
        /// <param name="deviceId"></param>
        [Obsolete("请使用 FunnyConfig 类进行设置",true)]
        public static void SetDeviceId(string deviceId) {
            FunnyDBAgent.SetDeviceId(deviceId);
        }

        public static string GetDeviceId() {
            return FunnyDBAgent.GetDeviceId();
        }

        /// <summary>
        /// 设置上报间隔
        /// </summary>
        /// <param name="interval"></param>
        public static void SetReportInterval(int interval) {
            FunnyDBAgent.SetReportInterval(interval);
        }

        /// <summary>
        /// 设置每次上报条数上限
        /// </summary>
        /// <param name="limit"></param>
        public static void SetReportLimit(int limit) {
            FunnyDBAgent.SetReportLimit(limit);
        }

        /// <summary>
        /// 立即上报一批数据
        /// </summary>
        public static void Flush() {
            FunnyDBAgent.Flush();
        }

        /// <summary>
        /// 开启调试阶段相关功能，如辅助日志等（线上需关闭，减少无关消耗）（可任意阶段调用）
        /// </summary>
        [Obsolete("请改用加自定义宏 ENABLE_FUNNYDB_DEBUG 方式开启 Debug", true)]
        public static void EnableDebug() {
            FunnyDBAgent.EnableDebug();
        }

        public static void ShowFDBToast(string message) {
            FunnyDBAgent.ShowToast(message);
        }
    }
}