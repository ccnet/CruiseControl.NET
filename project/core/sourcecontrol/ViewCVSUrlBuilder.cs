using Exortech.NetReflector;
using System;

namespace ThoughtWorks.CruiseControl.Core
{
	[ReflectorType("viewcvs")]
	public class ViewCVSUrlBuilder : IUrlBuilder
	{
		private string _url;

		[ReflectorProperty("url")] 
		public string Url
		{
			get { return _url; }
			set
			{
				_url = value;
				if (!_url.EndsWith("/"))
					_url += "/";
				_url += "{0}";
			}
		}

		public void SetupModification(Modification[] modifications)
		{
			foreach (Modification mod in modifications)
			{
				mod.Url = String.Format(_url, mod.FolderName.Length == 0 ? mod.FileName : mod.FolderName + "/" + mod.FileName);
			}
		}
	}
}