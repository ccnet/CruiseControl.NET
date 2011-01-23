using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ThoughtWorks.CruiseControl.Core.tasks.Conditions.DateTimeHelpers
{
    [DebuggerDisplay("Time period [{FromTime} - {ToTime}] Inverted : {Inverted}")]
    public class TimePeriod
    {
        private static readonly Regex valueSplitter = new Regex(@"^(?<From>\d{1,2}:\d{2})-(?<To>\d{1,2}:\d{2})$");

        private readonly TimeSpan _fromTime;
        private readonly bool _inverted;

        private readonly TimeSpan _toTime;

        /// <exception cref="System.ArgumentException"><c>ArgumentException</c>.</exception>
        public TimePeriod(string source)
        {
            if (!valueSplitter.IsMatch(source))
                throw new ArgumentException("Time periods must be in for format '[hour]:[minute]-[hour]:[minute]'");

            Match split = valueSplitter.Match(source);

            _fromTime = Parse(split.Groups["From"].Value);
            _toTime = Parse(split.Groups["To"].Value);

            if (_fromTime > _toTime)
            {
                _inverted = true;
                TimeSpan tempSpan = _fromTime;
                _fromTime = _toTime;
                _toTime = tempSpan;
            }
        }

        private TimeSpan FromTime
        {
            get
            {
                return _fromTime;
            }
        }

        private TimeSpan ToTime
        {
            get
            {
                return _toTime;
            }
        }

        private bool Inverted
        {
            get
            {
                return _inverted;
            }
        }

        public static implicit operator string(TimePeriod p)
        {
            return p.ToString();
        }

        public static implicit operator TimePeriod(string s)
        {
            return new TimePeriod(s);
        }

        public static TimePeriod[] ParsePeriods(string periods)
        {
            List<TimePeriod> result = new List<TimePeriod>();

            string[] periodArray = periods.Split(',');
            foreach (string period in periodArray)
            {
                result.Add(new TimePeriod(period));
            }

            return result.ToArray();
        }

        /// <exception cref="ArgumentException"><c>ArgumentException</c>.</exception>
        public static TimeSpan Parse(string value)
        {
            TimeSpan result;
            if (!TimeSpan.TryParse(value, out result))
                if (value == "24:00")
                    result = new TimeSpan(1, 0, 0, 0);
                else
                    throw new ArgumentException("Invalid time specification '" + value + "'");
            return result;
        }

        public bool Contained(TimeSpan time)
        {
            bool contained = (FromTime.Ticks <= time.Ticks && ToTime.Ticks >= time.Ticks);
            return Inverted ? !contained : contained;
        }

        public override string ToString()
        {
            TimeSpan to = Inverted ? FromTime : ToTime;
            TimeSpan from = Inverted ? ToTime : FromTime;

            if (to.Days == 0)
                return string.Format("{0:D2}:{1:D2}-{2:D2}:{3:D2}", from.Hours, from.Minutes, to.Hours, to.Minutes);
            
            return string.Format("{0:D2}:{1:D2}-24:00", from.Hours, from.Minutes);
        }
    }
}