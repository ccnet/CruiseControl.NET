using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
    public class UncombinableFingerprintException : ApplicationException
    {
        public UncombinableFingerprintException(string leftHandEtag, string rightHandEtag) :
            base(
            string.Format(
                "A ConditionalGetFingerprint was combined with another fingerprint with a different etag. Fingerprints must have the same etag to be combined. The left hand etag was '{0}' and the right hand etag was '{1}'",
                leftHandEtag, rightHandEtag))
        {
        }
    }
}