using System;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote.Messages
{
    /// <summary>
    /// The base level message for all server-related requests.
    /// </summary>
    [XmlRoot("serverMessage")]
    [Serializable]
    public class ServerRequest
        : CommunicationsMessage
    {
        #region Private fields
        private string identifier = Guid.NewGuid().ToString();
        private string serverName;
        private string sessionToken;
        private string sourceName = Environment.MachineName;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new empty <see cref="ServerRequest"/>.
        /// </summary>
        public ServerRequest()
        {
        }

        /// <summary>
        /// Initialise a new <see cref="ServerRequest"/> with a session token.
        /// </summary>
        /// <param name="sessionToken">The session token to use.</param>
        public ServerRequest(string sessionToken)
        {
            this.sessionToken = sessionToken;
        }
        #endregion

        #region Public properties
        #region Identifier
        /// <summary>
        /// A unique identifier for the message.
        /// </summary>
        [XmlAttribute("identifier")]
        public string Identifier
        {
            get { return identifier; }
            set { identifier = value; }
        }
        #endregion

        #region ServerName
        /// <summary>
        /// The name of the server that this message is for.
        /// </summary>
        [XmlAttribute("server")]
        public string ServerName
        {
            get { return serverName; }
            set { serverName = value; }
        }
        #endregion

        #region SourceName
        /// <summary>
        /// The name of the machine that this message is from.
        /// </summary>
        [XmlAttribute("source")]
        public string SourceName
        {
            get { return sourceName; }
            set { sourceName = value; }
        }
        #endregion

        #region SessionToken
        /// <summary>
        /// A token to identify the session.
        /// </summary>
        [XmlAttribute("session")]
        public string SessionToken
        {
            get { return sessionToken; }
            set { sessionToken = value; }
        }
        #endregion

        #region DisplayName
        /// <summary>
        /// Gets or sets the display name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        /// <remarks>
        /// This will only be used on non-secure servers - if the server is secured then the user name from
        /// the session token will be used instead.
        /// </remarks>
        [XmlAttribute("displayName")]
        public string DisplayName { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region Equals()
        /// <summary>
        /// Checks if this request is the same as another.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var dummy = obj as ServerRequest;

            if (dummy != null)
            {
                return string.Equals((dummy).identifier, identifier, StringComparison.CurrentCulture);
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region GetHashCode()
        /// <summary>
        /// Returns the hash code for this request.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return identifier.GetHashCode();
        }
        #endregion

        #region ToString()
        /// <summary>
        /// Converts this request into a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            XmlSerializer serialiser = new XmlSerializer(this.GetType());
            StringBuilder builder = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = UTF8Encoding.UTF8;
            settings.Indent = false;
            settings.OmitXmlDeclaration = true;
            XmlWriter writer = XmlWriter.Create(builder, settings);
            serialiser.Serialize(writer, this);
            return builder.ToString();
        }
        #endregion
        #endregion
    }
}
