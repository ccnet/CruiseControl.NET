using System;
using System.Collections.Specialized;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	// ToDo - test!!
	public class NameValueCollectionRequest : IRequest
	{
		private readonly NameValueCollection map;
	    private readonly NameValueCollection headers;
	    private readonly string path;
		private readonly string rawUrl;
		private readonly string applicationPath;

		public NameValueCollectionRequest(NameValueCollection map, NameValueCollection headers, string path, string rawUrl, string applicationPath)
		{
			this.map = map;
		    this.headers = headers;
		    this.path = path;
			this.rawUrl = rawUrl;
			this.applicationPath = applicationPath;
		}

		public string FindParameterStartingWith(string prefix)
		{
			foreach (string key in map.Keys)
			{
				if (key.StartsWith(prefix))
				{
					return key;
				}
			}
			return "";
		}

		public string GetText(string id)
		{
			string text = map[id];
			if (text == null || text == string.Empty)
			{
				return string.Empty;
			}
			else
			{
				return text;
			}
		}

		public bool GetChecked(string id)
		{
			string value = GetText(id);
			return (value != null && value == "on");
		}

		public int GetInt(string id, int defaultValue)
		{
			// To Do - something more sensible
			string text = GetText(id);
			if (text != null && text != string.Empty)
			{
				try
				{
					return int.Parse(text);
				}
				catch (FormatException)
				{
					// Todo - exception?
					return defaultValue;
				}
			}
			else
			{
				return defaultValue;
			}
		}

		public NameValueCollection Params
		{
			get { return map; }
		}

		public string RawUrl
		{
			get { return rawUrl; }
		}

		public string FileNameWithoutExtension
		{
			get
			{
				int lastSlashIndex = path.LastIndexOf('/');
				if (lastSlashIndex == -1)
					lastSlashIndex = 0;

				int lastPeriod = path.LastIndexOf('.');
				if (lastPeriod == -1)
					lastPeriod = path.Length;

				return path.Substring(lastSlashIndex + 1, lastPeriod - lastSlashIndex - 1);
			}
		}

		public string[] SubFolders
		{
			get
			{
				string relativePath = path.Substring(applicationPath.Length + (applicationPath.EndsWith("/") ? 0 : 1));
				int lastSlashIndex = relativePath.LastIndexOf('/');
				if (lastSlashIndex < 0)
				{
					return new string[0];
				}

				return relativePath.Substring(0, lastSlashIndex).Split('/');
			}
		}

		public string ApplicationPath
		{
			get { return applicationPath; }
		}


	    public string IfModifiedSince
	    {
            get { return headers["If-Modified-Since"]; }
	    }

	    public string IfNoneMatch
	    {
            get { return headers["If-None-Match"]; }
	    }
	}
}