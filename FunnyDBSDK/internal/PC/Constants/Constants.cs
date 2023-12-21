#if UNITY_STANDALONE || UNITY_EDITOR
namespace SoFunny.FunnyDB.PC
{
    internal sealed class Constants {
        internal static string FUNNY_DB_VERSION = "0.8.3";

        // Key
        internal static string KEY_TIME = "#time";
        internal static string KEY_LOG_ID = "#log_id";
        internal static string KEY_IDENTIFY = "#identify";
        internal static string KEY_NETWORK = "#network";
        internal static string KEY_EVENT = "#event";
        internal static string KEY_DEVICE_ID = "#device_id";
        internal static string KEY_USER_ID = "#user_id";
        internal static string KEY_CHANNEL = "#channel";
        internal static string KEY_SDK_TYPE = "#sdk_type";
        internal static string KEY_SDK_VERSION = "#sdk_version";
        internal static string KEY_DEVICE_MODEL = "#device_model";
        internal static string KEY_DEVICE_NAME = "#device_name";
        internal static string KEY_MANUFACTURER = "#manufacturer";
        internal static string KEY_SCREEN_HEIGHT = "#screen_height";
        internal static string KEY_SCREEN_WIDTH = "#screen_width";
        internal static string KEY_OS = "#os";
        internal static string KEY_OS_PLATFORM = "#os_platform";
        internal static string KEY_OS_VERSION = "#os_version";
        internal static string KEY_CARRIER = "#carrier";
        internal static string KEY_TYPE = "type";
        internal static string KEY_DATA = "data";
        internal static string KEY_MESSAGES = "messages";
        internal static string KEY_OPERATE = "#operate";
        internal static string KEY_PROPERTY = "properties";

        internal static string KEY_DB_ID = "id";
        internal static string KEY_DB_TIME = "time";
        internal static string KEY_DB_DATA = "data";

        public static string KEY_SYSTEM_LANGUAGE = "#system_language";
        public static string KEY_ZONE_OFFSET = "#zone_offset";

        // Value
        internal static string VALUE_UNKNOWN = "";
        internal static string VALUE_METHOD = "POST";
        internal static string VALUE_API_URL = "/v1/collect";
        internal static string VALUE_SDK_TYPE = "Unity";
        internal static string VALUE_WINDOWS = "windows";
        internal static string VALUE_OS_PLATFORM  = "Android";
        internal static string VALUE_EVENT = "Event";
        internal static string VALUE_USER_MUTATION = "UserMutation";
        internal static string VALUE_DEVICE_MUTATION = "DeviceMutation";
        internal static string VALUE_SET = "set";
        internal static string VALUE_SET_ONCE = "setOnce";
        internal static string VALUE_ADD = "add";

        internal static int MAX_CONCURRENCY = 2;

        internal static int DEFAULT_REPORT_INTERVAL = 15000;                    // 默认每 15 秒发起一次上报
        internal static int DEFAULT_REPORT_SIZE_LIMIT = 20;                     // 默认每次上报请求最多包含 20 条数据, 最大值不超过 50, 因为 Message 默认最大值为 50
        internal static int DEFAULT_REPORT_SIZE_MAX_LIMIT = 50;
        internal static int MAX_REPORT_INTERVAL_LIMIT = 60 * 1000;              // 上报间隔最大值为 60。超过该值则设置为 60

        // Send Type
        /// <summary>
        /// 未指定，根据 EventUpload.CurSendType 来上报，通过接口设置或者程序默认值
        /// </summary>
        internal const int FUNNY_DB_SEND_TYPE_NONE = -1;
        internal static readonly string[] NTP_SERVERS = new string[] { "ntp.aliyun.com", "time.android.com", "cn.pool.ntp.org" };
        internal static int FUNNY_DB_SEND_FLUSH = 3;

        // 自动采集事件名称
        internal static string REPORT_EVENT_INSTALL_NAME = "#app_install";
        internal static string REPORT_EVENT_START_NAME = "#app_start";
        internal static string REPORT_EVENT_FOREGROUND_NAME = "#app_foreground";
        internal static string REPORT_EVENT_BACKGROUND_NAME = "#app_background";
        internal static string REPORT_EVENT_CRASH_NAME = "#app_crash";
        internal static string REPORT_EVENT_APP_KILL_NAME = "#app_kill";
        internal static string REPORT_EVENT_DEVICE_LOGIN_NAME = "#device_login";

        internal static  string KEY_INSTALL_TIME = "#install_time";
        internal static  string KEY_APP_START_RESUME_FROM_BACKGROUND = "#resume_from_background";
        internal static  string KEY_APP_START_REASON = "#start_reason";
        internal static  string KEY_APP_START_BACKGROUND_DURATION = "#background_duration";
        internal static  string KEY_APP_END_DURATION = "#duration";
        internal static  string KEY_APP_CRASH_REASON = "#app_crashed_reason";
        internal static  string KEY_APP_KILL_TIME = "#kill_time";

        internal static string KEY_RAM_CAPACITY = "#ram_capacity";
        internal static string KEY_CPU_MODEL = "#cpu_model";
        internal static string KEY_CPU_CORE_COUNT = "#cpu_core_count";
        internal static string KEY_CPU_FREQUENCY = "#cpu_frequency";

        internal static string SP_KEY_FIRST_INSTALL_TIME = "app_first_install_time"; // 应用安装时间

        // 上报渠道
        internal enum ReportChannel
        {
            ChannelTypeSDK = 1,
            ChannelTypePrj = 0
        }
    }
}
#endif