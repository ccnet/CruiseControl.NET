using System;
using System.Xml.Serialization;

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
        /// Initialises a new <see cref="EventTypeAuditFilter"/>.
        /// </summary>
        public EventTypeAuditFilter()
        {
        }

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
        public EventTypeAuditFilter(SecurityEvent eventType, AuditFilterBase innerFilter)
            : base(innerFilter)
        {
            this.type = eventType;
        }

        #region Public properties
        #region EventType
        /// <summary>
        /// The type of event.
        /// </summary>
        [XmlAttribute("type")]
        public SecurityEvent EventType
        {
            get { return type; }
            set { type = value; }
        }
        #endregion
        #endregion

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
