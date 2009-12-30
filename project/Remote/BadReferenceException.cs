#pragma warning disable 1591
using System;
using System.Runtime.Serialization;

namespace ThoughtWorks.CruiseControl.Core
{
    [Serializable]
    public class BadReferenceException
        : CruiseControlException
    {
        private const string referenceData = "REFERENCE_NAME";
        private string reference;

        public BadReferenceException(string reference)
            : this(reference, string.Format("Reference '{0}' is either incorrect or missing.", reference), null)
        {
        }
        public BadReferenceException(string reference, string s) : this(reference, s, null) { }
        public BadReferenceException(string reference, string s, Exception e)
            : base(s, e)
        {
            this.reference = reference;
        }
        public BadReferenceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.reference = info.GetString(referenceData);
        }

        public string Reference
        {
            get { return this.reference; }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(referenceData, reference);
        }
    }
}
