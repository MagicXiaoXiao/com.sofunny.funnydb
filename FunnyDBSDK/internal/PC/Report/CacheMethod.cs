#if UNITY_STANDALONE || UNITY_EDITOR
using System.Collections.Concurrent;
using Newtonsoft.Json;

namespace SoFunny.FunnyDB.PC
{
    internal sealed class CacheMethod
    {
        private static ConcurrentBag<CacheMethod> cacheMethods = new ConcurrentBag<CacheMethod>();
        public string Method;
        public object[] MethodParams;

        public CacheMethod(string method, object[] methodParms)
        {
            Method = method;
            MethodParams = methodParms;
        }
        public static void ReportCacheEvents()
        {
            foreach (var item in cacheMethods)
            {
                Logger.Log($"zxx send cacheMethod {JsonConvert.SerializeObject(item)}");
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
            cacheMethods.Clear();
        }

        public static void AddEvent(CacheMethod cacheMethod)
        {
            cacheMethods.Add(cacheMethod);
            Logger.Log($"zxx cacheMethod {JsonConvert.SerializeObject(cacheMethod)}");
        }
    }
}
#endif
