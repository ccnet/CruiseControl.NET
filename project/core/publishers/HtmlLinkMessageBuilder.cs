using System.Globalization;
namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    /// <summary>
    /// 	
    /// </summary>
	public class HtmlLinkMessageBuilder : IMessageBuilder
	{
		private bool includeAnchorTag;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlLinkMessageBuilder" /> class.	
        /// </summary>
        /// <param name="includeAnchorTag">The include anchor tag.</param>
        /// <remarks></remarks>
		public HtmlLinkMessageBuilder(bool includeAnchorTag)
		{
			this.includeAnchorTag = includeAnchorTag;
		}

        /// <summary>
        /// Builds the message.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public string BuildMessage(IIntegrationResult result)
		{
			string link = result.ProjectUrl;
			if (includeAnchorTag) link = string.Format(CultureInfo.CurrentCulture, @"<a href=""{0}"">web page</a>", link);
			return string.Format(System.Globalization.CultureInfo.CurrentCulture,"CruiseControl.NET Build Results for project {0} ({1})", result.ProjectName, link);
		}


        /// <summary>
        /// Gets or sets the XSL files.	
        /// </summary>
        /// <value>The XSL files.</value>
        /// <remarks></remarks>
        public System.Collections.IList xslFiles
        {
            get
            {
                return null;
            }
            set
            {
                return;
            }
        }
    }
}
