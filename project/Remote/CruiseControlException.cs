
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
        /// <param name="s">The s.</param>
        /// <remarks></remarks>
		public CruiseControlException(string s) : base(s) {}
        /// <summary>
        /// Initializes a new instance of the <see cref="CruiseControlException" /> class.	
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
		public CruiseControlException(string s, Exception e) : base(s, e) {}
        /// <summary>
        /// Initializes a new instance of the <see cref="CruiseControlException" /> class.	
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        /// <remarks></remarks>
		public CruiseControlException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}