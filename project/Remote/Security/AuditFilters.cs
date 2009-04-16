using System;

namespace ThoughtWorks.CruiseControl.Remote.Security
{
    /// <summary>
    /// Generates a filter.
    /// </summary>
    public static class AuditFilters
    {
        /// <summary>
        /// Filters by project.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public static IAuditFilter ByProject(string projectName)
        {
            return new ProjectAuditFilter(projectName);
        }

        /// <summary>
        /// Filters by user.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static IAuditFilter ByUser(string userName)
        {
            return new UserAuditFilter(userName);
        }

        /// <summary>
        /// Filters by event type.
        /// </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        public static IAuditFilter ByEventType(SecurityEvent eventType)
        {
            return new EventTypeAuditFilter(eventType);
        }

        /// <summary>
        /// Filters by security right.
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public static IAuditFilter ByRight(SecurityRight right)
        {
            return new SecurityRightAuditFilter(right);
        }

        /// <summary>
        /// Filters by date range.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static IAuditFilter ByDateRange(DateTime startDate, DateTime endDate)
        {
            return new DateRangeAuditFilter(startDate, endDate);
        }

        /// <summary>
        /// Combines multiple filters together.
        /// </summary>
        /// <param name="filters">The filters to combine.</param>
        /// <returns></returns>
        public static IAuditFilter Combine(params IAuditFilter[] filters)
        {
            return new CombinationAuditFilter(filters);
        }
    }
}
