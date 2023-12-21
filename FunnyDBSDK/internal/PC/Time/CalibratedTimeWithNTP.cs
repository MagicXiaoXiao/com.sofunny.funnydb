#if UNITY_STANDALONE || UNITY_EDITOR
using System;
using System.Threading.Tasks;

namespace SoFunny.FunnyDB.PC
{
    internal class CalibratedTimeWithNTP : ICalibratedTime
    {
        internal const int DEFAULT_TIME_OUT = 3000;
        private long _startTime;
        private long _systemElapsedRealtime;
        private string[] _ntpServer = Constants.NTP_SERVERS;

        internal CalibratedTimeWithNTP()
        {
            Task.Run(() =>
            {
                NTPClient nTPClient = new NTPClient();

                DateTime dataTime;
                foreach (string host in _ntpServer)
                {
                    try
                    {
                        dataTime = nTPClient.RequestTime(host, DEFAULT_TIME_OUT);
                        DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                        _startTime = (long)(dataTime - startTime).TotalMilliseconds;
                        _systemElapsedRealtime = Environment.TickCount;
                        Logger.LogVerbose($"ntpTime Success {host}, {dataTime}");
                        break;
                    }
                    catch (Exception e)
                    {
                        Logger.LogVerbose(e.ToString());
                    }
                }
            });
        }

        public DateTime Get()
        {
            if (_systemElapsedRealtime == 0)
            {
                Logger.LogVerbose("DateTime From Default");
                return DateTime.UtcNow;
            }
            Logger.LogVerbose("DateTime From ntp");
            long nowTickCount = Environment.TickCount;
            long timestamp = nowTickCount - _systemElapsedRealtime + _startTime;
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return dt.AddMilliseconds(timestamp);
        }

        public long GetInMills()
        {
            if (_systemElapsedRealtime == 0)
            {
                Logger.LogVerbose("DateTime From Default");
                TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                return (long)ts.TotalMilliseconds;
            }
            long nowTickCount = Environment.TickCount;
            return nowTickCount - _systemElapsedRealtime + _startTime;
        }
    }
}
#endif