using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Web
{
	public class PageTransformer : IPageLoader
	{
		private readonly string xslFile;
		private readonly string xmlFile;
		private IFileTransformer _transformer;

		public PageTransformer(string xmlFile, string xslFile) : this(new XslFileTransformer(new XslTransformer()), xmlFile, xslFile)
		{}

		public PageTransformer(IFileTransformer fileTransformer, string xmlFile, string xslFile)
		{
			_transformer = fileTransformer;
			this.xmlFile = xmlFile;
			this.xslFile = xslFile;
		}

		public string LoadPageContent()
		{
			try
			{
				return _transformer.Transform(xmlFile, xslFile);
			}
			catch (CruiseControlException e)
			{
				return new HtmlExceptionFormatter(e).ToString();
			}
		}
	}
}