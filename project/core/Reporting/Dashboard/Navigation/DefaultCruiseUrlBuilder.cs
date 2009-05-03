using System.Text;
using System.Web;

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
			return BuildServerUrl(action, serverSpecifier, "");
		}

		public string BuildServerUrl(string action, IServerSpecifier serverSpecifier, string queryString)
		{
			return urlBuilder.BuildUrl(
				action, 
				queryString, 
				GeneratePath(serverSpecifier.ServerName, "", ""));
		}

		public string BuildProjectUrl(string action, IProjectSpecifier projectSpecifier)
		{
			return urlBuilder.BuildUrl(
				action, 
				"",
				GeneratePath(projectSpecifier.ServerSpecifier.ServerName, projectSpecifier.ProjectName, ""));
		}

		public string BuildBuildUrl(string action, IBuildSpecifier buildSpecifier)
		{
			return urlBuilder.BuildUrl(
				action, 
				"",
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
				path.AppendFormat("{0}/{1}", ServerRESTSpecifier, HttpUtility.UrlEncode(serverName));
				if (projectName != string.Empty)
				{
					path.AppendFormat("/{0}/{1}", ProjectRESTSpecifier, HttpUtility.UrlEncode(projectName));
					if (buildName != string.Empty)
					{
						path.AppendFormat("/{0}/{1}", BuildRESTSpecifier, HttpUtility.UrlEncode(buildName));
					}
				}
			}
			return path.ToString();
		}
	}
}