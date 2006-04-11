using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation
{
	public class DefaultServerSpecifier : IServerSpecifier
	{
		private readonly string serverName;
		private readonly bool allowForceBuild;
		private readonly bool allowStartStopBuild;

		public DefaultServerSpecifier(string serverName)
		{
			if (serverName == null)
			{
				throw new CruiseControlException("Server Specifier cannot be instantiated with a null server name");
			}
			if (serverName == string.Empty)
			{
				throw new CruiseControlException("Server Specifier cannot be instantiated with an empty server name");
			}
			this.serverName = serverName;
		}

		public DefaultServerSpecifier(string serverName, bool allowForceBuild, bool allowStartStopBuild)
		{
			this.serverName = serverName;
			this.allowForceBuild = allowForceBuild;
			this.allowStartStopBuild = allowStartStopBuild;
		}

		public string ServerName
		{
			get { return serverName; }
		}

		public bool AllowForceBuild
		{
			get { return allowForceBuild; }
		}

		public bool AllowStartStopBuild
		{
			get { return allowStartStopBuild; }
		}

		public override bool Equals(object obj)
		{
			if (obj is DefaultServerSpecifier)
			{
				DefaultServerSpecifier other = obj as DefaultServerSpecifier;
				return this.ServerName == other.ServerName;
//				return (StringUtil.EqualsIngnoreCase(this.ServerName, other.ServerName));
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public override string ToString()
		{
			return serverName;
		}
	}
}