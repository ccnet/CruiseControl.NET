namespace ThoughtWorks.CruiseControl.Remote
{
    using System;
    using System.Runtime.Serialization;

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
        public CommunicationsException(string message) : base(message) { }

        /// <summary>
        /// Initialise a new <see cref="CommunicationsException"/>.
        /// </summary>
        public CommunicationsException(string message, Exception e) : base(message, e) { }

        /// <summary>
        /// Initialise a new <see cref="CommunicationsException"/>.
        /// </summary>
        public CommunicationsException(string message, string type)
            : base(message)
        {
            this.ErrorType = type;
        }

        /// <summary>
        /// Initialise a new <see cref="CommunicationsException"/>.
        /// </summary>
        public CommunicationsException(string message, Exception e, string type) : base(message, e)
        {
            this.ErrorType = type;
        }

        /// <summary>
        /// Initialise a new <see cref="CommunicationsException"/>.
        /// </summary>
        public CommunicationsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.ErrorType = info.GetString("_errorType");
        }
        #endregion

        #region Public properties
        #region ErrorType
        /// <summary>
        /// The error tytpe returned from the server.
        /// </summary>
        public string ErrorType { get; private set; }
        #endregion
        #endregion

        #region Public methods
        #region GetObjectData()
        /// <summary>
        /// Sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is a null reference (Nothing in Visual Basic).
        /// </exception>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/>
        /// </PermissionSet>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_errorType", this.ErrorType);
            base.GetObjectData(info, context);
        }
        #endregion
        #endregion
    }
}