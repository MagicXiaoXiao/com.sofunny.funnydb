#if UNITY_STANDALONE || UNITY_EDITOR
using System.Collections.Concurrent;
using Newtonsoft.Json;
using UnityEngine;

namespace SoFunny.FunnyDB.PC
{
    internal sealed class CacheMethod
    {
        private static ConcurrentQueue<CacheMethod> cacheMethods = new ConcurrentQueue<CacheMethod>();
        public string Method;
        public object[] MethodParams;

        public CacheMethod(string method, object[] methodParms)
        {
            Method = method;
            MethodParams = methodParms;
        }
        public static void ReportCacheEvents()
        {
            while (cacheMethods.TryDequeue(out var item))
            {
                switch (item.Method)
                {
                    case Constants.CACHE_METHOD_NAME_REPORT_EVENT:
                        FunnyDBPCInstance.Instance.ReportEvent((string)item.MethodParams[0], (string)item.MethodParams[1], (int)item.MethodParams[2], (int)item.MethodParams[3]);
                        break;
                    case Constants.CACHE_METHOD_NAME_REPORT_CUSTOM:
                        FunnyDBPCInstance.Instance.ReportCustom((int)item.MethodParams[0], (int)item.MethodParams[1], (string)item.MethodParams[2]);
                        break;
                }
            }
            Debug.Log("ReportCacheEvents after: " + JsonConvert.SerializeObject(cacheMethods));
        }

        public static void AddEvent(CacheMethod cacheMethod)
        {
            cacheMethods.Enqueue(cacheMethod);
            Debug.Log("addEvent: " + JsonConvert.SerializeObject(cacheMethod));
        }
    }
}
#endif
