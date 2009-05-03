using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation
{
	public class DefaultUrlBuilder : IUrlBuilder
	{
		private string extension;
		public static readonly string DEFAULT_EXTENSION = "aspx";
        private ISessionStorer sessionStore;

		public DefaultUrlBuilder()
		{
			extension = DEFAULT_EXTENSION;
		}

        public ISessionStorer SessionStorer
        {
            get { return sessionStore; }
            set { sessionStore = value; }
        }

		public string Extension
		{
			set { this.extension = value; }
			get { return extension; }
		}

		public string BuildUrl(string action)
		{
			return BuildUrl(action, null, null);
		}

		public string BuildUrl(string action, string queryString)
		{
			return BuildUrl(action, queryString, null);
		}

		/// <summary>
		/// Assumes that the path, queryString and action have been safely url encoded.
		/// Instead use a parameter collection and url builder can take care of encoding.
		/// </summary>
		public string BuildUrl(string action, string queryString, string path)
		{
            queryString = GenerateQuery(queryString);
			string url = string.Format("{0}{1}.{2}", CalculatePath(path), action, extension);
			if (queryString!= null && queryString != string.Empty)
			{
				url += string.Format("?{0}", queryString);
			}
			return url;
		}

        private string GenerateQuery(string queryString)
        {
            if (sessionStore != null)
            {
                if (string.IsNullOrEmpty(queryString))
                {
                    return sessionStore.GenerateQueryToken();
                }
                else
                {
                    return queryString + "&" + sessionStore.GenerateQueryToken();
                }
            }
            else
            {
                return queryString == null ? string.Empty : queryString;
            }
        }

		private string CalculatePath(string path)
		{
			if (path == null || path.Trim() == string.Empty)
			{
				return "";
			}
			else
			{
				return (path.EndsWith("/") ? path : path + "/");
			}
		}
	}
}