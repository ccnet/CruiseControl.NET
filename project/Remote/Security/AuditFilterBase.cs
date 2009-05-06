using System;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote.Security
{
    /// <summary>
    /// A base class that provides some common audit filtering.
    /// </summary>
    [Serializable]
    [XmlInclude(typeof(CombinationAuditFilter))]
    [XmlInclude(typeof(DateRangeAuditFilter))]
    [XmlInclude(typeof(EventTypeAuditFilter))]
    [XmlInclude(typeof(ProjectAuditFilter))]
    [XmlInclude(typeof(SecurityRightAuditFilter))]
    [XmlInclude(typeof(UserAuditFilter))]
    public abstract class AuditFilterBase
    {
        private AuditFilterBase innerFilter;

        /// <summary>
        /// Starts a new blank filter.
        /// </summary>
        public AuditFilterBase() { }

        /// <summary>
        /// Starts a new filter with an inner filter.
        /// </summary>
        /// <param name="inner">The inner filter.</param>
        public AuditFilterBase(AuditFilterBase inner)
        {
            this.innerFilter = inner;
        }
        /// <summary>
        /// Checks whether the record should be included in the filter.
        /// </summary>
        /// <param name="record">The record to check.</param>
        /// <returns>True to include the record, false otherwise.</returns>
        public virtual bool CheckFilter(AuditRecord record)
        {
            bool include = DoCheckFilter(record);
            if (include && (innerFilter != null)) include = innerFilter.CheckFilter(record);
            return include;
        }

        /// <summary>
        /// Filters by project.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public virtual AuditFilterBase ByProject(string projectName)
        {
            return new ProjectAuditFilter(projectName, this);
        }

        /// <summary>
        /// Filters by user.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public virtual AuditFilterBase ByUser(string userName)
        {
            return new UserAuditFilter(userName, this);
        }

        /// <summary>
        /// Filters by event type.
        /// </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        public virtual AuditFilterBase ByEventType(SecurityEvent eventType)
        {
            return new EventTypeAuditFilter(eventType, this);
        }

        /// <summary>
        /// Filters by security right.
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual AuditFilterBase ByRight(SecurityRight right)
        {
            return new SecurityRightAuditFilter(right, this);
        }

        /// <summary>
        /// Filters by date range.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public virtual AuditFilterBase ByDateRange(DateTime startDate, DateTime endDate)
        {
            return new DateRangeAuditFilter(startDate, endDate, this);
        }

        /// <summary>
        /// Checks whether the record should be included in the filter.
        /// </summary>
        /// <param name="record">The record to check.</param>
        /// <returns>True to include the record, false otherwise.</returns>
        protected abstract bool DoCheckFilter(AuditRecord record);
    }
}
