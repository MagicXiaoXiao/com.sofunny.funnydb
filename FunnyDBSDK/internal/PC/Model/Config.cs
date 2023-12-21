#if UNITY_STANDALONE || UNITY_EDITOR
using System;
using static SoFunny.FunnyDB.PC.EnumConstants;

namespace SoFunny.FunnyDB.PC
{
    internal class AccessInfo
    {
        internal string _accessSecet;
        internal string _accessKeyId;
        internal string _endPoint;

        internal string AccessSecret
        {
            get { return _accessSecet; }
            set { _accessSecet = value; }
        }
        internal string AccessKeyId
        {
            get { return _accessKeyId; }
            set { _accessKeyId = value; }
        }

        internal string EndPoint
        {
            get { return _endPoint; }
            set { _endPoint = value; }
        }

        public AccessInfo(string accessKeyId, string accessSecret, string endPoint)
        {
            AccessKeyId = accessKeyId;
            AccessSecret = accessSecret;
            EndPoint = endPoint;
        }
    }

    internal static class ReportSettings
    {
        private static int _sdkStatus;
        public static int SendType = (int)DBSDK_SEND_TYPE_ENUM.NOW;
        /// <summary>
        /// 单位: 秒
        /// </summary>
        public static int ReportInterval = Constants.DEFAULT_REPORT_INTERVAL / 1000;
        public static int ReportSizeLimit = Constants.DEFAULT_REPORT_SIZE_LIMIT;

        public static int SdkStatus
        {
            private get { return _sdkStatus; }
            set { _sdkStatus = value; }
        }

        static ReportSettings()
        {
            SdkStatus = (int)DBSDK_STATUS_ENUM.DEFAULT;
        }

        // DEFAULT or ONLY_COLLECT => collect
        public static bool CanCollect()
        {
            return SdkStatus == ((int)DBSDK_STATUS_ENUM.DEFAULT) || SdkStatus == ((int)DBSDK_STATUS_ENUM.ONLY_COLLECT);
        }

        // DEFAULT or STOP_COLLECT(already get data) => send
        // FIXME 控制逻辑有争议，与产品确认下
        // 1、具体实现逻辑
        // 2、管控方面
        public static bool CanSend()
        {
            return SdkStatus == ((int)DBSDK_STATUS_ENUM.DEFAULT) || SdkStatus == ((int)DBSDK_STATUS_ENUM.STOP_COLLECT);
        }

        // ONLY_COLLECT => save the data directly into DB:
        public static bool CanToDB()
        {
            return SdkStatus == ((int)DBSDK_STATUS_ENUM.ONLY_COLLECT);
        }
    }

}
#endif