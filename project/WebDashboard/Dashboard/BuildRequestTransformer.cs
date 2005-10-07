using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Core.Util;

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

		public string Transform(IBuildSpecifier buildSpecifier, params string[] transformerFileNames)
		{
			string log = buildRetriever.GetBuild(buildSpecifier).Log;
			return transformer.Transform(log, transformerFileNames);
		}
	}
}
