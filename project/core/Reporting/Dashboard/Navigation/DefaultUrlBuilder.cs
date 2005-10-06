using System;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation
{
	public class DefaultUrlBuilder : IUrlBuilder
	{
		private string extension;

		public DefaultUrlBuilder()
		{
			extension = "aspx";
		}

		public string BuildUrl(string action)
		{
			return BuildUrl(action, null);
		}

		/// <summary>
		/// Assumes that the queryString and action have been safely url encoded.
		/// Instead use a parameter collection and url builder can take care of encoding.
		/// </summary>
		public string BuildUrl(string action, string queryString)
		{
			string url = string.Format("{0}.{1}", action, extension);
			if (queryString!= null && queryString != string.Empty)
			{
				url += string.Format("?{0}", queryString);
			}
			return url;
		}

		public string Extension
		{
			set { this.extension = value; }
		}
	}
}