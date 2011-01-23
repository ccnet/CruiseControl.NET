using System;
using System.Text.RegularExpressions;

namespace ThoughtWorks.CruiseControl.Core.tasks.Conditions.DateTimeHelpers
{
    public static class DatetimeFunctions
    {
        internal static DateTime getTimeInTimeZone(string zone, DateTime time)
        {
            TimeSpan offset = getTimeZoneOffset(zone, time);
            return time.Add(offset);
        }

        /// <exception cref="ArgumentException">Timezone must be in the format GMT[+|-][hour]:[minute]</exception>
        public static TimeSpan getTimeZoneOffset(string zone, DateTime time)
        {
            if (string.IsNullOrEmpty(zone) || zone.Equals("current", StringComparison.InvariantCultureIgnoreCase)) return new TimeSpan(0);

            TimeSpan baseOffset = new TimeSpan(-TimeZone.CurrentTimeZone.GetUtcOffset(time).Ticks);

            TimeSpan timeZoneOffset = ParseTimeZone(zone);
            return timeZoneOffset - baseOffset;
        }

        public static TimeSpan ParseTimeZone(string zone)
        {
            Regex timezoneSplitter = new Regex(@"^GMT(?<offset>(?<symbol>[+-])(?<hour>\d{1,2})(:(?<minute>\d{2}))?)?$");
            if (!timezoneSplitter.IsMatch(zone))
                throw new ArgumentException("Timezone must be in the format GMT[+|-][hour]:[minute]");

            Match timezone = timezoneSplitter.Match(zone);
            if (!timezone.Groups["offset"].Success)
                return new TimeSpan(0); //Only GMT specified

            bool positive = timezone.Groups["symbol"].Value == "+";
            int hours = 0;
            int minutes = 0;
            int.TryParse(timezone.Groups["hour"].Value, out hours);
            int.TryParse(timezone.Groups["minute"].Value, out minutes);

            if(hours>12)
                throw new ArgumentException("GMT must be between -12 and +12");

            if(minutes<0 || minutes>59)
                throw new ArgumentException("Minutes must be between 0 and 59");

            TimeSpan timeZoneOffset = positive ? new TimeSpan(0, hours, minutes, 0) : -new TimeSpan(0, hours, minutes, 0);
            return timeZoneOffset;
        }
    }
}