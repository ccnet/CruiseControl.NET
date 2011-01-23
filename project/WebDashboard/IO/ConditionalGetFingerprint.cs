using System;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
    public class ConditionalGetFingerprint
    {
        public readonly static ConditionalGetFingerprint NOT_AVAILABLE = new ConditionalGetFingerprint(DateTime.MinValue, "\"NOT AVAILABLE\"");

        private readonly DateTime lastModifiedTime;
        private readonly string eTag;

        public ConditionalGetFingerprint(DateTime ifModifiedSince, string ifNoneMatch)
        {
            lastModifiedTime = ifModifiedSince;
            eTag = ifNoneMatch;
        }

        public ConditionalGetFingerprint Combine(ConditionalGetFingerprint other)
        {
            if (this == NOT_AVAILABLE || other == NOT_AVAILABLE) return NOT_AVAILABLE;
            if (eTag != other.eTag) throw new UncombinableFingerprintException(eTag, other.eTag);

            DateTime newerModificationTime = DateUtil.MaxDate(lastModifiedTime, other.lastModifiedTime);
            return new ConditionalGetFingerprint(newerModificationTime, eTag);
        }

        public override bool Equals(object obj)
        {
            if (this == NOT_AVAILABLE || obj == NOT_AVAILABLE) return false;
            if (this == obj) return true;
            ConditionalGetFingerprint conditionalGetFingerprint = obj as ConditionalGetFingerprint;
            if (conditionalGetFingerprint == null) return false;
        	
            return 
                lastModifiedTime.Equals(conditionalGetFingerprint.lastModifiedTime) &&
                eTag.Equals(conditionalGetFingerprint.eTag);
        }

        public override int GetHashCode()
        {
            return lastModifiedTime.GetHashCode() + 29*eTag.GetHashCode();
        }

        public DateTime LastModifiedTime
        {
            get { return lastModifiedTime; }
        }

        public string ETag
        {
            get { return eTag; }
        }
    }
}