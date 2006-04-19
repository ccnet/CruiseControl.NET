using System.Collections;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.publishers.Statistics
{
	public interface IPlotter
	{
		void DrawGraph(IList ordinateData, IList abscissaData);
		void WriteToStream(IList ordinateData, IList abscissaData, Stream stream);
	}
}