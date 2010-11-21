using System;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// Generates a URL for ViewCVS.
    /// </summary>
    /// <title>ViewCVS URL Builder</title>
    /// <version>1.0</version>
    /// <example>
    /// <code>
    /// &lt;issueUrlBuilder type="defaultIssueTracker"&gt;
    /// &lt;url&gt;http://jira.public.thoughtworks.org/browse/CCNET-{0}&lt;/url&gt;
    /// &lt;/issueUrlBuilder&gt;
    /// </code>
    /// </example>
	[ReflectorType("viewcvs")]
	public class ViewCVSUrlBuilder : IModificationUrlBuilder
	{
		private string _url;

        /// <summary>
        /// The base URL.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
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

        /// <summary>
        /// Setups the modification.	
        /// </summary>
        /// <param name="modifications">The modifications.</param>
        /// <remarks></remarks>
		public void SetupModification(Modification[] modifications)
		{
			foreach (Modification mod in modifications)
			{
				mod.Url = String.Format(_url, mod.FolderName.Length == 0 ? mod.FileName : mod.FolderName + "/" + mod.FileName);
			}
		}
	}
}