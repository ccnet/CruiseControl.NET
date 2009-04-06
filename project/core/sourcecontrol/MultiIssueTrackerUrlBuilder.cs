using Exortech.NetReflector;
using System;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    [ReflectorType("multiIssueTracker")]
    public class MultiIssueTrackerUrlBuilder : IModificationUrlBuilder
    {

        private IModificationUrlBuilder[] _issueTrackers;


        [ReflectorArray("issueTrackers", Required = true)]
        public IModificationUrlBuilder[] IssueTrackers
        {
            get
            {
                if (_issueTrackers == null)
                    _issueTrackers = new IModificationUrlBuilder[0];

                return _issueTrackers;
            }

            set { _issueTrackers = value; }
        }

        public void SetupModification(Modification[] modifications)
        {            
            foreach (IModificationUrlBuilder modificationUrlBuilder in _issueTrackers)
            {
                modificationUrlBuilder.SetupModification(modifications);          
            }            
        }

    }
}
