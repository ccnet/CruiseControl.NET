namespace ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation
{
	public class AbsolutePathUrlBuilderDecorator : IUrlBuilder
	{
		private readonly IUrlBuilder decoratedUrlBuilder;
		private readonly string basePath;

		public AbsolutePathUrlBuilderDecorator(IUrlBuilder decoratedUrlBuilder, string basePath)
		{
			this.decoratedUrlBuilder = decoratedUrlBuilder;
			this.basePath = basePath;
		}

		public string BuildUrl(string action)
		{
			return Decorate(decoratedUrlBuilder.BuildUrl(action));
		}

		public string BuildUrl(string action, string partialQueryString)
		{
			return Decorate(decoratedUrlBuilder.BuildUrl(action, partialQueryString));
		}

		public string BuildUrl(string action, string partialQueryString, string relativeBaseUrl)
		{
			return Decorate(decoratedUrlBuilder.BuildUrl(action, partialQueryString, relativeBaseUrl));
		}

		public string BuildFormName(string action)
		{
			return decoratedUrlBuilder.BuildFormName(action);
		}

		// return string.Format("{0}://{1}:{2}{3}/{4}", context.Request.Url.Scheme, context.Request.Url.Host, context.Request.Url.Port, context.Request.ApplicationPath, relativePath);

		private string Decorate(string relativeUrl)
		{
			return string.Format("{0}{1}{2}", basePath, basePath.EndsWith("/") ? "" : "/", relativeUrl);
		}
	}
}