using System;

namespace ThoughtWorks.CruiseControl.Remote.Security
{
    /// <summary>
    /// A filter for the audit log.
    /// </summary>
    public interface IAuditFilter
    {
        /// <summary>
        /// Checks whether the record should be included in the filter.
        /// </summary>
        /// <param name="record">The record to check.</param>
        /// <returns>True to include the record, false otherwise.</returns>
        bool CheckFilter(AuditRecord record);

        /// <summary>
        /// Filters by project.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        IAuditFilter ByProject(string projectName);

        /// <summary>
        /// Filters by user.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        IAuditFilter ByUser(string userName);

        /// <summary>
        /// Filters by event type.
        /// </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        IAuditFilter ByEventType(SecurityEvent eventType);

        /// <summary>
        /// Filters by security right.
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        IAuditFilter ByRight(SecurityRight right);

        /// <summary>
        /// Filters by date range.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        IAuditFilter ByDateRange(DateTime startDate, DateTime endDate);
    }
}
