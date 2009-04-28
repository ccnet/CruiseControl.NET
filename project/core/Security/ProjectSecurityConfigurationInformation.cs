using Exortech.NetReflector;
using System;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    [ReflectorType("projectSecurity")]
    public class ProjectSecurityConfigurationInformation
    {
        private string projectName;
        private IProjectAuthorisation projectSecurity;

        [ReflectorProperty("name")]
        public string Name
        {
            get { return projectName; }
            set { projectName = value; }
        }

        [ReflectorProperty("authorisation", InstanceTypeKey = "type")]
        public IProjectAuthorisation Security
        {
            get { return projectSecurity; }
            set { projectSecurity = value; }
        }
    }
}
