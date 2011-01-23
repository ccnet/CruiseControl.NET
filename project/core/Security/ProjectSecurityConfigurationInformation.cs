using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// Pass the security information to a client.
    /// </summary>
    [ReflectorType("projectSecurity")]
    public class ProjectSecurityConfigurationInformation
    {
        private string projectName;
        private IProjectAuthorisation projectSecurity;

        /// <summary>
        /// Gets or sets the name.	
        /// </summary>
        /// <value>The name.</value>
        /// <remarks></remarks>
        [ReflectorProperty("name")]
        public string Name
        {
            get { return projectName; }
            set { projectName = value; }
        }

        /// <summary>
        /// Gets or sets the security.	
        /// </summary>
        /// <value>The security.</value>
        /// <remarks></remarks>
        [ReflectorProperty("authorisation", InstanceTypeKey = "type")]
        public IProjectAuthorisation Security
        {
            get { return projectSecurity; }
            set { projectSecurity = value; }
        }
    }
}
