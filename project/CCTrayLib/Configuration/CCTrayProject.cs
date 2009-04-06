using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
	[XmlType("Project")]
	public class CCTrayProject
	{
		private BuildServer buildServer;
		private string projectName;
		private bool showProject;

		public CCTrayProject()
		{
			buildServer = new BuildServer();
			showProject = true;
		}

		public CCTrayProject(string serverUrl, string projectName)
		{
			ServerUrl = serverUrl;
			ProjectName = projectName;
			showProject = true;
		}

		public CCTrayProject(BuildServer buildServer, string projectName)
		{
			this.buildServer = buildServer;
			this.projectName = projectName;
			showProject = true;
		}

		[XmlAttribute(AttributeName="serverUrl")]
		public string ServerUrl
		{
			get { return buildServer.Url; }
			set { buildServer = new BuildServer(value); }
		}

		[XmlAttribute(AttributeName="projectName")]
		public string ProjectName
		{
			get { return projectName; }
			set { projectName = value; }
		}
		
		[XmlAttribute(AttributeName = "showProject")]
		public bool ShowProject
		{
			get { return showProject; }
			set { showProject = value; }
		}
		
		[XmlIgnore]
		public BuildServer BuildServer
		{
			get { return buildServer; }
			set { buildServer = value; }
		}

        /// <summary>
        /// Check to see if two sets of configuration are the same.
        /// </summary>
        /// <param name="obj">The other configuration set to check.</param>
        /// <returns>True if they are both the same, false otherwise.</returns>
        /// <remarks>
        /// This is part of the fix for CCNET-1179.
        /// </remarks>
        public override bool Equals(object obj)
        {
            CCTrayProject objToCompare = obj as CCTrayProject;
            if (objToCompare != null)
            {
                bool isSame = string.Equals(projectName, objToCompare.projectName);

                if (isSame)
                {
                    if ((buildServer != null) && (objToCompare.buildServer != null))
                    {
                        // If both instances have a build server then compare the build server settings
                        isSame = string.Equals(buildServer.Url, objToCompare.buildServer.Url);
                    }
                    else if ((buildServer != null) && (objToCompare.buildServer != null))
                    {
                        // If neither instance has a build server then they are the same
                        isSame = true;
                    }
                    else
                    {
                        // Otherwise we know we have a difference
                        isSame = false;
                    }
                }
                return isSame;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieve the hashcode for this configuration.
        /// </summary>
        /// <returns>The hashcode of the project name plus build server URL.</returns>
        /// <remarks>
        /// This is part of the fix for CCNET-1179.
        /// </remarks>
        public override int GetHashCode()
        {
            string hashCode = string.Empty;
            if (projectName != null) hashCode = projectName;
            if (buildServer != null)
            {
                hashCode += buildServer.Url;
            }
            return hashCode.GetHashCode();
        }
	}
}
