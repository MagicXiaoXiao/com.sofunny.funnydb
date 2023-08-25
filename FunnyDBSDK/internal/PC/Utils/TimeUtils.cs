using System;

namespace SoFunny.FunnyDB.PC
{

    internal static class TimeUtils
    {
        /// <summary>
        /// 获取时区
        /// </summary>
        /// <returns></returns>
        internal static double GetZoneOffset()
        {
            TimeSpan timeSpan = new TimeSpan();
            try
            {
                timeSpan = TimeZoneInfo.Local.BaseUtcOffset;
            }
            catch (Exception excption)
            {
                Logger.LogVerbose(excption.Message);
            }
            return timeSpan.TotalHours;
        }

        internal static long GetTimeStamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds) - 8 * 60 * 60 * 1000;
        }
    }
}

