using System;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote.Security
{
    /// <summary>
    /// Details about an evented event.
    /// </summary>
    [Serializable]
    public class AuditRecord
    {
        private DateTime timeOfEvent;
        private string projectName;
        private string userName;
        private SecurityEvent eventType;
        private SecurityRight eventRight;
        private string message;

        /// <summary>
        /// The date and time of the event.
        /// </summary>
        public DateTime TimeOfEvent
        {
            get { return timeOfEvent; }
            set { timeOfEvent = value; }
        }

        /// <summary>
        /// The name of the project the event was for.
        /// </summary>
        public string ProjectName
        {
            get { return projectName; }
            set { projectName = value; }
        }

        /// <summary>
        /// The name of the user the event was for.
        /// </summary>
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        /// <summary>
        /// The type of event.
        /// </summary>
        public SecurityEvent EventType
        {
            get { return eventType; }
            set { eventType = value; }
        }

        /// <summary>
        /// The right that is being audited.
        /// </summary>
        public SecurityRight SecurityRight
        {
            get { return eventRight; }
            set { eventRight = value; }
        }

        /// <summary>
        /// An optional message for the event.
        /// </summary>
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }
}
