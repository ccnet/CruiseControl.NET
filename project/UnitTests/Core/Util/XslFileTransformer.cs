using System.IO;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class XslFileTransformer : IFileTransformer
	{
		private readonly ITransformer transformer;

		public XslFileTransformer(ITransformer transformer)
		{
			this.transformer = transformer;
		}

		public string Transform(string inputFileName, string transformerFileName)
		{
			if (! File.Exists(inputFileName))
			{
				throw new CruiseControlException(string.Format("Logfile not found: {0}", inputFileName));
			}
			using (StreamReader reader = new StreamReader(inputFileName))
			{
				return transformer.Transform(reader.ReadToEnd(), transformerFileName);
			}
		}	
	}
}