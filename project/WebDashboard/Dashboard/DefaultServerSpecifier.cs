using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class DefaultServerSpecifier : IServerSpecifier
	{
		private readonly string serverName;

		public string ServerName
		{
			get { return serverName; }
		}

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

		public override bool Equals(object obj)
		{
			if (obj is DefaultServerSpecifier)
			{
				DefaultServerSpecifier other = obj as DefaultServerSpecifier;
				return (this.ServerName == other.ServerName);
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
