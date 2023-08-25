using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SoFunny.FunnyDB
{
    internal static partial class Logger
    {
        internal const string k_Tag = "[ FunnyDB ]";

        internal const string k_GlobalVerboseLoggingDefine = "ENABLE_FUNNYDB_DEBUG";
        internal const string k_GlobalLotsLogsVerboseDefine = "ENABLE_FUNNYDB_DEBUG_LOTS_LOGS";

        [Conditional(k_GlobalVerboseLoggingDefine)]
        internal static void Log(object message, ColorStyle style = ColorStyle.Normal)
        {
#if UNITY_EDITOR
            switch (style)
            {
                case ColorStyle.Normal:
                    Debug.unityLogger.Log(k_Tag, $"<color=#F8F8FF>{message}</color>");
                    break;
                case ColorStyle.Green:
                    Debug.unityLogger.Log(k_Tag, $"<color=#ADFF2F>{message}</color>");
                    break;
                case ColorStyle.Red:
                    Debug.unityLogger.Log(k_Tag, $"<color=#FF6347>{message}</color>");
                    break;
                case ColorStyle.Blue:
                    Debug.unityLogger.Log(k_Tag, $"<color=#1E90FF>{message}</color>");
                    break;
                default:
                    Debug.unityLogger.Log(k_Tag, message);
                    break;
            }
#else
            Debug.unityLogger.Log(k_Tag, message);
#endif
        }

        [Conditional(k_GlobalVerboseLoggingDefine)]
        internal static void LogWarning(object message) => Debug.unityLogger.LogWarning(k_Tag, message);

        [Conditional(k_GlobalVerboseLoggingDefine)]
        internal static void LogError(this object message)
        {

#if UNITY_EDITOR
            Debug.unityLogger.Log(k_Tag, $"<color=red>{message}</color>");
#else
            Debug.unityLogger.Log(k_Tag, message);
#endif
        }

        [Conditional(k_GlobalVerboseLoggingDefine)]
        internal static void LogException(this Exception exception) => Debug.unityLogger.Log(LogType.Exception, k_Tag, exception);

        [Conditional(k_GlobalLotsLogsVerboseDefine)]
        internal static void LogVerbose(object message) => Debug.unityLogger.Log(k_Tag, message);


        internal enum ColorStyle
        {
            Normal,
            Green,
            Red,
            Blue,
        }

    }
}

