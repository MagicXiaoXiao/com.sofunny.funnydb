#if (UNITY_EDITOR || UNITY_STANDALONE)

namespace SoFunny.FunnyDB {
    public sealed class Constants {
        public static string FUNNY_DB_VERSION = "0.1.0";

        // Key
        public static string KEY_TIME = "#time";
        public static string KEY_LOG_ID = "#log_id";
        public static string KEY_IDENTIFY = "#identify";
        public static string KEY_NETWORK = "#network";
        public static string KEY_EVENT = "#event";
        public static string KEY_DEVICE_ID = "#device_id";
        public static string KEY_USER_ID = "#user_id";
        public static string KEY_CHANNEL = "#channel";
        public static string KEY_SDK_TYPE = "#sdk_type";
        public static string KEY_SDK_VERSION = "#sdk_version";
        public static string KEY_DEVICE_MODEL = "#device_model";
        public static string KEY_MANUFACTURER = "#manufacturer";
        public static string KEY_SCREEN_HEIGHT = "#screen_height";
        public static string KEY_SCREEN_WIDTH = "#screen_width";
        public static string KEY_OS = "#os";
        public static string KEY_OS_PLATFORM = "#os_platform";
        public static string KEY_OS_VERSION = "#os_version";
        public static string KEY_CARRIER = "#carrier";
        public static string KEY_TYPE = "type";
        public static string KEY_DATA = "data";
        public static string KEY_MESSAGES = "messages";
        public static string KEY_OPERATE = "#operate";
        public static string KEY_PROPERTY = "properties";

        public static string KEY_DB_ID = "id";
        public static string KEY_DB_TIME = "time";
        public static string KEY_DB_DATA = "data";

        // Value
        public static string VALUE_UNKNOWN = "unknown";
        public static string VALUE_METHOD = "POST";
        public static string VALUE_API_URL = "/v1/collect";
        public static string VALUE_SDK_TYPE = "Unity";
        public static string VALUE_WINDOWS = "windows";
        public static string VALUE_OS_PLATFORM  = "Android";
        public static string VALUE_EVENT = "Event";
        public static string VALUE_USER_MUTATION = "UserMutation";
        public static string VALUE_DEVICE_MUTATION = "DeviceMutation";
        public static string VALUE_SET = "set";
        public static string VALUE_SET_ONCE = "setOnce";
        public static string VALUE_ADD = "add";
    }
}

#endif