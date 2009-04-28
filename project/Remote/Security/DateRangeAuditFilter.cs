using System;

namespace ThoughtWorks.CruiseControl.Remote.Security
{
    /// <summary>
    /// Filters by a date range.
    /// </summary>
    [Serializable]
    public class DateRangeAuditFilter
        : AuditFilterBase
    {
        private DateTime filterStartDate;
        private DateTime filterEndDate;

        /// <summary>
        /// Starts a new filter with the date range.
        /// </summary>
        /// <param name="userName"></param>
        public DateRangeAuditFilter(DateTime startDate, DateTime endDate)
            : this(startDate, endDate, null) { }

        /// <summary>
        /// Starts a new filter with the date range and inner filter.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="innerFilter"></param>
        public DateRangeAuditFilter(DateTime startDate, DateTime endDate, IAuditFilter innerFilter)
            : base(innerFilter)
        {
            if (startDate > endDate) throw new ArgumentOutOfRangeException("endDate cannot be before startDate");
            this.filterStartDate = startDate;
            this.filterEndDate = endDate;
        }

        /// <summary>
        /// Checks if the date range matches.
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        protected override bool DoCheckFilter(AuditRecord record)
        {
            bool include = (record.TimeOfEvent >= filterStartDate) &&
                (record.TimeOfEvent <= filterEndDate);
            return include;
        }
    }
}
