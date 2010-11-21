using System.Text;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation
{
    /// <summary>
    /// 	
    /// </summary>
	public class DefaultCruiseUrlBuilder : ICruiseUrlBuilder
	{
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public static readonly string BuildRESTSpecifier = "build";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public static readonly string ProjectRESTSpecifier = "project";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public static readonly string ServerRESTSpecifier = "server";

		private readonly IUrlBuilder urlBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCruiseUrlBuilder" /> class.	
        /// </summary>
        /// <param name="urlBuilder">The URL builder.</param>
        /// <remarks></remarks>
		public DefaultCruiseUrlBuilder(IUrlBuilder urlBuilder)
		{
			this.urlBuilder = urlBuilder;
		}

        /// <summary>
        /// Builds the server URL.	
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="serverSpecifier">The server specifier.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public string BuildServerUrl(string action, IServerSpecifier serverSpecifier)
		{
			return BuildServerUrl(action, serverSpecifier, string.Empty);
		}

        /// <summary>
        /// Builds the server URL.	
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="serverSpecifier">The server specifier.</param>
        /// <param name="queryString">The query string.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public string BuildServerUrl(string action, IServerSpecifier serverSpecifier, string queryString)
		{
			return urlBuilder.BuildUrl(
				action, 
				queryString, 
				GeneratePath(serverSpecifier.ServerName, string.Empty, string.Empty));
		}

        /// <summary>
        /// Builds the project URL.	
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="projectSpecifier">The project specifier.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public string BuildProjectUrl(string action, IProjectSpecifier projectSpecifier)
		{
			return urlBuilder.BuildUrl(
				action, 
				string.Empty,
				GeneratePath(projectSpecifier.ServerSpecifier.ServerName, projectSpecifier.ProjectName, string.Empty));
		}

        /// <summary>
        /// Builds the build URL.	
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="buildSpecifier">The build specifier.</param>
        /// <returns></returns>
        /// <remarks></remarks>
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

        /// <summary>
        /// Gets or sets the extension.	
        /// </summary>
        /// <value>The extension.</value>
        /// <remarks></remarks>
		public string Extension
		{
			set { urlBuilder.Extension = value; }
			get { return urlBuilder.Extension; }
		}

        /// <summary>
        /// Gets the inner builder.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
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