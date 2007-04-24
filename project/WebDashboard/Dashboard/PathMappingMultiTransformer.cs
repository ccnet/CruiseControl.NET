using System.Collections;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class PathMappingMultiTransformer : IMultiTransformer
	{
		private readonly IMultiTransformer transformer;
		private readonly IPhysicalApplicationPathProvider physicalApplicationPathProvider;

		public PathMappingMultiTransformer(IPhysicalApplicationPathProvider physicalApplicationPathProvider, IMultiTransformer transformer)
		{
			this.physicalApplicationPathProvider = physicalApplicationPathProvider;
			this.transformer = transformer;
		}

		public string Transform(string input, string[] transformerFileNames, Hashtable xsltArgs)
		{
			ArrayList mappedFiles = new ArrayList();
			foreach (string transformerFileName in transformerFileNames)
			{
				mappedFiles.Add(physicalApplicationPathProvider.GetFullPathFor(transformerFileName));
			}
			return transformer.Transform(input, (string[]) mappedFiles.ToArray(typeof (string)), xsltArgs);
		}
	}
}
