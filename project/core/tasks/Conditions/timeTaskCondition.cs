using System;
using System.Collections.Generic;
using System.Globalization;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Tasks.Conditions;
using ThoughtWorks.CruiseControl.Core.tasks.Conditions.DateTimeHelpers;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.tasks.Conditions
{
    [System.Diagnostics.DebuggerDisplay("Time Condition")]
    [ReflectorType("timeCondition")]
    public class TimeTaskCondition : ConditionBase
    {
        private readonly DateTimeProvider _dtProvider;
        private static readonly CultureInfo _enUsCulture = new CultureInfo(1033);
        private static readonly Dictionary<string, TimePeriod[]> _timeperiodCache = new Dictionary<string, TimePeriod[]>();

        public TimeTaskCondition() : this(new DateTimeProvider())
        {
        }

        public TimeTaskCondition(DateTimeProvider dtProvider)
        {
            DayOfWeek = WeekDay.Any.ToString();
            TimeOfDay = "00:00-24:00";
            TimeToEvaluate = TimeToEvaluate.now;
            _dtProvider = dtProvider;
        }

        [ReflectorProperty("dayOfWeek", Required = false)]
        public string DayOfWeek { get; set; }

        [ReflectorProperty("timeOfDay", Required = false)]
        public string TimeOfDay{ get; set;}

        [ReflectorProperty("timeZone", Required = false)]
        public string TimeZone { get; set; }

        [ReflectorProperty("timeToEvaluate", Required = false)]
        public TimeToEvaluate TimeToEvaluate { get; set; }

        protected override bool Evaluate(IIntegrationResult result)
        {
            DateTime current = getEvaluationTime(result);
            current = DatetimeFunctions.getTimeInTimeZone(TimeZone, current);
            if (evalWeekDay(current))
                return evalTimeperiod(current);
            return false;
        }

        private DateTime getEvaluationTime(IIntegrationResult result)
        {
            switch (TimeToEvaluate)
            {
                case TimeToEvaluate.now:
                    return _dtProvider.Now;
                case TimeToEvaluate.buildStart:
                    return result.StartTime;
                case TimeToEvaluate.buildEnd:
                    return result.EndTime;
                case TimeToEvaluate.firstModification:
                    if (result.Modifications.Length > 0)
                        return firstModifiedTime(result.Modifications);
                    break;
                case TimeToEvaluate.lastModification:
                    if (result.Modifications.Length > 0)
                        return lastModifiedTime(result.Modifications);
                    break;
            }
            throw new ArgumentOutOfRangeException();
        }

        private DateTime lastModifiedTime(Modification[] modifications)
        {
            DateTime subResult = modifications[0].ModifiedTime;
            foreach (Modification modification in modifications)
                if (modification.ModifiedTime > subResult)
                    subResult = modification.ModifiedTime;
            return subResult;
        }

        private DateTime firstModifiedTime(Modification[] modifications)
        {
            DateTime subResult = modifications[0].ModifiedTime;
            foreach (Modification modification in modifications)
                if (modification.ModifiedTime < subResult)
                    subResult = modification.ModifiedTime;
            return subResult;
        }

        private bool evalWeekDay(DateTime current)
        {
            WeekDayMask weekDayMask = DayOfWeek;
            DateTime time = current;
            WeekDay day = WeekDayMask.ParseWeekDay(time.ToString("dddd", _enUsCulture));

            return weekDayMask.Contains(day);
        }

        private bool evalTimeperiod(DateTime currentDateTime)
        {
            TimePeriod[] periods;
            if (!_timeperiodCache.ContainsKey(TimeOfDay))
            {
                _timeperiodCache.Add(TimeOfDay, TimePeriod.ParsePeriods(TimeOfDay));
            }
            periods = _timeperiodCache[TimeOfDay];

            TimeSpan currentTime = currentDateTime.Subtract(currentDateTime.Date);

            foreach (TimePeriod period in periods)
            {
                if (period.Contained(currentTime))
                    return true;
            }
            return false;
        }
    }
}