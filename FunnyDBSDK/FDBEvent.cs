using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using SoFunny.FunnyDB.Bridge;
using SoFunny.FunnyDB.PC;
using static SoFunny.FunnyDB.PC.EnumConstants;

namespace SoFunny.FunnyDB
{

    public class FDBEvent
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
            string userCustom = JsonConvert.SerializeObject(customProperties);
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
            string userCustom = JsonConvert.SerializeObject(customProperties);
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
            string userCustom = JsonConvert.SerializeObject(customProperties);
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
            string deviceCustom = JsonConvert.SerializeObject(customProperties);
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
            string userCustom = JsonConvert.SerializeObject(customProperties);
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
            string deviceCustom = JsonConvert.SerializeObject(customProperties);
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
            string custom = string.Empty;
            if (null != customProperties)
            {
                FunnyReportVerifyUtils.VerifyDictionaryValue(customProperties);
                custom = JsonConvert.SerializeObject(customProperties);
            }
            FunnyDBAgent.ReportEvent(eventName, custom);
        }


        /// <summary>
        /// 上报自定义事件，字符串形式
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="jsonString"></param>
        public static void ReportEvent(string eventName, string jsonString = "")
        {
            if (!FunnyReportVerifyUtils.VerifyEventName(eventName))
            {
                return;
            }
            FunnyDBAgent.ReportEvent(eventName, jsonString);
        }
    }

}

