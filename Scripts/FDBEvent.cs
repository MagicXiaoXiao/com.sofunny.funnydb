using System.Collections;
using Newtonsoft.Json;


namespace SoFunny.FunnyDB {

    public class FDBEvent
    {

        /// <summary>
        /// 设置用户属性值
        /// </summary>
        /// <param name="customTable"></param>
        public static void ReportSetUser(Hashtable customTable)
        {
            if (null == customTable)
            {
                return;
            }
            FunnyReportVerifyUtils.VerifyHashTableValue(customTable);
            string userCustom = JsonConvert.SerializeObject(customTable);
            FunnyDBAgent.ReportCustom((int)DBSDK_CUSTOM_TYPE_ENUM.USER, (int)DBSDK_OPERATE_TYPE_ENUM.SET, userCustom);
        }

        /// <summary>
        /// 设置唯一用户属性 <br/> (对应参数只允许设置一次)
        /// </summary>
        /// <param name="customTable"></param>
        public static void ReportSetOnceUser(Hashtable customTable)
        {
            if (customTable == null)
            {
                return;
            }
            FunnyReportVerifyUtils.VerifyHashTableValue(customTable);
            string userCustom = JsonConvert.SerializeObject(customTable);
            FunnyDBAgent.ReportCustom((int)DBSDK_CUSTOM_TYPE_ENUM.USER, (int)DBSDK_OPERATE_TYPE_ENUM.SET_ONCE, userCustom);
        }

        /// <summary>
        /// 添加用户属性值
        /// </summary>
        /// <param name="customTable"></param>
        public static void ReportAddUser(Hashtable customTable)
        {
            if (null == customTable)
            {
                return;
            }
            string userCustom = JsonConvert.SerializeObject(customTable);
            FunnyDBAgent.ReportCustom((int)DBSDK_CUSTOM_TYPE_ENUM.USER, (int)DBSDK_OPERATE_TYPE_ENUM.ADD, userCustom);
        }

        /// <summary>
        /// 设置设备属性值
        /// </summary>
        /// <param name="customTable"></param>
        public static void ReportSetDevice(Hashtable customTable)
        {
            if (null == customTable)
            {
                return;
            }
            FunnyReportVerifyUtils.VerifyHashTableValue(customTable);
            string deviceCustom = JsonConvert.SerializeObject(customTable);
            FunnyDBAgent.ReportCustom((int)DBSDK_CUSTOM_TYPE_ENUM.DEVICE, (int)DBSDK_OPERATE_TYPE_ENUM.SET, deviceCustom);
        }

        /// <summary>
        /// 设置唯一设备属性 <br/> (对应参数只允许设置一次)
        /// </summary>
        /// <param name="customTable"></param>
        public static void ReportSetOnceDevice(Hashtable customTable)
        {
            if (customTable == null)
            {
                return;
            }
            FunnyReportVerifyUtils.VerifyHashTableValue(customTable);
            string userCustom = JsonConvert.SerializeObject(customTable);
            FunnyDBAgent.ReportCustom((int)DBSDK_CUSTOM_TYPE_ENUM.DEVICE, (int)DBSDK_OPERATE_TYPE_ENUM.SET_ONCE, userCustom);
        }

        /// <summary>
        /// 添加设备属性值
        /// </summary>
        /// <param name="customTable"></param>
        public static void ReportAddDevice(Hashtable customTable)
        {
            if (null == customTable)
            {
                return;
            }
            FunnyReportVerifyUtils.VerifyHashTableValue(customTable);
            string deviceCustom = JsonConvert.SerializeObject(customTable);
            FunnyDBAgent.ReportCustom((int)DBSDK_CUSTOM_TYPE_ENUM.DEVICE, (int)DBSDK_OPERATE_TYPE_ENUM.ADD, deviceCustom);
        }

        /// <summary>
        /// 上报自定义事件
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="customTable">参数表</param>
        public static void ReportEvent(string eventName, Hashtable customTable)
        {
            if(!FunnyReportVerifyUtils.VerifyEventName(eventName))
            {
                return;
            }
            FunnyReportVerifyUtils.VerifyHashTableValue(customTable);
            string custom = string.Empty;
            if (null != customTable)
            {
                custom = JsonConvert.SerializeObject(customTable);
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

