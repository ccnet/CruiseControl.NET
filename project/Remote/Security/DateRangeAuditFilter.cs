using System;
using System.Xml.Serialization;

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
        /// Initialises a new <see cref="DateRangeAuditFilter"/>.
        /// </summary>
        public DateRangeAuditFilter()
        {
        }

        /// <summary>
        /// Starts a new filter with the date range.
        /// </summary>
        public DateRangeAuditFilter(DateTime startDate, DateTime endDate)
            : this(startDate, endDate, null) { }

        /// <summary>
        /// Starts a new filter with the date range and inner filter.
        /// </summary>
        public DateRangeAuditFilter(DateTime startDate, DateTime endDate, AuditFilterBase innerFilter)
            : base(innerFilter)
        {
            if (startDate > endDate) throw new ArgumentOutOfRangeException("endDate cannot be before startDate");
            this.filterStartDate = startDate;
            this.filterEndDate = endDate;
        }

        #region Public properties
        #region StartDate
        /// <summary>
        /// The start date.
        /// </summary>
        [XmlAttribute("startDate")]
        public DateTime StartDate
        {
            get { return filterStartDate; }
            set { filterStartDate = value; }
        }
        #endregion

        #region EndDate
        /// <summary>
        /// The end date.
        /// </summary>
        [XmlAttribute("endDate")]
        public DateTime EndDate
        {
            get { return filterEndDate; }
            set { filterEndDate = value; }
        }
        #endregion
        #endregion

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
