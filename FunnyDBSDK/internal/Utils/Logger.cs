using System;
using System.Diagnostics;
using System.Threading;
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
            string finalMsg = $"Thread={Thread.CurrentThread.ManagedThreadId} - {message}";
#if UNITY_EDITOR
            switch (style)
            {
                case ColorStyle.Normal:
                    Debug.unityLogger.Log(k_Tag, $"<color=#F8F8FF>{finalMsg}</color>");
                    break;
                case ColorStyle.Green:
                    Debug.unityLogger.Log(k_Tag, $"<color=#ADFF2F>{finalMsg}</color>");
                    break;
                case ColorStyle.Red:
                    Debug.unityLogger.Log(k_Tag, $"<color=#FF6347>{finalMsg}</color>");
                    break;
                case ColorStyle.Blue:
                    Debug.unityLogger.Log(k_Tag, $"<color=#1E90FF>{finalMsg}</color>");
                    break;
                default:
                    Debug.unityLogger.Log(k_Tag, finalMsg);
                    break;
            }
#else
            Debug.unityLogger.Log(k_Tag, finalMsg);
#endif
        }

        [Conditional(k_GlobalVerboseLoggingDefine)]
        internal static void LogWarning(object message) {
            string finalMsg = $"Thread={Thread.CurrentThread.ManagedThreadId} - {message}";
            Debug.unityLogger.LogWarning(k_Tag, finalMsg);
        }

        [Conditional(k_GlobalVerboseLoggingDefine)]
        internal static void LogError(this object message)
        {
            string finalMsg = $"Thread={Thread.CurrentThread.ManagedThreadId} - {message}";
#if UNITY_EDITOR
            Debug.unityLogger.Log(k_Tag, $"<color=red>{finalMsg}</color>");
#else
            Debug.unityLogger.Log(k_Tag, finalMsg);
#endif
        }

        [Conditional(k_GlobalVerboseLoggingDefine)]
        internal static void LogException(this Exception exception)
        {
            string finalMsg = $"Thread={Thread.CurrentThread.ManagedThreadId} - {exception}";
            Debug.unityLogger.Log(LogType.Exception, k_Tag, finalMsg);
        }

        [Conditional(k_GlobalLotsLogsVerboseDefine)]
        internal static void LogVerbose(object message) {
            string finalMsg = $"Thread={Thread.CurrentThread.ManagedThreadId} - {message}";
            Debug.unityLogger.Log(k_Tag, finalMsg);
        }


        internal enum ColorStyle
        {
            Normal,
            Green,
            Red,
            Blue,
        }

    }
}

