
using System;
using System.Runtime.Serialization;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// 	
    /// </summary>
    [Serializable]
    public class BadReferenceException
        : CruiseControlException
    {
        private const string referenceData = "REFERENCE_NAME";
        private string reference;

        /// <summary>
        /// Initializes a new instance of the <see cref="BadReferenceException" /> class.	
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <remarks></remarks>
        public BadReferenceException(string reference)
            : this(reference, string.Format(System.Globalization.CultureInfo.CurrentCulture,"Reference '{0}' is either incorrect or missing.", reference), null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BadReferenceException" /> class.	
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <param name="message">The message.</param>
        /// <remarks></remarks>
        public BadReferenceException(string reference, string message) : this(reference, message, null) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="BadReferenceException" /> class.	
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <param name="message">The message.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        public BadReferenceException(string reference, string message, Exception e)
            : base(message, e)
        {
            this.reference = reference;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BadReferenceException" /> class.	
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        /// <remarks></remarks>
        public BadReferenceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.reference = info.GetString(referenceData);
        }

        /// <summary>
        /// Gets the reference.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string Reference
        {
            get { return this.reference; }
        }

        /// <summary>
        /// Gets the object data.	
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        /// <remarks></remarks>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(referenceData, reference);
        }
    }
}
