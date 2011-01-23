
using System;
namespace ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation
{
    /// <summary>
    /// 	
    /// </summary>
	public class DefaultServerSpecifier : IServerSpecifier
	{
		private readonly string serverName;
		private readonly bool allowForceBuild;
		private readonly bool allowStartStopBuild;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultServerSpecifier" /> class.	
        /// </summary>
        /// <param name="serverName">Name of the server.</param>
        /// <remarks></remarks>
		public DefaultServerSpecifier(string serverName)
		{
			if (serverName == null)
			{
				throw new CruiseControlException("Server Specifier cannot be instantiated with a null server name");
			}
			if ((serverName != null && serverName.Length == 0))
			{
				throw new CruiseControlException("Server Specifier cannot be instantiated with an empty server name");
			}
			this.serverName = serverName;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultServerSpecifier" /> class.	
        /// </summary>
        /// <param name="serverName">Name of the server.</param>
        /// <param name="allowForceBuild">The allow force build.</param>
        /// <param name="allowStartStopBuild">The allow start stop build.</param>
        /// <remarks></remarks>
		public DefaultServerSpecifier(string serverName, bool allowForceBuild, bool allowStartStopBuild)
		{
			this.serverName = serverName;
			this.allowForceBuild = allowForceBuild;
			this.allowStartStopBuild = allowStartStopBuild;
		}

        /// <summary>
        /// Gets the name of the server.	
        /// </summary>
        /// <value>The name of the server.</value>
        /// <remarks></remarks>
		public string ServerName
		{
			get { return serverName; }
		}

        /// <summary>
        /// Gets the allow force build.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public bool AllowForceBuild
		{
			get { return allowForceBuild; }
		}

        /// <summary>
        /// Gets the allow start stop build.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public bool AllowStartStopBuild
		{
			get { return allowStartStopBuild; }
		}

        /// <summary>
        /// Equalses the specified obj.	
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public override bool Equals(object obj)
		{
            DefaultServerSpecifier other = obj as DefaultServerSpecifier;
            if (other != null)
			{
				return this.ServerName == other.ServerName;
			}
			return false;
		}

        /// <summary>
        /// Gets the hash code.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

        /// <summary>
        /// Toes the string.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
		public override string ToString()
		{
			return serverName;
		}
	}
}