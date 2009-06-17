using System.Web;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
	public class RequestWrappingCruiseRequest : ICruiseRequest
	{
		private readonly IRequest request;
        private readonly ICruiseUrlBuilder urlBuilder;
        private readonly ISessionRetriever sessionRetriever;

        public RequestWrappingCruiseRequest(IRequest request, 
            ICruiseUrlBuilder urlBuilder,
            ISessionRetriever sessionRetriever)
		{
			this.request = request;
            this.urlBuilder = urlBuilder;
            this.sessionRetriever = sessionRetriever;
		}

        public ICruiseUrlBuilder UrlBuilder
        {
            get { return this.urlBuilder; }
		}

		public string ServerName
		{
			get { return FindRESTSpecifiedResource(DefaultCruiseUrlBuilder.ServerRESTSpecifier); }
		}

		public string ProjectName
		{
			get { return FindRESTSpecifiedResource(DefaultCruiseUrlBuilder.ProjectRESTSpecifier); }
		}

		public string BuildName
		{
			get { return FindRESTSpecifiedResource(DefaultCruiseUrlBuilder.BuildRESTSpecifier); }
		}

		private string FindRESTSpecifiedResource(string specifier)
		{
			string[] subFolders = request.SubFolders;

			for (int i = 0; i < subFolders.Length; i += 2)
			{
				if (subFolders[i] == specifier)
				{
					if (i < subFolders.Length)
					{
						return HttpUtility.UrlDecode(subFolders[i + 1]);
					}
					else
					{
						throw new CruiseControlException(
							string.Format("unexpected URL format - found {0} REST Specifier, but no following value", specifier));
					}
				}
			}

			return string.Empty;
		}

		public IServerSpecifier ServerSpecifier
		{
			get { return new DefaultServerSpecifier(ServerName); }
		}

		public IProjectSpecifier ProjectSpecifier
		{
			get { return new DefaultProjectSpecifier(ServerSpecifier, ProjectName); }
		}

		public IBuildSpecifier BuildSpecifier
		{
			get { return new DefaultBuildSpecifier(ProjectSpecifier, BuildName); }
		}

		public IRequest Request
		{
			get { return request; }
		}

        
        /// <summary>
        /// Attempt to retrieve a session token
        /// </summary>
        /// <returns></returns>
        public virtual string RetrieveSessionToken()
        {
            return RetrieveSessionToken(sessionRetriever);
        }

        /// <summary>
        /// Attempt to retrieve a session token
        /// </summary>
        /// <returns></returns>
        public virtual string RetrieveSessionToken(ISessionRetriever sessionRetriever)
        {
            // Attempt to find a session token
            string sessionToken = request.GetText("sessionToken");
            if (string.IsNullOrEmpty(sessionToken) && (sessionRetriever != null))
            {
                sessionToken = sessionRetriever.RetrieveSessionToken(request);
	}
            return sessionToken;
        }
	}
}
