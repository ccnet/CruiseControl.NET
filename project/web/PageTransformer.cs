using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Util;

namespace ThoughtWorks.CruiseControl.Web
{
	public class PageTransformer : IPageLoader
	{
		private ITransformer _transformer;

		public PageTransformer(string xmlFile, string xslFile)
		{
			_transformer = new LogTransformer(xmlFile, xslFile);
		}

		public PageTransformer(ITransformer transformer)
		{
			_transformer = transformer;
		}

		public string LoadPageContent()
		{
			try
			{
				return  _transformer.Transform();
			}
			catch (CruiseControlException e)
			{
				return new HtmlExceptionFormatter(e).ToString();
			}
		}
	}
}