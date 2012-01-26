
using System;
using System.Runtime.Serialization;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// Base cruiseControl specific exception, inheriting from <see cref="System.ApplicationException"/>.
    /// </summary>
	[Serializable]
	public class CruiseControlException : ApplicationException
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="CruiseControlException" /> class.	
        /// </summary>
        /// <remarks></remarks>
		public CruiseControlException() : base(string.Empty) {}
        /// <summary>
        /// Initializes a new instance of the <see cref="CruiseControlException" /> class.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks></remarks>
		public CruiseControlException(string message) : base(message) {}
        /// <summary>
        /// Initializes a new instance of the <see cref="CruiseControlException" /> class.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="e">The exception.</param>
        /// <remarks></remarks>
		public CruiseControlException(string message, Exception e) : base(message, e) {}
        /// <summary>
        /// Initializes a new instance of the <see cref="CruiseControlException" /> class.	
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        /// <remarks></remarks>
		public CruiseControlException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}