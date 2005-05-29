using System.Collections;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class PathMappingMultiTransformer : IMultiTransformer
	{
		private readonly IMultiTransformer transformer;
		private readonly IPathMapper pathMapper;

		public PathMappingMultiTransformer(IPathMapper pathMapper, IMultiTransformer transformer)
		{
			this.pathMapper = pathMapper;
			this.transformer = transformer;
		}

		public string Transform(string input, string[] transformerFileNames)
		{
			ArrayList mappedFiles = new ArrayList();
			foreach (string transformerFileName in transformerFileNames)
			{
				mappedFiles.Add(pathMapper.GetLocalPathFromURLPath(transformerFileName));
			}
			return transformer.Transform(input, (string[]) mappedFiles.ToArray(typeof (string)));
		}
	}
}
