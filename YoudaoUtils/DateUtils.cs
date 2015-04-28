

namespace YoudaoNoteUtils
{
    using System;

    public static class DateUtils
    {
        // 表示 1970 年 1 月 1 日 00:00:00 与 01 年 01 月 01 日 00:00:00 之间
        // 以 100 纳秒为单位的差值。即：(new DateTime(1970, 1, 1) - new DateTime(1, 1, 1)).Ticks
        private const long Diff = 621355968000000000;

        /// <summary>
        /// 将自 1970 年 1 月 1 日 00:00:00 以来经历的秒数转换成本地 DateTime 。
        /// </summary>
        /// <param name="seconds">自 1970 年 1 月 1 日 00:00:00 以来经历的秒数。</param>
        /// <returns>转换后的本地时间。</returns>
        public static DateTime ConvertFromSecondsToLocalDatetime(long seconds)
        {
            return new DateTime(seconds * 1000 * 1000 * 10 + Diff).ToLocalTime();
        }

        /// <summary>
        /// 将自 1970 年 1 月 1 日 00:00:00 以来经历的毫秒数转换成本地 DateTime 。
        /// </summary>
        /// <param name="milliseconds">自 1970 年 1 月 1 日 00:00:00 以来经历的毫秒数。</param>
        /// <returns>转换后的本地时间。</returns>
        public static DateTime ConvertFromMillisecondsToLocalDatetime(long milliseconds)
        {
            return new DateTime(milliseconds * 1000 * 10 + Diff).ToLocalTime();
        }

        /// <summary>
        /// 将本地 DateTime 对象转换成自 1970 年 1 月 1 日 00:00:00 以来经历的秒数。
        /// </summary>
        /// <param name="dt">本地 DateTime 对象。</param>
        /// <returns>自 1970 年 1 月 1 日 00:00:00 以来经历的秒数。</returns>
        public static long ConvertFromLocalDateTimeToSeconds(DateTime dt)
        {
            return (dt.ToUniversalTime().Ticks - Diff) / 1000 / 1000 / 10;
        }

        /// <summary>
        /// 将本地 DateTime 对象转换成自 1970 年 1 月 1 日 00:00:00 以来经历的毫秒数。
        /// </summary>
        /// <param name="dt">本地 DateTime 对象。</param>
        /// <returns>自 1970 年 1 月 1 日 00:00:00 以来经历的毫秒数。</returns>
        public static long ConvertFromLocalDateTimeToMilliseconds(DateTime dt)
        {
            return (dt.ToUniversalTime().Ticks - Diff) / 1000 / 10;
        }
    }
}
