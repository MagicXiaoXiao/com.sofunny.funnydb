using System.Collections.Generic;
using SoFunny.FunnyDB.Bridge;
using static SoFunny.FunnyDB.PC.EnumConstants;

namespace SoFunny.FunnyDB
{

    public partial class FDBEvent
    {

        /// <summary>
        /// 设置用户属性值
        /// </summary>
        /// <param name="customProperties"></param>
        public static void ReportSetUser(Dictionary<string, object> customProperties)
        {
            if (null == customProperties)
            {
                return;
            }
            FunnyReportVerifyUtils.VerifyDictionaryValue(customProperties);
            string userCustom = JsonWriterUtils.ConvertDictionaryToJson(customProperties);
            FunnyDBAgent.ReportCustom((int)DBSDK_CUSTOM_TYPE_ENUM.USER, (int)DBSDK_OPERATE_TYPE_ENUM.SET, userCustom);
        }

        /// <summary>
        /// 设置唯一用户属性 <br/> (对应参数只允许设置一次)
        /// </summary>
        /// <param name="customProperties"></param>
        public static void ReportSetOnceUser(Dictionary<string, object> customProperties)
        {
            if (customProperties == null)
            {
                return;
            }
            FunnyReportVerifyUtils.VerifyDictionaryValue(customProperties);
            string userCustom = JsonWriterUtils.ConvertDictionaryToJson(customProperties);
            FunnyDBAgent.ReportCustom((int)DBSDK_CUSTOM_TYPE_ENUM.USER, (int)DBSDK_OPERATE_TYPE_ENUM.SET_ONCE, userCustom);
        }

        /// <summary>
        /// 添加用户属性值
        /// </summary>
        /// <param name="customProperties"></param>
        public static void ReportAddUser(Dictionary<string, object> customProperties)
        {
            if (null == customProperties)
            {
                return;
            }
            FunnyReportVerifyUtils.VerifyDictionaryValue(customProperties);
            string userCustom = JsonWriterUtils.ConvertDictionaryToJson(customProperties);
            FunnyDBAgent.ReportCustom((int)DBSDK_CUSTOM_TYPE_ENUM.USER, (int)DBSDK_OPERATE_TYPE_ENUM.ADD, userCustom);
        }

        /// <summary>
        /// 设置设备属性值
        /// </summary>
        /// <param name="customProperties"></param>
        public static void ReportSetDevice(Dictionary<string, object> customProperties)
        {
            if (null == customProperties)
            {
                return;
            }
            FunnyReportVerifyUtils.VerifyDictionaryValue(customProperties);
            string deviceCustom = JsonWriterUtils.ConvertDictionaryToJson(customProperties);
            FunnyDBAgent.ReportCustom((int)DBSDK_CUSTOM_TYPE_ENUM.DEVICE, (int)DBSDK_OPERATE_TYPE_ENUM.SET, deviceCustom);
        }

        /// <summary>
        /// 设置唯一设备属性 <br/> (对应参数只允许设置一次)
        /// </summary>
        /// <param name="customProperties"></param>
        public static void ReportSetOnceDevice(Dictionary<string, object> customProperties)
        {
            if (customProperties == null)
            {
                return;
            }
            FunnyReportVerifyUtils.VerifyDictionaryValue(customProperties);
            string userCustom = JsonWriterUtils.ConvertDictionaryToJson(customProperties);
            FunnyDBAgent.ReportCustom((int)DBSDK_CUSTOM_TYPE_ENUM.DEVICE, (int)DBSDK_OPERATE_TYPE_ENUM.SET_ONCE, userCustom);
        }

        /// <summary>
        /// 添加设备属性值
        /// </summary>
        /// <param name="customProperties"></param>
        public static void ReportAddDevice(Dictionary<string, object> customProperties)
        {
            if (null == customProperties)
            {
                return;
            }
            FunnyReportVerifyUtils.VerifyDictionaryValue(customProperties);
            string deviceCustom = JsonWriterUtils.ConvertDictionaryToJson(customProperties);
            FunnyDBAgent.ReportCustom((int)DBSDK_CUSTOM_TYPE_ENUM.DEVICE, (int)DBSDK_OPERATE_TYPE_ENUM.ADD, deviceCustom);
        }
        /// <summary>
        /// 上报自定义事件
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="customProperties">参数表</param>
        public static void ReportEvent(string eventName, Dictionary<string, object> customProperties)
        {
            if (!FunnyReportVerifyUtils.VerifyEventName(eventName))
            {
                return;
            }

            string customStr = null;
            if (null != customProperties)
            {
                FunnyReportVerifyUtils.VerifyDictionaryValue(customProperties);
                customStr = JsonWriterUtils.ConvertDictionaryToJson(customProperties);
            }
            FunnyDBAgent.ReportEvent(eventName, customStr);
        }
    }
    /// <summary>
    /// 字符串方式传参
    /// </summary>
    public partial class FDBEvent
    {
        /// <summary>
        /// 设置用户属性值
        /// </summary>
        /// <param name="customProperties"></param>
        public static void ReportSetUser(string jsonStr)
        {
            if (string.IsNullOrEmpty(jsonStr))
            {
                return;
            }
            FunnyDBAgent.ReportCustom((int)DBSDK_CUSTOM_TYPE_ENUM.USER, (int)DBSDK_OPERATE_TYPE_ENUM.SET, jsonStr);
        }

        /// <summary>
        /// 设置唯一用户属性 <br/> (对应参数只允许设置一次)
        /// </summary>
        /// <param name="customProperties"></param>
        public static void ReportSetOnceUser(string jsonStr)
        {
            if (string.IsNullOrEmpty(jsonStr))
            {
                return;
            }
            FunnyDBAgent.ReportCustom((int)DBSDK_CUSTOM_TYPE_ENUM.USER, (int)DBSDK_OPERATE_TYPE_ENUM.SET_ONCE, jsonStr);
        }

        /// <summary>
        /// 添加用户属性值
        /// </summary>
        /// <param name="customProperties"></param>
        public static void ReportAddUser(string jsonStr)
        {
            if (string.IsNullOrEmpty(jsonStr))
            {
                return;
            }
            FunnyDBAgent.ReportCustom((int)DBSDK_CUSTOM_TYPE_ENUM.USER, (int)DBSDK_OPERATE_TYPE_ENUM.ADD, jsonStr);
        }

        /// <summary>
        /// 设置设备属性值
        /// </summary>
        /// <param name="customProperties"></param>
        public static void ReportSetDevice(string jsonStr)
        {
            if (string.IsNullOrEmpty(jsonStr))
            {
                return;
            }
            FunnyDBAgent.ReportCustom((int)DBSDK_CUSTOM_TYPE_ENUM.DEVICE, (int)DBSDK_OPERATE_TYPE_ENUM.SET, jsonStr);
        }

        /// <summary>
        /// 设置唯一设备属性 <br/> (对应参数只允许设置一次)
        /// </summary>
        /// <param name="customProperties"></param>
        public static void ReportSetOnceDevice(string jsonStr)
        {
            if (string.IsNullOrEmpty(jsonStr))
            {
                return;
            }
            FunnyDBAgent.ReportCustom((int)DBSDK_CUSTOM_TYPE_ENUM.DEVICE, (int)DBSDK_OPERATE_TYPE_ENUM.SET_ONCE, jsonStr);
        }

        /// <summary>
        /// 添加设备属性值
        /// </summary>
        /// <param name="customProperties"></param>
        public static void ReportAddDevice(string jsonStr)
        {
            if (string.IsNullOrEmpty(jsonStr))
            {
                return;
            }
            FunnyDBAgent.ReportCustom((int)DBSDK_CUSTOM_TYPE_ENUM.DEVICE, (int)DBSDK_OPERATE_TYPE_ENUM.ADD, jsonStr);
        }

        /// <summary>
        /// 直接给 Json 字符串上报自定义事件
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="customProperties">有效的 Json 字符串</param>
        public static void ReportEvent(string eventName, string jsonStr)
        {
            if (string.IsNullOrEmpty(jsonStr))
            {
                return;
            }
            FunnyDBAgent.ReportEvent(eventName, jsonStr);
        }
    }
}

