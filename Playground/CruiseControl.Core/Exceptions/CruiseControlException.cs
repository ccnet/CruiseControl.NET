namespace CruiseControl.Core.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Base cruiseControl specific exception, inheriting from <see cref="System.ApplicationException"/>.
    /// </summary>
    [Serializable]
    public class CruiseControlException 
        : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CruiseControlException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public CruiseControlException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CruiseControlException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CruiseControlException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CruiseControlException" /> class.	
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        public CruiseControlException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}