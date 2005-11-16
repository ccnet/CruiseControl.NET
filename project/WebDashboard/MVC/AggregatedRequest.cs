using System;
using System.Collections;
using System.Collections.Specialized;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	public class AggregatedRequest : IRequest
	{
		private readonly IRequest request2;
		private readonly IRequest request1;

		public AggregatedRequest(IRequest request1, IRequest request2)
		{
			this.request1 = request1;
			this.request2 = request2;
		}

		public string FindParameterStartingWith(string prefix)
		{
			string value = request1.FindParameterStartingWith(prefix);
			if (value == null || value == string.Empty)
			{
				value = request2.FindParameterStartingWith(prefix);
			}
			return value;
		}

		public string GetText(string id)
		{
			string value = request1.GetText(id);
			if (value == null || value == string.Empty)
			{
				value = request2.GetText(id);
			}
			return value;
		}

		public bool GetChecked(string id)
		{
			bool value = request1.GetChecked(id);
			if (! value)
			{
				value = request2.GetChecked(id);
			}
			return value;
		}

		public int GetInt(string id, int defaultValue)
		{
			int value = request1.GetInt(id, defaultValue);
			if (value == defaultValue)
			{
				value = request2.GetInt(id, defaultValue);
			}
			return value;
		}

		public NameValueCollection Params
		{
			get
			{
				NameValueCollection combinedParams = new NameValueCollection(request1.Params);
				NameValueCollection request2Params = request2.Params;
				foreach (string key in request2Params)
				{
					if (combinedParams[key] == null || combinedParams[key] == string.Empty)
					{
						combinedParams[key] = request2Params[key];
					}
				}
				return combinedParams;
			}
		}

		public string RawUrl
		{
			get { return request1.RawUrl; }
		}

		public string FileNameWithoutExtension
		{
			get { return request1.FileNameWithoutExtension; }
		}

		public string[] SubFolders
		{
			get { return request1.SubFolders; }
		}

		public string ApplicationPath
		{
			get { return request1.ApplicationPath; }
		}
	}
}
