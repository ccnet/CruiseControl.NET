using System.Web.UI;
using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class BuildRequestTransformer : IBuildLogTransformer
	{
		private readonly IMultiTransformer transformer;
		private readonly IBuildRetriever buildRetriever;

		public BuildRequestTransformer(IBuildRetriever buildRetriever, IMultiTransformer transformer)
		{
			this.buildRetriever = buildRetriever;
			this.transformer = transformer;
		}

		public Control Transform(IBuildSpecifier buildSpecifier, params string[] transformerFileNames)
		{
			string log = buildRetriever.GetBuild(buildSpecifier).Log;
			HtmlGenericControl control = new HtmlGenericControl("div");
			control.InnerHtml = transformer.Transform(log, transformerFileNames);
			return control;
		}
	}
}
