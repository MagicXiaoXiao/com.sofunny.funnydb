using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// FunnyDB SDK
/// </summary>
namespace SoFunny.FunnyDB {

    public enum DBSDK_STATUS_ENUM {
        /// <summary>
        /// 正常上报状态（默认）
        /// </summary>
        DEFAULT = 1,
        /// <summary>
        /// 立即暂停采集数据，不影响未上报数据的正常流程
        /// </summary>
        STOP_COLLECT = 2,
        /// <summary>
        /// 只采集不上报数据，数据直接入库
        /// </summary>
        ONLY_COLLECT = 3,
    }
    public enum DBSDK_SEND_TYPE_ENUM {
        /// <summary>
        /// 实时上报（默认）
        /// </summary>
        NOW = 1,
        /// <summary>
        /// 批量延迟
        /// </summary>
        DELAY = 2,
    }
    public enum DBSDK_CUSTOM_TYPE_ENUM {
        /// <summary>
        /// 用户
        /// </summary>
        USER = 1,
        /// <summary>
        /// 设备
        /// </summary>
        DEVICE = 2,
    }
    public enum DBSDK_OPERATE_TYPE_ENUM {
        /// <summary>
        /// 设置
        /// </summary>
        SET = 1,
        /// <summary>
        /// 新增
        /// </summary>
        ADD = 2,
        /// <summary>
        /// 唯一设置
        /// </summary>
        SET_ONCE = 3,
    }

    public class FunnyDBSDK {

        /// <summary>
        /// 初始化方法，需要传入 FunnyFBConfig
        /// </summary>
        /// <param name="config"></param>
        public static void Initialize(FunnyDBConfig config) {
            // 设置设备号
            if (!string.IsNullOrEmpty(config.deviceID)) {
                FunnyDBAgent.SetDeviceId(config.deviceID);
            }
            // 设置渠道
            if (!string.IsNullOrEmpty(config.channel)) {
                FunnyDBAgent.SetChannel(config.channel);
            }

            // Android 设置 OAID
            if (config.oaidEnable) {
                switch (config.oaidType) {
                    case FDB_OAID_TYPE.CertData:
                        FunnyDBAgent.SetOAIDFileData(config.oaidData);
                        break;
                    case FDB_OAID_TYPE.CertFileName:
                        FunnyDBAgent.SetOAIDFileName(config.oaidData);
                        break;
                    default:break;
                }
            }

            // 调用初始化方法
            FunnyDBAgent.Initialize(config.keyID, config.keySecret, config.endPoint);
        }


        /// <summary>
        /// FunnyDB 初始化方法
        /// </summary>
        /// <param name="keyID"></param>
        /// <param name="keySecret"></param>
        /// <param name="endPoint"></param>
        [Obsolete("请使用 FunnyDBSDK.Initialize(FunnyDBConfig config) 方法")]
        public static void Initialize(string keyID, string keySecret) {
            FunnyDBAgent.Initialize(keyID, keySecret, "");
        }

        /// <summary>
        /// FunnyDB 初始化 (设置 endPoint)
        /// </summary>
        /// <param name="keyID"></param>
        /// <param name="keySecret"></param>
        /// <param name="endPoint"></param>
        [Obsolete("请使用 FunnyDBSDK.Initialize(FunnyDBConfig config) 方法")]
        public static void Initialize(string keyID, string keySecret, string endPoint) {
            FunnyDBAgent.Initialize(keyID, keySecret, endPoint);
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
        public static void EnableDebug() {
            FunnyDBAgent.EnableDebug();
        }

        public static void ShowFDBToast(string message) {
            FunnyDBAgent.ShowToast(message);
        }

    }
}