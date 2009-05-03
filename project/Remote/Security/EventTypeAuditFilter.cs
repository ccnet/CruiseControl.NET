using System;

namespace ThoughtWorks.CruiseControl.Remote.Security
{
    /// <summary>
    /// Filters by an event type.
    /// </summary>
    [Serializable]
    public class EventTypeAuditFilter
        : AuditFilterBase
    {
        private SecurityEvent type;

        /// <summary>
        /// Starts a new filter with the event type.
        /// </summary>
        /// <param name="eventType"></param>
        public EventTypeAuditFilter(SecurityEvent eventType)
            : this(eventType, null) { }

        /// <summary>
        /// Starts a new filter with the event type and inner filter.
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="innerFilter"></param>
        public EventTypeAuditFilter(SecurityEvent eventType, IAuditFilter innerFilter)
            : base(innerFilter)
        {
            this.type = eventType;
        }

        /// <summary>
        /// Checks if the event type matches.
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        protected override bool DoCheckFilter(AuditRecord record)
        {
            bool include = (this.type == record.EventType);
            return include;
        }
    }
}
