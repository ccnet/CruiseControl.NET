using System;
using System.Runtime.Serialization;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// An exception that occurred during communications.
    /// </summary>
    [Serializable]
    public class CommunicationsException 
        : ApplicationException
    {
        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="CommunicationsException"/>.
        /// </summary>
        public CommunicationsException() : base("A communications error has occurred.") { }

        /// <summary>
        /// Initialise a new <see cref="CommunicationsException"/>.
        /// </summary>
        public CommunicationsException(string s) : base(s) { }

        /// <summary>
        /// Initialise a new <see cref="CommunicationsException"/>.
        /// </summary>
        public CommunicationsException(string s, Exception e) : base(s, e) { }

        /// <summary>
        /// Initialise a new <see cref="CommunicationsException"/>.
        /// </summary>
        public CommunicationsException(string s, string type)
            : base(s)
        {
            ErrorType = type;
        }

        /// <summary>
        /// Initialise a new <see cref="CommunicationsException"/>.
        /// </summary>
        public CommunicationsException(string s, Exception e, string type) : base(s, e)
        {
            ErrorType = type;
        }

        /// <summary>
        /// Initialise a new <see cref="CommunicationsException"/>.
        /// </summary>
        public CommunicationsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        #endregion

        #region Public properties
        #region ErrorType
        /// <summary>
        /// The error tytpe returned from the server.
        /// </summary>
        public string ErrorType { get; private set; }
        #endregion
        #endregion
    }
}