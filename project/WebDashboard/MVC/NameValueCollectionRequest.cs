using System;
using System.Collections.Specialized;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	public class NameValueCollectionRequest : IRequest
	{
		private readonly NameValueCollection map;

		public NameValueCollectionRequest(NameValueCollection map)
		{
			this.map = map;
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
				// This is valid becuase of the way our config objects work
				return null;
			}
			else
			{
				return text;
			}
		}

		public bool GetChecked(string id)
		{
			string value = GetText(id);
			return (value != null && value =="on");
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
	}
}
