
namespace ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation
{
    /// <summary>
    /// 	
    /// </summary>
	public class AbsolutePathUrlBuilderDecorator : IUrlBuilder
	{
		private readonly IUrlBuilder decoratedUrlBuilder;
		private readonly string basePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbsolutePathUrlBuilderDecorator" /> class.	
        /// </summary>
        /// <param name="decoratedUrlBuilder">The decorated URL builder.</param>
        /// <param name="basePath">The base path.</param>
        /// <remarks></remarks>
		public AbsolutePathUrlBuilderDecorator(IUrlBuilder decoratedUrlBuilder, string basePath)
		{
			this.decoratedUrlBuilder = decoratedUrlBuilder;
			this.basePath = basePath;
		}

        /// <summary>
        /// Builds the URL.	
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public string BuildUrl(string action)
		{
			return Decorate(decoratedUrlBuilder.BuildUrl(action));
		}

        /// <summary>
        /// Builds the URL.	
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="partialQueryString">The partial query string.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public string BuildUrl(string action, string partialQueryString)
		{
			return Decorate(decoratedUrlBuilder.BuildUrl(action, partialQueryString));
		}

        /// <summary>
        /// Builds the URL.	
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="partialQueryString">The partial query string.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public string BuildUrl(string action, string partialQueryString, string path)
		{
			return Decorate(decoratedUrlBuilder.BuildUrl(action, partialQueryString, path));
		}

        /// <summary>
        /// Gets or sets the extension.	
        /// </summary>
        /// <value>The extension.</value>
        /// <remarks></remarks>
		public string Extension
		{
			set { decoratedUrlBuilder.Extension = value; }
			get { return decoratedUrlBuilder.Extension; }
		}

		// return string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0}://{1}:{2}{3}/{4}", context.Request.Url.Scheme, context.Request.Url.Host, context.Request.Url.Port, context.Request.ApplicationPath, relativePath);

		private string Decorate(string relativeUrl)
		{
			return string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0}{1}{2}", basePath, basePath.EndsWith("/") ? string.Empty : "/", relativeUrl);
		}
	}
}