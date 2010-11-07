using System.Text;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation
{
	public class DefaultCruiseUrlBuilder : ICruiseUrlBuilder
	{
		public static readonly string BuildRESTSpecifier = "build";
		public static readonly string ProjectRESTSpecifier = "project";
		public static readonly string ServerRESTSpecifier = "server";

		private readonly IUrlBuilder urlBuilder;

		public DefaultCruiseUrlBuilder(IUrlBuilder urlBuilder)
		{
			this.urlBuilder = urlBuilder;
		}

		public string BuildServerUrl(string action, IServerSpecifier serverSpecifier)
		{
			return BuildServerUrl(action, serverSpecifier, string.Empty);
		}

		public string BuildServerUrl(string action, IServerSpecifier serverSpecifier, string queryString)
		{
			return urlBuilder.BuildUrl(
				action, 
				queryString, 
				GeneratePath(serverSpecifier.ServerName, string.Empty, string.Empty));
		}

		public string BuildProjectUrl(string action, IProjectSpecifier projectSpecifier)
		{
			return urlBuilder.BuildUrl(
				action, 
				string.Empty,
				GeneratePath(projectSpecifier.ServerSpecifier.ServerName, projectSpecifier.ProjectName, string.Empty));
		}

		public string BuildBuildUrl(string action, IBuildSpecifier buildSpecifier)
		{
			return urlBuilder.BuildUrl(
				action, 
				string.Empty,
				GeneratePath(
					buildSpecifier.ProjectSpecifier.ServerSpecifier.ServerName, 
					buildSpecifier.ProjectSpecifier.ProjectName, 
					buildSpecifier.BuildName));
		}

		public string Extension
		{
			set { urlBuilder.Extension = value; }
			get { return urlBuilder.Extension; }
		}

        public IUrlBuilder InnerBuilder
        {
            get { return urlBuilder; }
        }

		private string GeneratePath(string serverName, string projectName, string buildName)
		{
			StringBuilder path = new StringBuilder();
			if (serverName != string.Empty)
			{
                path.AppendFormat("{0}/{1}", ServerRESTSpecifier, StringUtil.UrlEncodeName(serverName));
				if (projectName != string.Empty)
				{
                    path.AppendFormat("/{0}/{1}", ProjectRESTSpecifier, StringUtil.UrlEncodeName(projectName));
					if (buildName != string.Empty)
					{
                        path.AppendFormat("/{0}/{1}", BuildRESTSpecifier, StringUtil.UrlEncodeName(buildName));
					}
				}
			}
			return path.ToString();
		}
	}
}