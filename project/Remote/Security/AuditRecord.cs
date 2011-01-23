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
        #region Private fields
        private DateTime timeOfEvent;
        private string projectName;
        private string userName;
        private SecurityEvent eventType;
        private SecurityRight eventRight;
        private string message;
        #endregion

        #region Public properties
        #region TimeOfEvent
        /// <summary>
        /// The date and time of the event.
        /// </summary>
        [XmlAttribute("time")]
        public DateTime TimeOfEvent
        {
            get { return timeOfEvent; }
            set { timeOfEvent = value; }
        }
        #endregion

        #region ProjectName
        /// <summary>
        /// The name of the project the event was for.
        /// </summary>
        [XmlAttribute("project")]
        public string ProjectName
        {
            get { return projectName; }
            set { projectName = value; }
        }
        #endregion

        #region UserName
        /// <summary>
        /// The name of the user the event was for.
        /// </summary>
        [XmlAttribute("user")]
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }
        #endregion

        #region EventType
        /// <summary>
        /// The type of event.
        /// </summary>
        [XmlAttribute("event")]
        public SecurityEvent EventType
        {
            get { return eventType; }
            set { eventType = value; }
        }
        #endregion

        #region SecurityRight
        /// <summary>
        /// The right that is being audited.
        /// </summary>
        [XmlAttribute("right")]
        public SecurityRight SecurityRight
        {
            get { return eventRight; }
            set { eventRight = value; }
        }
        #endregion

        #region Message
        /// <summary>
        /// An optional message for the event.
        /// </summary>
        [XmlElement("message")]
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        #endregion
        #endregion
    }
}
