using Exortech.NetReflector;
using System;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("websvn")]
	public class WebSVNUrlBuilder : IModificationUrlBuilder
	{
		private string _url;

		[ReflectorProperty("url")] 
		public string Url 
		{
			get { return _url; }
			set { _url = value; }
		}

		public void SetupModification(Modification[] modifications) 
		{
			foreach( Modification mod in modifications ) 
			{
				mod.Url = String.Format( _url, mod.FolderName + "/" + mod.FileName, mod.ChangeNumber );
			}
		}
	}
}
