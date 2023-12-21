#if UNITY_STANDALONE || UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace SoFunny.FunnyDB.PC
{
    internal class CrashCollectHandler
    {

        internal void Init()
        {
            Application.logMessageReceived += logReceiver;
            AppDomain.CurrentDomain.UnhandledException += uncaughtExceptionHandler;
        }

        private void uncaughtExceptionHandler(object sender, System.UnhandledExceptionEventArgs args)
        {
            if (args == null || args.ExceptionObject == null)
            {
                return;
            }

            try
            {
                if (args.ExceptionObject.GetType() != typeof(Exception))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            Exception e = (Exception)args.ExceptionObject;

            string crashReason = "Exception: " + e.GetType().Name + " <br> " + e.Message + " <br> " + e.StackTrace + " <br> ";
            AppCrashEvent.Track(crashReason);
        }

        private void logReceiver(string logString, string stackTrace, LogType logType)
        {
            if (logType == LogType.Exception || logType == LogType.Error || logType == LogType.Assert)
            {
                string reasonStr = "exception_type: " + logType.ToString() + " <br> " + "exception_message: " + logString + " <br> " + "stack_trace: " + stackTrace + " <br> ";
                AppCrashEvent.Track(reasonStr);
            }
        }
    }
}
#endif
