using System;

namespace ThoughtWorks.CruiseControl.Core.tasks.Conditions.DateTimeHelpers
{
    public class WeekDayMask
    {
        private WeekDay mask;
        public WeekDayMask(string weekdays)
        {
            mask = GetWeekDayMask(weekdays);
        }

        internal static WeekDay GetWeekDayMask(string weekdays)
        {
            WeekDay result = WeekDay.None;
            foreach (string weekday in weekdays.Split(','))
            {
                if (weekday.Contains("-"))
                {
                    string[] split = weekday.Split('-');
                    if (split.Length != 2)
                        throw new ArgumentException("Invalid weekdays string", "weekdays");

                    WeekDay startWeekDay = ParseWeekDay(split[0]);
                    int startOffset = (int)Math.Log((int)startWeekDay, 2);
                    int endOffset = (int)Math.Log((int)ParseWeekDay(split[1]), 2);
                    if (startOffset == endOffset)
                        result = startWeekDay;
                    else
                        if (startOffset < endOffset)
                            for (int i = startOffset; i <= endOffset; i++)
                                result = result | (WeekDay)(int)Math.Pow(2, i);
                        else
                        {
                            for (int i = 0; i <= endOffset; i++)
                                result = result | (WeekDay)(int)Math.Pow(2, i);
                            for (int i = startOffset; i <= 6; i++)
                                result = result | (WeekDay)(int)Math.Pow(2, i);
                        }
                }
                else
                    result = result | ParseWeekDay(weekday);
            }
            return result;
        }

        internal static WeekDay ParseWeekDay(string weekday)
        {
            try
            {
                return (WeekDay)Enum.Parse(typeof(WeekDay), weekday, true);
            }
            catch (Exception exception)
            {
                throw new ArgumentException("Unable to parse weekday string '" + weekday + "'", exception);
            }
        }

        public override string ToString()
        {
            return mask.ToString();
        }

        public static implicit operator string(WeekDayMask mask)
        {
            return mask.ToString();
        }

        public static implicit operator WeekDayMask(string s)
        {
            return new WeekDayMask(s);
        }

        public bool Contains(WeekDay day)
        {
            return ((day & mask) != WeekDay.None);
        }
    }
}