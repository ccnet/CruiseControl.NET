
using System;
using System.Runtime.Serialization;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// 	
    /// </summary>
    [Serializable]
    public class SecurityException
        : CruiseControlException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityException" /> class.	
        /// </summary>
        /// <remarks></remarks>
		public SecurityException() : base("A security failure has occurred.") {}
        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityException" /> class.	
        /// </summary>
        /// <param name="s">The s.</param>
        /// <remarks></remarks>
		public SecurityException(string s) : base(s) {}
        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityException" /> class.	
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
		public SecurityException(string s, Exception e) : base(s, e) {}
        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityException" /> class.	
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        /// <remarks></remarks>
        public SecurityException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        /// <summary>
        /// Gets the object data.	
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        /// <remarks></remarks>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
